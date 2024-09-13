// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using AspNetCore.CacheOutput.InMemory.Extensions;
using Helper;
using Helper.Factories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using ContentApiCore.Controllers;
using ContentApiCore.Swagger;
using OdhNotifier;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using SqlKata.Execution;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;

namespace ContentApiCore
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            Configuration = configuration;
            CurrentEnvironment = env;
        }
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment CurrentEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {                    
            //Adding Cache Service in Memory
            services.AddInMemoryCacheOutput();
            services.AddSingleton<CustomCacheKeyGenerator>();

            services.AddDistributedMemoryCache();

            services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy())
                .AddNpgSql(Configuration.GetConnectionString("PgConnection"), tags: new[] { "services" });

            services.AddLogging(options =>
            {
                options.ClearProviders();

                var levelSwitch = new LoggingLevelSwitch
                {
                    MinimumLevel =
                        CurrentEnvironment.IsDevelopment() ?
                            LogEventLevel.Debug :
                            LogEventLevel.Warning
                };
                var loggerConfiguration = new LoggerConfiguration()
                    .MinimumLevel.ControlledBy(levelSwitch)
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .Enrich.FromLogContext()
                    .WriteTo.Console(outputTemplate: "{Message}{NewLine}")
                    .WriteTo.Debug()
                    .CreateLogger();
                options.AddSerilog(loggerConfiguration, dispose: true);

                // Configure Serilogs own configuration to use
                // the configured logger configuration.
                // This allows to Log via Serilog's Log and ILogger.
                Log.Logger = loggerConfiguration;
            });

            services.Configure<GzipCompressionProviderOptions>(options => options.Level = System.IO.Compression.CompressionLevel.Optimal);
            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<GzipCompressionProvider>();
            });

            services.AddCors(o =>
            {
                o.AddPolicy("CorsPolicy", builder =>
                {
                    builder.AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials()
                            .SetIsOriginAllowed(hostName => true);
                });
            });

            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
            });

            services.AddRazorPages();

            services.AddSingleton<ISettings, Settings>();
            services.AddScoped<QueryFactory, PostgresQueryFactory>();
            services.AddScoped<IOdhPushNotifier, OdhPushNotifier>();

            //Initialize JWT Authentication
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                    .AddJwtBearer(jwtBearerOptions =>
                {
                    jwtBearerOptions.Authority = Configuration.GetSection("OauthServerConfig").GetValue<string>("Authority");
                    //jwtBearerOptions.Audience = "account";                
                    jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = "preferred_username",
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidIssuer = Configuration.GetSection("OauthServerConfig").GetValue<string>("Authority"),
                        ValidateIssuer = true,
                    };
                    jwtBearerOptions.Events = new JwtBearerEvents()
                    {
                        OnAuthenticationFailed = c =>
                        {
                            c.NoResult();
                            c.Response.StatusCode = 401;
                            c.Response.ContentType = "text/plain";

                            //Generate Log
                            HttpRequestExtensions.GenerateLogResponse(c.HttpContext);

                            return c.Response.WriteAsync("");
                        },
                    };
                });

            services.AddMvc(options =>
                {
                    options.OutputFormatters.Add(new Formatters.CsvOutputFormatter());
                    options.FormatterMappings.SetMediaTypeMappingForFormat("csv", "text/csv");

                    options.OutputFormatters.Add(new Formatters.JsonLdOutputFormatter());
                    //Hack only ldjson accepted
                    options.FormatterMappings.SetMediaTypeMappingForFormat("json-ld", "application/ldjson");

                    options.OutputFormatters.Add(new Formatters.RawdataOutputFormatter());
                    options.FormatterMappings.SetMediaTypeMappingForFormat("rawdata", "application/rawdata");
                });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Open Data Hub Content Api",
                    Version = "v1",
                    Description = "Open Data Hub Content Api based on .Net Core with PostgreSQL",
                    TermsOfService = new System.Uri("https://opendatahub.readthedocs.io/en/latest/"),
                    Contact = new OpenApiContact
                    {
                        Name = "Open Data Hub Team",
                        Email = "help@opendatahub.com",
                        Url = new System.Uri("https://opendatahub.com/"),
                    },
                });
                c.MapType<LegacyBool>(() => new OpenApiSchema
                {
                    Type = "boolean"
                });
                c.MapType<PageSize>(() => new OpenApiSchema
                {
                    Type = "integer"
                });
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                //var xmlPathdatamodel = Path.Combine(AppContext.BaseDirectory, $"DataModel.xml");
                c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
                //c.IncludeXmlComments(xmlPathdatamodel, includeControllerXmlComments: true);
                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        Password = new OpenApiOAuthFlow
                        {
                            TokenUrl = new Uri(Configuration.GetSection("OauthServerConfig").GetValue<string>("Authority") + "protocol/openid-connect/token")
                        },
                        ClientCredentials = new OpenApiOAuthFlow
                        {
                            TokenUrl = new Uri(Configuration.GetSection("OauthServerConfig").GetValue<string>("Authority") + "protocol/openid-connect/token")
                        }
                    },
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                c.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
                c.OperationFilter<AuthenticationRequirementsOperationFilter>();
                c.SchemaFilter<DeprecatedAttributeSchemaFilter>();
                c.SchemaFilter<EnumAttributeSchemaFilter>();
                c.EnableAnnotations();               
            });
            services.AddSwaggerGenNewtonsoftSupport();

            //Use server side caching of the swagger doc
            services.Replace(ServiceDescriptor.Transient<ISwaggerProvider, CachingSwaggerProvider>());

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedProto;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseForwardedHeaders();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseResponseCompression();
         
            app.UseStaticFiles(new StaticFileOptions()
            {
                OnPrepareResponse = ctx =>
                {
                    //Activate Browser Cache of the swagger file for 1 day                                        
                    if (ctx.File.Name.ToLower() == "swagger.json")
                    {
                        ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=86400");
                    }
                    ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
                    ctx.Context.Response.Headers.Append("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept");
                },
            });

            app.UseRouting();

            //Important! Register Cors Policy before Using Authentication and Authorization
            app.UseCors("CorsPolicy");
            app.UseAuthentication();
            app.UseAuthorization();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger(c =>
            {
                c.PreSerializeFilters.Add((swagger, httpReq) =>
                {
                    swagger.Servers = new List<OpenApiServer> { new OpenApiServer { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}" } };
                });
            });

            app.ApplicationServices.SaveSwaggerJson(Configuration.GetSection("ApiConfig").GetValue<string>("Url"), Configuration.GetSection("JsonConfig").GetValue<string>("Jsondir"));

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Open Data Hub Content API V1");
                c.RoutePrefix = "swagger";
                c.OAuthClientSecret("");
                c.OAuthRealm("noi");
                c.EnableDeepLinking();
            });

            app.UseRateLimiting();

            app.UseKeycloakAuthorizationService();

            ////LOG EVERY REQUEST WITH HEADERs
            app.UseODHCustomHttpRequestConfig(Configuration);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseHealthChecks("/self", new HealthCheckOptions
            {
                Predicate = r => r.Name.Contains("self")
            });

            app.UseHealthChecks("/ready", new HealthCheckOptions
            {
                Predicate = r => r.Tags.Contains("services")
            });
        }
    }

   
}

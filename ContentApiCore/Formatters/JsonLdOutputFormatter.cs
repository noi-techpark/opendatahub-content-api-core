// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using ContentApiModels;
using DataModel;
using Helper.JsonHelpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ContentApiCore.Formatters
{
    public class JsonLdOutputFormatter : TextOutputFormatter
    {
        public JsonLdOutputFormatter()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/ld+json"));
            // Hack because Output formatter Mapping does not work with + inside
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/ldjson"));            

            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);
        }

        private List<object>? Transform(PathString path, JsonRaw jsonRaw, string language, string currentroute)
        {
            //TODO: extract language
            var settings = new JsonSerializerSettings { ContractResolver = new GetOnlyContractResolver() };


            if (path.StartsWithSegments("/v1/Example"))
            {
                var acco = JsonConvert.DeserializeObject<Example>(jsonRaw.Value, settings);
                return JsonLDTransformer.TransformToSchemaNet.TransformDataToSchemaNet(acco, currentroute, "example", language);
            }       
            else
            {
                return null;
            }
        }

        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            if (context.Object is JsonRaw jsonRaw)
            {
                //Get the requested language
                var query = context.HttpContext.Request.Query;
                string language = (string?)query["language"] ?? "en";

                //Get the route
                var location = new Uri($"{context.HttpContext.Request.Scheme}://{context.HttpContext.Request.Host}/{context.HttpContext.Request.Path}");
                var currentroute = location.AbsoluteUri;


                var transformed = Transform(context.HttpContext.Request.Path, jsonRaw, language, currentroute);
                if (transformed != null)
                {
                    var jsonLD = "";
                    var options = new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault };

                    if (transformed.Count == 1)
                    {
                        jsonLD = System.Text.Json.JsonSerializer.Serialize(transformed.FirstOrDefault(), options); //TODO REMOVE NULL VALUES
                            //JsonConvert.SerializeObject(transformed.FirstOrDefault(), Newtonsoft.Json.Formatting.None, jsonsettings);
                                    
                    }
                    else if (transformed.Count > 1)
                    {
                        jsonLD = System.Text.Json.JsonSerializer.Serialize(transformed, options);
                    }

                    
                    await context.HttpContext.Response.WriteAsync(jsonLD);
                    //await context.HttpContext.Response.WriteAsync(transformed);
                }
                else
                {
                    await OutputFormatterHelper.NotImplemented(context);
                }
            }
            else
            {
                await OutputFormatterHelper.BadRequest(context);
            }
        }
    }
}

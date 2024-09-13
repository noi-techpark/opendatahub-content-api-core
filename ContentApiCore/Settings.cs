// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using Helper;

namespace ContentApiCore
{       
    public class Settings : ISettings
    {
        private readonly IConfiguration configuration;
        private readonly Lazy<string> connectionString;
        private readonly XmlConfig xmlConfig;
        private readonly JsonConfig jsonConfig;
        private readonly S3ImageresizerConfig s3imageresizerConfig;        
        private readonly PushServerConfig pushserverConfig;        
        private readonly List<Field2HideConfig> field2hideConfig;
        private readonly List<RequestInterceptorConfig> requestInterceptorConfig;
        private readonly List<RateLimitConfig> rateLimitConfig;
        private readonly NoRateLimitConfig noRateLimitConfig;
        private readonly List<FCMConfig> fcmConfig;

        private readonly List<NotifierConfig> notifierConfig;


        public Settings(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.connectionString = new Lazy<string>(() =>
            this.configuration.GetConnectionString("PGConnection"));
            var xml = this.configuration.GetSection("XmlConfig");
            this.xmlConfig = new XmlConfig(xml.GetValue<string>("Xmldir", ""), xml.GetValue<string>("XmldirWeather", ""));
            var json = this.configuration.GetSection("JsonConfig");
            this.jsonConfig = new JsonConfig(json.GetValue<string>("Jsondir", ""));
            var s3img = this.configuration.GetSection("S3ImageresizerConfig");
            this.s3imageresizerConfig = new S3ImageresizerConfig(s3img.GetValue<string>("Url", ""), s3img.GetValue<string>("DocUrl", ""), s3img.GetValue<string>("BucketAccessPoint", ""), s3img.GetValue<string>("AccessKey", ""), s3img.GetValue<string>("SecretKey", ""));
            var pushserver = this.configuration.GetSection("PushServerConfig");
            this.pushserverConfig = new PushServerConfig(pushserver.GetValue<string>("Username", ""), pushserver.GetValue<string>("Password", ""), pushserver.GetValue<string>("ServiceUrl", ""));
            var field2hidelist = this.configuration.GetSection("Field2HideConfig").GetChildren();
            this.field2hideConfig = new List<Field2HideConfig>();
            foreach (var field2hide in field2hidelist)
            {
                this.field2hideConfig.Add(new Field2HideConfig(field2hide.GetValue<string>("Entity", ""), field2hide.GetValue<string>("Fields", ""), field2hide.GetValue<string>("DisplayOnRoles", "")));
            }

            var requestinterceptorlist = this.configuration.GetSection("RequestInterceptorConfig").GetChildren();
            this.requestInterceptorConfig = new List<RequestInterceptorConfig>();

            foreach (var requestinterceptor in requestinterceptorlist)
            {
                this.requestInterceptorConfig.Add(new RequestInterceptorConfig(requestinterceptor.GetValue<string>("Action", ""), requestinterceptor.GetValue<string>("Controller", ""), requestinterceptor.GetValue<string>("QueryStrings", ""), 
                    requestinterceptor.GetValue<string>("RedirectAction", ""), requestinterceptor.GetValue<string>("RedirectController", ""), requestinterceptor.GetValue<string>("RedirectQueryStrings", "")));
            }

            var ratelimitlist = this.configuration.GetSection("RateLimitConfig").GetChildren();
            this.rateLimitConfig = new List<RateLimitConfig>();
            foreach (var ratelimitconfig in ratelimitlist)
            {
                this.rateLimitConfig.Add(new RateLimitConfig(ratelimitconfig.GetValue<string>("Type", ""), ratelimitconfig.GetValue<int>("TimeWindow", 0), ratelimitconfig.GetValue<int>("MaxRequests", 0)));
            }

            var noratelimitroutes = this.configuration.GetSection("NoRateLimitConfig").GetSection("NoRateLimitRoutesConfig").GetChildren();
            var noratelimitreferers = this.configuration.GetSection("NoRateLimitConfig").GetSection("NoRateLimitRefererConfig").GetChildren();
            this.noRateLimitConfig = new NoRateLimitConfig(new List<string>(), new List<string>());            
            foreach (var routepath in noratelimitroutes)
            {
                this.noRateLimitConfig.NoRateLimitRoutes.Add(routepath.GetValue<string>("Path",""));                
            }
            foreach (var referer in noratelimitreferers)
            {
                this.noRateLimitConfig.NoRateLimitReferer.Add(referer.GetValue<string>("Referer", ""));
            }

            this.fcmConfig = new List<FCMConfig>();

            var fcmdict = this.configuration.GetSection("FCMConfig").GetChildren();
            if(fcmdict != null)
            {
                foreach (var fcmkey in fcmdict)
                {
                    var fcmconfigobj = new FCMConfig(fcmkey.Key, fcmkey.GetValue<string>("ServerKey", ""), fcmkey.GetValue<string>("SenderId", ""), fcmkey.GetValue<string>("ProjectName", ""), fcmkey.GetValue<string>("FCMServiceAccount", ""), fcmkey.GetValue<string>("FCMServiceAccountJson", ""));
                    this.fcmConfig.Add(fcmconfigobj);
                }
            }

            this.notifierConfig = new List<NotifierConfig>();

            var notifierconfigdict = this.configuration.GetSection("NotifierConfig").GetChildren();
            if (notifierconfigdict != null)
            {
                foreach (var notifiercfg in notifierconfigdict)
                {
                    this.notifierConfig.Add(new NotifierConfig(notifiercfg.Key, notifiercfg.GetValue<string>("Url", ""), notifiercfg.GetValue<string>("User", ""), notifiercfg.GetValue<string>("Password", "")));
                }
            }
        }

        public string PostgresConnectionString => this.connectionString.Value;
        public XmlConfig XmlConfig => this.xmlConfig;
        public JsonConfig JsonConfig => this.jsonConfig;
        public S3ImageresizerConfig S3ImageresizerConfig => this.s3imageresizerConfig;
        public PushServerConfig PushServerConfig => this.pushserverConfig;
        public List<FCMConfig> FCMConfig => this.fcmConfig;
        public List<Field2HideConfig> Field2HideConfig => this.field2hideConfig;
        public List<RequestInterceptorConfig> RequestInterceptorConfig => this.requestInterceptorConfig;
        public List<RateLimitConfig> RateLimitConfig => this.rateLimitConfig;
        public NoRateLimitConfig NoRateLimitConfig => this.noRateLimitConfig;

        public List<NotifierConfig> NotifierConfig => this.notifierConfig;
    }
}

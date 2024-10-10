// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using ContentApiModels;
using DataModel;
using Helper;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using static ContentApiCoreTests.IntegrationTests.Helpers;

namespace ContentApiCoreTests.IntegrationTests
{
    [Trait("Category", "Integration")]
    public class ExampleApiControllerTests : IClassFixture<CustomWebApplicationFactory<ContentApiCore.Startup>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<ContentApiCore.Startup> _factory;

        public ExampleApiControllerTests(CustomWebApplicationFactory<ContentApiCore.Startup> factory)
        {
            _factory = factory;
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Theory]
        [InlineData("/v1/Example")]
        [InlineData("/v1/Example?pagesize=1")]        
        [InlineData("/v1/Example?language=de")]        
        [InlineData("/v1/Example?pagenumber=1&pagesize=10&seed=null")]
        [InlineData("/v1/Example?pagenumber=1&pagesize=20&active=true&seed=null")]
        public async Task Get_Examples(string url)
        {
            var response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType?.ToString());
            string json = await response.Content.ReadAsStringAsync();
            dynamic? data = JsonConvert.DeserializeObject(json);
            Assert.NotNull(data);
            if (data != null)
            {
                Assert.IsType<JObject>(data);
                JsonIsType<long>(data.TotalResults);
                Assert.NotEqual(0, (long)data.TotalResults);
                JsonIsType<long>(data.TotalPages);
                Assert.NotEqual(0, (long)data.TotalPages);
                JsonIsType<long>(data.CurrentPage);
                Assert.Equal(1, (long)data.CurrentPage);
                JsonIsType<string>(data.Seed);
                Assert.Empty(data.Seed);
                Assert.IsType<JArray>(data.Items);
                Assert.NotEmpty(data.Items);
            }
        }

        [Theory]
        [InlineData("/v1/Example/123456789")]
        [InlineData("/v1/Example/12345678910")]
        public async Task Get_SinglePoi(string url)
        {
            var response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType?.ToString());
            string json = await response.Content.ReadAsStringAsync();
            dynamic? data = JsonConvert.DeserializeObject(json);
            Assert.NotNull(data);
            if (data != null)
            {
                Assert.IsType<JObject>(data);
                JsonIsType<string>(data.Id);
                JsonIsType<string>(data.Type);
                JsonIsType<string?>(data.SmgId);
                JsonIsType<bool>(data.Active);
                //Assert.IsType<JArray>(data.AreaId);
                //Assert.NotEmpty(data.AreaId);
                Assert.IsType<JObject>(data.Detail);
                Assert.IsType<JObject>(data.Detail.de);
                JsonIsType<string>(data.Detail.de.Title);
                //JsonIsType<bool>(data.IsOpen);
                Assert.IsType<JArray>(data.GpsInfo);
                //JsonIsType<bool>(data.Highlight);
                Assert.IsType<JArray>(data.HasLanguage);
                Assert.IsType<JObject>(data.ContactInfos);
                Assert.IsType<JObject>(data.ContactInfos.de);
                JsonIsType<string>(data.TourismorganizationId);
            }
        }

        [Theory]
        [InlineData("/v1/Poi/0000000")]
        [InlineData("/v1/Poi/0000001")]
        public async Task Get_SingleNonExistentPoi(string url)
        {
            var response = await _client.GetAsync(url);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Get_ExampleTypes()
        {
            var response = await _client.GetAsync("/v1/ExampleTypes");
            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType?.ToString());
            string json = await response.Content.ReadAsStringAsync();
            dynamic? data = JsonConvert.DeserializeObject<ExampleTypes[]>(json);
            Assert.NotEmpty(data);
        }
    }
}

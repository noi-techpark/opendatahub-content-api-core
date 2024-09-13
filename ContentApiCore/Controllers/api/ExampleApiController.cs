// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using AspNetCore.CacheOutput;
using DataModel;
using Helper;
using Helper.Generic;
using Helper.Identity;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ContentApiCore.Responses;
using OdhNotifier;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ContentApiCore.Controllers.api
{
    /// <summary>
    /// Examples Api
    /// </summary>
    [EnableCors("CorsPolicy")]
    [NullStringParameterActionFilter]
    public class ExampleController : OdhController
    {      
        public ExampleController(IWebHostEnvironment env, ISettings settings, ILogger<ExampleController> logger, QueryFactory queryFactory, IOdhPushNotifier odhpushnotifier)
           : base(env, settings, logger, queryFactory, odhpushnotifier)
        {
        }

        #region SWAGGER Exposed API

        //Standard GETTER

        /// <summary>
        /// GET Example List
        /// </summary>
        /// <param name="pagenumber">Pagenumber</param>
        /// <param name="pagesize">Elements per Page, (default:10)</param>
        /// <param name="seed">Seed '1 - 10' for Random Sorting, '0' generates a Random Seed, 'null' disables Random Sorting, (default:null)</param>
        /// <param name="idlist">IDFilter (Separator ',' List of IDs), (default:'null')</param>
        /// <param name="exampletype">Example Types Filter (Separator ',' List of ExampleTypes IDs), (default:'null')</param>
        /// <param name="active">Active Filter (possible Values: 'true' only Active Data, 'false' only Disabled Data), (default:'null')</param>
        /// <param name="source">Filter by Source (Separator ','), (Sources available 'idm','noi'...),(default: 'null')</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="language">Language field selector, displays data and fields in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="langfilter">Language filter (returns only data available in the selected Language, Separator ',' possible values: 'de,it,en,nl,sc,pl,fr,ru', 'null': Filter disabled)</param>
        /// <param name="publishedon">Published On Filter (Separator ',' List of publisher IDs), (default:'null')</param>       
        /// <param name="updatefrom">Returns data changed after this date Format (yyyy-MM-dd), (default: 'null')</param>
        /// <param name="searchfilter">String to search for, Title in all languages are searched, (default: null) <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#searchfilter" target="_blank">Wiki searchfilter</a></param>
        /// <param name="rawfilter"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter" target="_blank">Wiki rawfilter</a></param>
        /// <param name="rawsort"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawsort" target="_blank">Wiki rawsort</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false. Documentation on <a href='https://github.com/noi-techpark/odh-docs/wiki/Common-parameters,-fields,-language,-searchfilter,-removenullvalues,-updatefrom#removenullvalues' target="_blank">Opendatahub Wiki</a></param>        
        /// <returns>Collection of Example Objects</returns>        
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(JsonResult<ExampleLinked>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[OdhCacheOutput(ClientTimeSpan = 0, ServerTimeSpan = 3600, CacheKeyGenerator = typeof(CustomCacheKeyGenerator), MustRevalidate = true)]
        [HttpGet, Route("Example")]
        public async Task<IActionResult> GetExamples(
            string? language = null,
            uint pagenumber = 1,
            PageSize pagesize = null!,
            string? exampletype = null,
            string? idlist = null,
            string? langfilter = null,
            LegacyBool active = null!,
            string? updatefrom = null,
            string? seed = null,
            string? publishedon = null,
            string? source = null,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? searchfilter = null,
            string? rawfilter = null,
            string? rawsort = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {

            return await GetExampleList(
                fields: fields ?? Array.Empty<string>(), language: language, pagenumber: pagenumber, pagesize: pagesize,
                type: exampletype, searchfilter: searchfilter, idfilter: idlist, languagefilter: langfilter, 
                active: active?.Value, seed: seed, source: source, lastchange: updatefrom, publishedon: publishedon,
                rawfilter: rawfilter, rawsort: rawsort, removenullvalues: removenullvalues, cancellationToken);
        }

        /// <summary>
        /// GET Example Single 
        /// </summary>
        /// <param name="id">ID of the Example</param>
        /// <param name="language">Language field selector, displays data and fields in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false. Documentation on <a href='https://github.com/noi-techpark/odh-docs/wiki/Common-parameters,-fields,-language,-searchfilter,-removenullvalues,-updatefrom#removenullvalues' target="_blank">Opendatahub Wiki</a></param>        
        /// <returns>Example Object</returns>
        /// <response code="200">Object created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(ExampleLinked), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("Example/{id}", Name = "SingleExample")]
        public async Task<IActionResult> GetExample(
            string id,
            string? language,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {
            return await GetExampleSingle(id, language, fields: fields ?? Array.Empty<string>(), removenullvalues, cancellationToken);
        }

        //Special GETTER

        /// <summary>
        /// GET ExampleTypes List
        /// </summary>
        /// <param name="language">Language field selector, displays data and fields in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="searchfilter">String to search for, Title in all languages are searched, (default: null) <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#searchfilter" target="_blank">Wiki searchfilter</a></param>
        /// <param name="rawfilter"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter" target="_blank">Wiki rawfilter</a></param>
        /// <param name="rawsort"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawsort" target="_blank">Wiki rawsort</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false. Documentation on <a href='https://github.com/noi-techpark/odh-docs/wiki/Common-parameters,-fields,-language,-searchfilter,-removenullvalues,-updatefrom#removenullvalues' target="_blank">Opendatahub Wiki</a></param>        
        /// <returns>Collection of ExampleTypes Object</returns>                
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(IEnumerable<ExampleTypes>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("ExampleTypes")]
        public async Task<IActionResult> GetExampleTypes(
            string? language,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? searchfilter = null,
            string? rawfilter = null,
            string? rawsort = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {
            return await GetExampleTypesList(language, fields: fields ?? Array.Empty<string>(), searchfilter, rawfilter, rawsort, removenullvalues, cancellationToken);
        }

        /// <summary>
        /// GET ExampleTypes Single
        /// </summary>
        /// <param name="id">ID of the ExampleType</param>
        /// <param name="language">Language field selector, displays data and fields in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false. Documentation on <a href='https://github.com/noi-techpark/odh-docs/wiki/Common-parameters,-fields,-language,-searchfilter,-removenullvalues,-updatefrom#removenullvalues' target="_blank">Opendatahub Wiki</a></param>        
        /// <returns>ExampleTypes Object</returns>                
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(ExampleTypes), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("ExampleTypes/{id}", Name = "SingleExampleTypes")]
        public async Task<IActionResult> GetExampleType(
            string id,
            string? language,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {
            return await GetExampleTypeSingle(id, language, fields: fields ?? Array.Empty<string>(), removenullvalues, cancellationToken);
        }

        #endregion

        #region GETTER

        private Task<IActionResult> GetExampleList(string[] fields, string? language, uint pagenumber, int? pagesize,
            string? type, string? searchfilter, string? idfilter, string? languagefilter,
            bool? active, string? seed, string? source, string? lastchange, 
            string? publishedon, string? rawfilter, string? rawsort, bool removenullvalues,
            CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                //Additional Read Filters to Add Check
                AdditionalFiltersToAdd.TryGetValue("Read", out var additionalfilter);

                ExampleHelper examplehelper = ExampleHelper.Create(
                    type, idfilter, languagefilter, active, source, lastchange, publishedon);
                
                var query =
                    QueryFactory.Query()
                        .SelectRaw("data")
                        .From("examples")
                        .ExampleWhereExpression(
                            idlist: examplehelper.idlist, typelist: examplehelper.typelist,
                            languagelist: examplehelper.languagelist, activefilter: examplehelper.active, 
                            sourcelist: examplehelper.sourcelist,
                            publishedonlist: examplehelper.publishedonlist,
                            searchfilter: searchfilter, language: language, lastchange: examplehelper.lastchange,
                            additionalfilter: additionalfilter,
                            userroles: UserRolesToFilter)
                        .ApplyRawFilter(rawfilter)
                        .ApplyOrdering_GeneratedColumns(ref seed, new PGGeoSearchResult() { geosearch = false }, rawsort);
                      

                // Get paginated data
                var data =
                    await query
                        .PaginateAsync<JsonRaw>(
                            page: (int)pagenumber,
                            perPage: pagesize ?? 25);
                                
                var dataTransformed =
                    data.List.Select(
                        raw => raw.TransformRawData(language, fields, filteroutNullValues: removenullvalues, urlGenerator: UrlGenerator, fieldstohide: null)
                    );

                uint totalpages = (uint)data.TotalPages;
                uint totalcount = (uint)data.Count;

                return ResponseHelpers.GetResult(
                    pagenumber,
                    totalpages,
                    totalcount,
                    seed,
                    dataTransformed,
                    Url);
            });
        }

        private Task<IActionResult> GetExampleSingle(string id, string? language, string[] fields, bool removenullvalues, CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                //Additional Read Filters to Add Check
                AdditionalFiltersToAdd.TryGetValue("Read", out var additionalfilter);

                var query =
                    QueryFactory.Query("examples")
                        .Select("data")
                        .Where("id", id.ToUpper())
                        .When(!String.IsNullOrEmpty(additionalfilter), q => q.FilterAdditionalDataByCondition(additionalfilter))
                        .FilterDataByAccessRoles(UserRolesToFilter);

                var data = await query.FirstOrDefaultAsync<JsonRaw?>();
                
                return data?.TransformRawData(language, fields, filteroutNullValues: removenullvalues, urlGenerator: UrlGenerator, fieldstohide: null);
            });
        }

        #endregion

        #region CATEGORIES

        private Task<IActionResult> GetExampleTypesList(string? language, string[] fields, string? searchfilter, string? rawfilter, string? rawsort, bool removenullvalues, CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                var query =
                    QueryFactory.Query("exampletypes")
                        .SelectRaw("data")
                        .SearchFilter(PostgresSQLWhereBuilder.TypeDescFieldsToSearchFor(language), searchfilter)
                        .ApplyRawFilter(rawfilter)
                        .OrderOnlyByRawSortIfNotNull(rawsort);

                var data = await query.GetAsync<JsonRaw?>();
                                
                return
                      data.Select(
                          raw => raw?.TransformRawData(language, fields, filteroutNullValues: removenullvalues, urlGenerator: UrlGenerator, fieldstohide: null)
                      );
            });
        }

        private Task<IActionResult> GetExampleTypeSingle(string id, string? language, string[] fields, bool removenullvalues, CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                var query =
                    QueryFactory.Query("exampletypes")
                        .Select("data")                        
                        .Where("id", id.ToLower());
                               
                var data = await query.FirstOrDefaultAsync<JsonRaw?>();

                return data?.TransformRawData(language, fields, filteroutNullValues: removenullvalues, urlGenerator: UrlGenerator, fieldstohide: null);
            });
        }

        #endregion

        #region POST PUT DELETE

        /// <summary>
        /// POST Insert new Example
        /// </summary>
        /// <param name="example">Example Object</param>
        /// <returns>Http Response</returns>                
        [AuthorizeODH(PermissionAction.Create)]
        [InvalidateCacheOutput(nameof(GetExampleList))]
        [ProducesResponseType(typeof(PGCRUDResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost, Route("Example")]
        public Task<IActionResult> Post([FromBody] ExampleLinked example)
        {            
            return DoAsyncReturn(async () =>
            {
                //Additional Filters on the Action Create
                AdditionalFiltersToAdd.TryGetValue("Create", out var additionalfilter);

                //GENERATE ID
                example.Id = Helper.IdGenerator.GenerateIDFromType(example);

                example.CheckMyInsertedLanguages(new List<string> { "de", "en", "it" });

                if (example.LicenseInfo == null)
                    example.LicenseInfo = new LicenseInfo() { ClosedData = false };

                example.TrimStringProperties();

                return await UpsertData<ExampleLinked>(example, new DataInfo("example", CRUDOperation.Create), new CompareConfig(true, true), new CRUDConstraints(additionalfilter, UserRolesToFilter));
            });
        }

        /// <summary>
        /// PUT Modify existing Example
        /// </summary>
        /// <param name="id">Example Id</param>
        /// <param name="example">Example Object</param>
        /// <returns>Http Response</returns>
        //[ApiExplorerSettings(IgnoreApi = true)]        
        [AuthorizeODH(PermissionAction.Update)]
        [InvalidateCacheOutput(nameof(GetExampleList))]
        [ProducesResponseType(typeof(PGCRUDResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPut, Route("Example/{id}")]
        public Task<IActionResult> Put(string id, [FromBody] ExampleLinked example)
        {            
            return DoAsyncReturn(async () =>
            {
                //Additional Filters on the Action Create
                AdditionalFiltersToAdd.TryGetValue("Update", out var additionalfilter);

                //Check ID uppercase lowercase
                example.Id = Helper.IdGenerator.CheckIdFromType<ExampleLinked>(id);

                if (String.IsNullOrEmpty(example.Source))
                    example.Source = "noi";

                example.CheckMyInsertedLanguages(new List<string> { "de", "en", "it" });

                example.TrimStringProperties();

                return await UpsertData<ExampleLinked>(example, new DataInfo("examples", CRUDOperation.Update), new CompareConfig(true, true), new CRUDConstraints(additionalfilter, UserRolesToFilter));
            });
        }

        /// <summary>
        /// DELETE Example by Id
        /// </summary>
        /// <param name="id">Example Id</param>
        /// <returns>Http Response</returns>
        //[ApiExplorerSettings(IgnoreApi = true)]        
        [AuthorizeODH(PermissionAction.Delete)]
        [InvalidateCacheOutput(nameof(GetExampleList))]
        [ProducesResponseType(typeof(PGCRUDResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpDelete, Route("Example/{id}")]
        public Task<IActionResult> Delete(string id)
        {
            return DoAsyncReturn(async () =>
            {
                //Additional Filters on the Action Create
                AdditionalFiltersToAdd.TryGetValue("Delete", out var additionalfilter);

                //Check ID uppercase lowercase
                id = Helper.IdGenerator.CheckIdFromType<ExampleLinked>(id);

                return await DeleteData<ExampleLinked>(id, new DataInfo("examples", CRUDOperation.Delete), new CRUDConstraints(additionalfilter, UserRolesToFilter));
            });
        }

        #endregion
    }
}

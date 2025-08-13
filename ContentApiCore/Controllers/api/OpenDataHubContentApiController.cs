// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using AspNetCore.CacheOutput;
using ContentApiCore.Responses;
using ContentApiModels;
using DataModel;
using Helper;
using Helper.Generic;
using Helper.Identity;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OdhNotifier;
using Schema.NET;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ContentApiCore.Controllers.api
{
    /// <summary>
    /// OpenDataHubContent Api
    /// </summary>
    [EnableCors("CorsPolicy")]
    [NullStringParameterActionFilter]
    public class OpenDataHubContentController : OdhController
    {      
        public OpenDataHubContentController(IWebHostEnvironment env, ISettings settings, ILogger<ExampleController> logger, QueryFactory queryFactory, IOdhPushNotifier odhpushnotifier)
           : base(env, settings, logger, queryFactory, odhpushnotifier)
        {
        }

        #region SWAGGER Exposed API

        //Standard GETTER

        /// <summary>
        /// GET OpenDataHubContent List
        /// </summary>
        /// <param name="pagenumber">Pagenumber</param>
        /// <param name="pagesize">Elements per Page, (default:10)</param>
        /// <param name="seed">Seed '1 - 10' for Random Sorting, '0' generates a Random Seed, 'null' disables Random Sorting, (default:null)</param>
        /// <param name="idlist">IDFilter (Separator ',' List of IDs), (default:'null')</param>
        /// <param name="source">Filter by Source (Separator ','), (Sources available 'idm','noi'...),(default: 'null')</param>
        /// <param name="language">Language field selector, displays data and fields in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="langfilter">Language filter (returns only data available in the selected Language, Separator ',' possible values: 'de,it,en,nl,sc,pl,fr,ru', 'null': Filter disabled)</param>
        /// <param name="publishedon">Published On Filter (Separator ',' List of publisher IDs), (default:'null')</param>       
        /// <param name="updatefrom">Returns data changed after this date Format (yyyy-MM-dd), (default: 'null')</param>
        /// <param name="latitude">GeoFilter FLOAT Latitude Format: '46.624975', 'null' = disabled, (default:'null') <a href='https://github.com/noi-techpark/odh-docs/wiki/Geosorting-and-Locationfilter-usage#geosorting-functionality' target="_blank">Wiki geosort</a></param>
        /// <param name="longitude">GeoFilter FLOAT Longitude Format: '11.369909', 'null' = disabled, (default:'null') <a href='https://github.com/noi-techpark/odh-docs/wiki/Geosorting-and-Locationfilter-usage#geosorting-functionality' target="_blank">Wiki geosort</a></param>
        /// <param name="radius">Radius INTEGER to Search in Meters. Only Object withhin the given point and radius are returned and sorted by distance. Random Sorting is disabled if the GeoFilter Informations are provided, (default:'null') <a href='https://github.com/noi-techpark/odh-docs/wiki/Geosorting-and-Locationfilter-usage#geosorting-functionality' target="_blank">Wiki geosort</a></param>
        /// <param name="polygon">valid WKT (Well-known text representation of geometry) Format, Examples (POLYGON ((30 10, 40 40, 20 40, 10 20, 30 10))) / By Using the GeoShapes Api (v1/GeoShapes) and passing Country.Type.Id OR Country.Type.Name Example (it.municipality.3066) / Bounding Box Filter bbc: 'Bounding Box Contains', 'bbi': 'Bounding Box Intersects', followed by a List of Comma Separated Longitude Latitude Tuples, 'null' = disabled, (default:'null') <a href='https://github.com/noi-techpark/odh-docs/wiki/Geosorting-and-Locationfilter-usage#polygon-filter-functionality' target="_blank">Wiki geosort</a></param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="searchfilter">String to search for, Title in all languages are searched, (default: null) <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#searchfilter" target="_blank">Wiki searchfilter</a></param>
        /// <param name="rawfilter"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter" target="_blank">Wiki rawfilter</a></param>
        /// <param name="rawsort"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawsort" target="_blank">Wiki rawsort</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false. Documentation on <a href='https://github.com/noi-techpark/odh-docs/wiki/Common-parameters,-fields,-language,-searchfilter,-removenullvalues,-updatefrom#removenullvalues' target="_blank">Opendatahub Wiki</a></param>        
        /// <returns>Collection of OpenDataHubContentLinked Objects</returns>        
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(JsonResult<OpenDataHubContentLinked>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]        
        [HttpGet, Route("OpenDataHubContent")]
        public async Task<IActionResult> GetOpenDataHubContents(
            string? language = null,
            uint pagenumber = 1,
            PageSize pagesize = null!,            
            string? idlist = null,
            string? langfilter = null,
            string? updatefrom = null,
            string? seed = null,
            LegacyBool active = null!,
            string? publishedon = null,
            string? source = null,
            string? latitude = null,
            string? longitude = null,
            string? radius = null,
            string? polygon = null,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? searchfilter = null,
            string? rawfilter = null,
            string? rawsort = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {
            var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(
                latitude,
                longitude,
                radius
            );
            var polygonsearchresult = await Helper.GeoSearchHelper.GetPolygon(
                polygon,
                QueryFactory
            );

            return await GetOpenDataHubContentList(
                fields: fields ?? Array.Empty<string>(), language: language, pagenumber: pagenumber, pagesize: pagesize,
                searchfilter: searchfilter, idfilter: idlist, languagefilter: langfilter, 
                active: active?.Value, seed: seed, source: source, lastchange: updatefrom, publishedon: publishedon,
                polygonsearchresult : polygonsearchresult, geosearchresult : geosearchresult,
                rawfilter: rawfilter, rawsort: rawsort, removenullvalues: removenullvalues, cancellationToken);
        }

        /// <summary>
        /// GET OpenDataHubContent Single 
        /// </summary>
        /// <param name="id">ID of the OpenDataHubContent</param>
        /// <param name="language">Language field selector, displays data and fields in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false. Documentation on <a href='https://github.com/noi-techpark/odh-docs/wiki/Common-parameters,-fields,-language,-searchfilter,-removenullvalues,-updatefrom#removenullvalues' target="_blank">Opendatahub Wiki</a></param>        
        /// <returns>OpenDataHubContentLinked Object</returns>
        /// <response code="200">Object created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(OpenDataHubContentLinked), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("OpenDataHubContent/{id}", Name = "SingleOpenDataHubContent")]
        public async Task<IActionResult> OpenDataHubContent(
            string id,
            string? language,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {
            return await GetOpenDataHubContentSingle(id, language, fields: fields ?? Array.Empty<string>(), removenullvalues, cancellationToken);
        }
      
        #endregion

        #region GETTER

        private Task<IActionResult> GetOpenDataHubContentList(string[] fields, string language, uint pagenumber, int? pagesize,           
            string? searchfilter, string? idfilter, string? languagefilter,
            bool? active, string? seed, string? source, string? lastchange, string? publishedon,
            GeoPolygonSearchResult? polygonsearchresult, PGGeoSearchResult geosearchresult,
            string? rawfilter, string? rawsort, bool removenullvalues,
            CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                //Additional Read Filters to Add Check
                AdditionalFiltersToAdd.TryGetValue("Read", out var additionalfilter);

                var query =
                    QueryFactory.Query()
                        .SelectRaw("data")
                        .From("opendatahubcontent")
                        .When(idfilter != null, x => x.IdUpperFilter(idfilter.Split(",")))
                        .When(publishedon != null, x => x.PublishedOnFilter_GeneratedColumn(publishedon.Split(",")))
                        .When(source != null, x => x.SourceFilter_GeneratedColumn(source.Split(",")))
                        .LastChangedFilter_GeneratedColumn(lastchange)
                        .SearchFilter(PostgresSQLWhereBuilder.TitleFieldsToSearchFor(language, languagefilter != null ? languagefilter.Split(",") : null), searchfilter)
                        .When(!String.IsNullOrEmpty(additionalfilter), q => q.FilterAdditionalDataByCondition(additionalfilter))
                        .FilterDataByAccessRoles(UserRolesToFilter)
                        .When(
                        polygonsearchresult != null,
                        x =>
                            x.WhereRaw(
                                PostgresSQLHelper.GetGeoWhereInPolygon_GeneratedColumns(
                                    polygonsearchresult.wktstring,
                                    polygonsearchresult.polygon,
                                    polygonsearchresult.srid,
                                    polygonsearchresult.operation,
                                    polygonsearchresult.reduceprecision
                                    )
                                )
                            )
                    .ApplyRawFilter(rawfilter)
                    .ApplyOrdering_GeneratedColumns(ref seed, geosearchresult, rawsort);                  
                      

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

        private Task<IActionResult> GetOpenDataHubContentSingle(string id, string? language, string[] fields, bool removenullvalues, CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                //Additional Read Filters to Add Check
                AdditionalFiltersToAdd.TryGetValue("Read", out var additionalfilter);

                var query =
                    QueryFactory.Query("opendatahubcontent")
                        .Select("data")
                        .Where("id", id.ToUpper())
                        .When(!String.IsNullOrEmpty(additionalfilter), q => q.FilterAdditionalDataByCondition(additionalfilter))
                        .FilterDataByAccessRoles(UserRolesToFilter);

                var data = await query.FirstOrDefaultAsync<JsonRaw?>();
                
                return data?.TransformRawData(language, fields, filteroutNullValues: removenullvalues, urlGenerator: UrlGenerator, fieldstohide: null);
            });
        }

        #endregion

        #region POST PUT DELETE

        /// <summary>
        /// POST Insert new OpenDataHubContent
        /// </summary>
        /// <param name="opendatahubcontent">OpenDataHubContentLinked Object</param>
        /// <returns>Http Response</returns>                
        [AuthorizeODH(PermissionAction.Create)]
        //[InvalidateCacheOutput(nameof(GetOpenDataHubContents))] only if cached result is served
        [ProducesResponseType(typeof(PGCRUDResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost, Route("OpenDataHubContent")]
        public Task<IActionResult> Post([FromBody] OpenDataHubContentLinked opendatahubcontent)
        {            
            return DoAsyncReturn(async () =>
            {
                //Additional Filters on the Action Create
                AdditionalFiltersToAdd.TryGetValue("Create", out var additionalfilter);

                //GENERATE ID
                opendatahubcontent.Id = Helper.IdGenerator.GenerateIDFromType(opendatahubcontent);

                //Implement Custom CheckMyInsertedLanguages
                //opendatahubcontent.CheckMyInsertedLanguages(new List<string> { "de", "en", "it" });

                if (opendatahubcontent.LicenseInfo == null)
                    opendatahubcontent.LicenseInfo = new LicenseInfo() { ClosedData = false };

                //Removes all spaces 
                opendatahubcontent.TrimStringProperties();

                return await UpsertData<OpenDataHubContentLinked>(opendatahubcontent, new DataInfo("opendatahubcontent", CRUDOperation.Create), new CompareConfig(true, true), new CRUDConstraints(additionalfilter, UserRolesToFilter));
            });
        }

        /// <summary>
        /// PUT Modify existing OpenDataHubContent
        /// </summary>
        /// <param name="id">OpenDataHubContentLinked Id</param>
        /// <param name="opendatahubcontent">OpenDataHubContentLinked Object</param>
        /// <returns>Http Response</returns>        
        [AuthorizeODH(PermissionAction.Update)]
        //[InvalidateCacheOutput(nameof(GetOpenDataHubContents))]
        [ProducesResponseType(typeof(PGCRUDResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPut, Route("OpenDataHubContent/{id}")]
        public Task<IActionResult> Put(string id, [FromBody] OpenDataHubContentLinked opendatahubcontent)
        {            
            return DoAsyncReturn(async () =>
            {
                //Additional Filters on the Action Create
                AdditionalFiltersToAdd.TryGetValue("Update", out var additionalfilter);

                //Check ID uppercase lowercase
                opendatahubcontent.Id = Helper.IdGenerator.CheckIdFromType<OpenDataHubContentLinked>(id);

                //Implement Custom CheckMyInsertedLanguages
                //opendatahubcontent.CheckMyInsertedLanguages(new List<string> { "de", "en", "it" });

                opendatahubcontent.TrimStringProperties();

                return await UpsertData<OpenDataHubContentLinked>(opendatahubcontent, new DataInfo("opendatahubcontent", CRUDOperation.Update), new CompareConfig(true, true), new CRUDConstraints(additionalfilter, UserRolesToFilter));
            });
        }

        /// <summary>
        /// DELETE OpenDataHubContent by Id
        /// </summary>
        /// <param name="id">OpenDataHubContent Id</param>
        /// <returns>Http Response</returns>              
        [AuthorizeODH(PermissionAction.Delete)]
        //[InvalidateCacheOutput(nameof(GetOpenDataHubContents))]
        [ProducesResponseType(typeof(PGCRUDResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpDelete, Route("OpenDataHubContent/{id}")]
        public Task<IActionResult> Delete(string id)
        {
            return DoAsyncReturn(async () =>
            {
                //Additional Filters on the Action Create
                AdditionalFiltersToAdd.TryGetValue("Delete", out var additionalfilter);

                //Check ID uppercase lowercase
                id = Helper.IdGenerator.CheckIdFromType<OpenDataHubContentLinked>(id);

                return await DeleteData<OpenDataHubContentLinked>(id, new DataInfo("opendatahubcontent", CRUDOperation.Delete), new CRUDConstraints(additionalfilter, UserRolesToFilter));
            });
        }

        #endregion
    }
}

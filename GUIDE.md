<!--
SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>

SPDX-License-Identifier: CC0-1.0
-->

# How to add a new DataModel / Controller

4 Steps are needed to define a new Model with a Endpoint and a Database storage 

## DataModel

Define new DataModel.
There are a few constraints. The Datamodel has to implement the Interfaces
- IIdentifiable
- IImportDateassigneable
- IMetaData

in general they "should" also implement this interfaces
- IMappingAware
- IHasLanguage
- ISource
- IActivateable
- IPublishedOn
- ILicenseInfo

means the minimum Set of fields are:
- Id
- _Meta
- FirstImport, LastChange  

optional  

- PublishedOn
- Active
- Mapping
- Source
- HasLanguage
- LicenseInfo

It is always useful to reuse already defined Objects in your DataModel like
- ContactInfos
- Detail
- GpsInfos

Example:

Let's add a new DataModel OpenDataHubContent
We add also Language Support, a Detail Objekt and Gps Infos

```
    public class OpenDataHubContent : IIdentifiable, IMetaData, IImportDateassigneable, ISource, IActivateable, IMappingAware, ILicenseInfo, IPublishedOn, IGPSInfoAware, IHasLanguage, IDetailInfosAware
    {
        public string Id { get; set; }
        public Metadata _Meta { get; set; }
        public DateTime? FirstImport { get; set; }
        public DateTime? LastChange { get; set; }
        public string Source { get; set; }
        public bool Active { get; set; }
        public IDictionary<string, IDictionary<string, string>> Mapping { get; set; }
        public LicenseInfo LicenseInfo { get; set; }
        public ICollection<string>? PublishedOn { get; set; }
        public ICollection<GpsInfo> GpsInfo { get; set; }        
        public ICollection<string>? HasLanguage { get; set; }
        public IDictionary<string, Detail> Detail { get; set; }
    }
```
For all fields which contains "Links" to have a browseable api we create the same Object followed by "Linked"
In Our Case we just want to add a Self Link to the DataModel  
We also have to add a generated field GpsPoints needed to have the full Geosearch functionality

```
    public class OpenDataHubContentLinked : OpenDataHubContent
    {
        [SwaggerSchema(Description = "generated field", ReadOnly = true)]
        public string? Self
        {
            get
            {
                return this.Id != null ? "Example/" + Uri.EscapeDataString(this.Id) : null;
            }
        }

        [SwaggerSchema(Description = "generated field", ReadOnly = true)]
        public IDictionary<string, GpsInfo> GpsPoints
        {
            get { return this.GpsInfo.ToGpsPointsDictionary(); }
        }
    }
```


## Controller

Define a Controller which inherits from OdhController
implement  
GET LIST
GET DETAIL
POST
PUT
DELETE
Methods

Example:

Let's add the Controller for OpenDataHubContent DataModel

The Controller supports Getting a List of OpenDataHubContent Features are
Pagination 
Random Sorting
Searchfilter
Pass an Id List
Get the result only in the desired Language
Get the result only if available in the desired Language
GeoFilter functionality
Polygon functionality
Fields functionality
Rawfilter
Rawsort
Remove Nullvalues

```
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
                opendatahubcontent.Id = Helper.IdGenerator.CheckIdFromType<ExampleLinked>(id);

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

```

## Database Table

Let's add a Table in our Postgres with the name `opendatahubcontent`
We need generated Columns for the fields/sections  
LicenseInfo  
Active  
HasLanguage  
Lastchange  
Latitude, Longitude  
PublishedOn  
Source  
Reduced  
Generated Id, Access_Role, Position are also needed_

We generated also some index to speed up Geosearch Queries and Searchfilter Queries

```SQL
CREATE TABLE opendatahubcontent (
	id varchar(200) NOT NULL,
	"data" jsonb NULL,
	gen_licenseinfo_closeddata bool GENERATED ALWAYS AS ((data #> '{LicenseInfo,ClosedData}'::text[])::boolean) STORED NULL,
	gen_active bool GENERATED ALWAYS AS ((data #> '{Active}'::text[])::boolean) STORED NULL,
	gen_haslanguage _text GENERATED ALWAYS AS (json_array_to_pg_array(data #> '{HasLanguage}'::text[])) STORED NULL,
	gen_lastchange timestamp GENERATED ALWAYS AS (text2ts(data #>> '{LastChange}'::text[])) STORED NULL,
	gen_latitude float8 GENERATED ALWAYS AS ((data #> '{GpsPoints,position,Latitude}'::text[])::double precision) STORED NULL,
	gen_longitude float8 GENERATED ALWAYS AS ((data #> '{GpsPoints,position,Longitude}'::text[])::double precision) STORED NULL,
	gen_publishedon _text GENERATED ALWAYS AS (json_array_to_pg_array(data #> '{PublishedOn}'::text[])) STORED NULL,
	gen_source text GENERATED ALWAYS AS (data #>> '{_Meta,Source}'::text[]) STORED NULL,
	gen_reduced bool GENERATED ALWAYS AS ((data #> '{_Meta,Reduced}'::text[])::boolean) STORED NULL,	
	gen_access_role _text GENERATED ALWAYS AS (calculate_access_array(data #>> '{_Meta,Source}'::text[], (data #> '{LicenseInfo,ClosedData}'::text[])::boolean, (data #> '{_Meta,Reduced}'::text[])::boolean)) STORED NULL,
	gen_position public.geometry GENERATED ALWAYS AS (st_setsrid(st_makepoint((data #> '{GpsPoints,position,Longitude}'::text[])::double precision, (data #> '{GpsPoints,position,Latitude}'::text[])::double precision), 4326)) STORED NULL,
	gen_id text GENERATED ALWAYS AS (data #>> '{Id}'::text[]) STORED NULL,
    rawdataid int4 NULL,
	CONSTRAINT opendatabhubcontent_pkey PRIMARY KEY (id)
);
CREATE INDEX opendatahubcontent_detail_de_title_trgm_idx ON public.opendatahubcontent USING gin (((data #>> '{Detail,de,Title}'::text[])) gin_trgm_ops);
CREATE INDEX opendatahubcontent_detail_en_title_trgm_idx ON public.opendatahubcontent USING gin (((data #>> '{Detail,en,Title}'::text[])) gin_trgm_ops);
CREATE INDEX opendatahubcontent_detail_it_title_trgm_idx ON public.opendatahubcontent USING gin (((data #>> '{Detail,it,Title}'::text[])) gin_trgm_ops);
CREATE INDEX opendatahubcontent_earthix ON public.opendatahubcontent USING gist (ll_to_earth(((((data -> 'GpsPoints'::text) -> 'position'::text) ->> 'Latitude'::text))::double precision, ((((data -> 'GpsPoints'::text) -> 'position'::text) ->> 'Longitude'::text))::double precision));

```

## Helpers

### MetaInfo Helper

If the `_Meta.Type` Value `opendatahubcontent` is right for us we don't have to make adaptions here.
If Classname.tolower() / tablename / typename are matching nothing to modify here.

### ODHTypeHelper

TranslateTypeToSearchField  
ConvertJsonRawToObject  
TranslateTable2Type  
TranslateTypeString2Type  


### LicenseInfoHelper

Add Here logic for assigning a license

### HasLanguageHelper

Add here logic if custom Language Checks should be made

## KeyCloak Authorization Rules

Add Endpoint to Resources!
Add Policy
Add the Resource in the Permission

## Tryout

Mark ´ContentApiCore´ as startup project.  
Add the Postgres Connectionstring
"ConnectionStrings": {
    "PgConnection": "Server=SERVERURL;Port=5432;User ID=USERNAME;Password=PASSWORD;Database=DBNAME"
  },


STart the application
Add a dataset with swagger

```javascript
{
  "Source": "testsource",
  "Active": true,
  "Mapping": {
    "test": {
      "key": "5",
    },
  },
  "LicenseInfo": {
    "License": "CC0",
    "LicenseHolder": "string",
    "Author": ""
  },
  "PublishedOn": [
    "test.net"
  ],
  "GpsInfo": [
    {
      "Gpstype": "position",
      "Latitude": 46.48985918267802,
      "Longitude": 11.311126990304523,
      "Altitude": 0,
      "AltitudeUnitofMeasure": "m"
    }
  ],
  "HasLanguage": [
    "de"
  ],
  "Detail": {
    "de": {
      "Header": "",
      "SubHeader": "",
      "IntroText": "",
      "BaseText": "",
      "Title": "TEST",
      "AdditionalText": "",
      "MetaTitle": "",
      "MetaDesc": "",
      "GetThereText": "",
      "Language": "",      
    },
    }
  }
}
```
// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using SqlKata;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Helper
{
    public static class PostgresSQLWhereBuilder
    {
        private static readonly string[] _languagesToSearchFor =
            new[] { "de", "it", "en", "nl", "cs", "pl", "fr", "pl" };




        /// <summary>
        /// Provide title fields as JsonPath
        /// </summary>
        /// <param name="language">
        /// If provided only the fields with the
        /// specified language get returned
        /// </param>
        public static string[] TitleFieldsToSearchFor(string? language) =>
            _languagesToSearchFor.Where(lang =>
                language != null ? lang == language : true
            ).Select(lang =>
                $"Detail.{lang}.Title"
            ).ToArray();

        public static string[] TitleFieldsToSearchFor(string? language, IReadOnlyCollection<string>? haslanguage) =>
            _languagesToSearchFor.Where(lang =>
                language != null ? lang == language : true &&
                haslanguage != null ? haslanguage.Contains(lang) : true
            ).Select(lang =>
                $"Detail.{lang}.Title"
            ).ToArray();

        public static string[] AccoTitleFieldsToSearchFor(string? language) =>
            _languagesToSearchFor.Where(lang =>
                language != null ? lang == language : true
            ).Select(lang =>
                $"AccoDetail.{lang}.Name"
            ).ToArray();

        public static string[] AccoTitleFieldsToSearchFor(string? language, IReadOnlyCollection<string>? haslanguage) =>
            _languagesToSearchFor.Where(lang =>
                language != null ? lang == language : true &&
                haslanguage != null ? haslanguage.Contains(lang) : true
            ).Select(lang =>
                $"AccoDetail.{lang}.Name"
            ).ToArray();

        public static string[] AccoRoomNameFieldsToSearchFor(string? language) =>
            _languagesToSearchFor.Where(lang =>
                language != null ? lang == language : true
            ).Select(lang =>
                $"AccoRoomDetail.{lang}.Name"
            ).ToArray();

        public static string[] AccoRoomNameFieldsToSearchFor(string? language, IReadOnlyCollection<string>? haslanguage) =>
        _languagesToSearchFor.Where(lang =>
            language != null ? lang == language : true &&
                haslanguage != null ? haslanguage.Contains(lang) : true
        ).Select(lang =>
            $"AccoRoomDetail.{lang}.Name"
        ).ToArray();

        public static string[] EventShortTitleFieldsToSearchFor(string? language) =>
            _languagesToSearchFor.Where(lang =>
                language != null ? lang == language : true
            ).Select(lang =>
                $"EventTitle.{lang}"
            ).ToArray();

        public static string[] EventShortTitleFieldsToSearchFor(string? language, IReadOnlyCollection<string>? haslanguage) =>
            _languagesToSearchFor.Where(lang =>
                language != null ? lang == language : true &&
                haslanguage != null ? haslanguage.Contains(lang) : true
            ).Select(lang =>
                $"EventTitle.{lang}"
            ).ToArray();

        public static string[] TourismMetaDataTitleFieldsToSearchFor(string? language) =>
            _languagesToSearchFor.Where(lang =>
                language != null ? lang == language : true
            ).Select(lang =>
                $"ApiDescription.{lang}"
            ).ToArray();

        //TODO TRANSFORM LANGUAGE to deu,eng,ita
        //public static string[] VenueTitleFieldsToSearchFor(string? language) =>
        //   _languagesToSearchFor.Where(lang =>
        //       language != null ? lang == language : true
        //   ).Select(lang =>
        //       $"attributes.name.{TransformLanguagetoDDStandard(lang)}"
        //   ).ToArray();     

        //public static string TransformLanguagetoDDStandard(string language) => language switch
        //{
        //    "de" =>  "deu",
        //    "it" =>  "ita",
        //    "en" =>  "eng",
        //    _ => language
        //};


        //TODO search name example
        //name: {
        //    deu: "Akademie deutsch-italienischer Studien",
        //    ita: "Accademia di studi italo-tedeschi",
        //    eng: "Academy of German-Italian Studies"
        //    },
        //private static string[] VenueTitleFieldsToSearchFor(string? language) =>
        // _languagesToSearchFor.Where(lang =>
        //     language != null ? lang == language : true
        // ).Select(lang =>
        //     $"odhdata.Detail.{lang}.Name"
        // ).ToArray();

        //Public for use in Controllers directly
        public static string[] TypeDescFieldsToSearchFor(string? language) =>
            _languagesToSearchFor.Where(lang =>
                language != null ? lang == language : true
            ).Select(lang =>
                $"TypeDesc.{lang}"
            ).ToArray();

        public static string[] TagNameFieldsToSearchFor(string? language) =>
            _languagesToSearchFor.Where(lang =>
                language != null ? lang == language : true
            ).Select(lang =>
                $"TagName.{lang}"
            ).ToArray();

        public static string[] NameFieldsToSearchFor(string? language) =>
            _languagesToSearchFor.Where(lang =>
                language != null ? lang == language : true
            ).Select(lang =>
                $"Name.{lang}"
            ).ToArray();

        public static string[] WeatherHistoryFieldsToSearchFor(string? language) =>
            _languagesToSearchFor.Where(lang =>
                language != null ? lang == language : true
            ).Select(lang =>
                $"Weather.{lang}.evolutiontitle"
            ).ToArray();

        public static void CheckPassedLanguage(ref string language, IEnumerable<string> availablelanguages)
        {
            language = language.ToLower();

            if (!availablelanguages.Contains(language))
                throw new Exception("passed language not available or passed incorrect string");
        }

        //Return where and Parameters
        [System.Diagnostics.Conditional("TRACE")]
        private static void LogMethodInfo(System.Reflection.MethodBase m, params object?[] parameters)
        {
            var parameterInfo =
                m.GetParameters()
                    .Zip(parameters)
                    .Select((x, _) => (x.First.Name, x.Second));
            Serilog.Log.Debug("{method}({@parameters})", m.Name, parameterInfo);
        }

          
        //Return Where and Parameters for Example
        public static Query ExampleWhereExpression(
            this Query query, IReadOnlyCollection<string> languagelist,
            IReadOnlyCollection<string> idlist, IReadOnlyCollection<string> typelist, bool? activefilter, IReadOnlyCollection<string> sourcelist,
            IReadOnlyCollection<string> publishedonlist, string? searchfilter, string? language, string? lastchange, string? additionalfilter,
            IEnumerable<string> userroles)
        {
            LogMethodInfo(
                System.Reflection.MethodBase.GetCurrentMethod()!,
                 "<query>", // not interested in query
                idlist, typelist, languagelist, activefilter, sourcelist, publishedonlist, searchfilter, language, lastchange
            );

            return query
                .IdUpperFilter(idlist)
                .When(typelist.Count > 0, q => q.TypeFilterOr_GeneratedColumn(typelist))
                .When(languagelist.Count > 0, q => q.HasLanguageFilterAnd_GeneratedColumn(languagelist)) //.HasLanguageFilter(languagelist)
                .ActiveFilter_GeneratedColumn(activefilter)         //OK GENERATED COLUMNS //.ActiveFilter(activefilter)
                .PublishedOnFilter_GeneratedColumn(publishedonlist)   //.PublishedOnFilter(publishedonlist)
                //.SyncSourceInterfaceFilter_GeneratedColumn(sourcelist)
                .SourceFilter_GeneratedColumn(sourcelist)
                .LastChangedFilter_GeneratedColumn(lastchange)
                .SearchFilter(TitleFieldsToSearchFor(language, languagelist), searchfilter)
                .When(!String.IsNullOrEmpty(additionalfilter), q => q.FilterAdditionalDataByCondition(additionalfilter))
                .FilterDataByAccessRoles(userroles);
        }

      
        //Return Where and Parameters for OdhTag and Tag
        public static Query PublishersWhereExpression(
            this Query query, IReadOnlyCollection<string> languagelist,
            IReadOnlyCollection<string> idlist, IReadOnlyCollection<string> sourcelist,
            string? searchfilter, string? language,
            string? additionalfilter,
            IEnumerable<string> userroles)
        {
            LogMethodInfo(
                System.Reflection.MethodBase.GetCurrentMethod()!,
                 "<query>", // not interested in query
                searchfilter, language, sourcelist
            );

            return query
                .SearchFilter(NameFieldsToSearchFor(language), searchfilter)
                .SourceFilter_GeneratedColumn(sourcelist)
                .When(idlist != null && idlist.Count > 0, q => query.WhereIn("id", idlist))
                .When(!String.IsNullOrEmpty(additionalfilter), q => q.FilterAdditionalDataByCondition(additionalfilter))
                .FilterDataByAccessRoles(userroles);
        }

        public static Query SourcesWhereExpression(
            this Query query, IReadOnlyCollection<string> languagelist,
            IReadOnlyCollection<string> idlist, IReadOnlyCollection<string> typeslist,
            string? searchfilter, string? language,
            string? additionalfilter,
            IEnumerable<string> userroles)
        {
            LogMethodInfo(
                System.Reflection.MethodBase.GetCurrentMethod()!,
                 "<query>", // not interested in query
                searchfilter, language, idlist
            );

            return query
                .SearchFilter(NameFieldsToSearchFor(language), searchfilter)
                .When(idlist != null && idlist.Count > 0, q => query.WhereIn("id", idlist))
                .When(typeslist != null && typeslist.Count > 0, q => query.SourceTypeFilter(typeslist))
                .When(!String.IsNullOrEmpty(additionalfilter), q => q.FilterAdditionalDataByCondition(additionalfilter))
                .FilterDataByAccessRoles(userroles);
        }

        public static Query PushResultWhereExpression(
            this Query query, 
            IReadOnlyCollection<string> idlist, IReadOnlyCollection<string> publisherlist,
            DateTime? begin, DateTime? end,
            IReadOnlyCollection<string> objectidlist, IReadOnlyCollection<string> objecttypelist,
            string? additionalfilter)
        {
            LogMethodInfo(
                System.Reflection.MethodBase.GetCurrentMethod()!,
                 "<query>", // not interested in query
                publisherlist, begin, end, idlist
            );

            return query
                .When(idlist != null && idlist.Count > 0, q => query.WhereIn("id", idlist))
                .When(publisherlist != null && publisherlist.Count > 0, q => query.WhereIn("gen_publisher", publisherlist))
                .When(objectidlist != null && objectidlist.Count > 0, q => query.WhereIn("gen_objectid", objectidlist))
                .When(objecttypelist != null && objecttypelist.Count > 0, q => query.WhereIn("gen_objecttype", objecttypelist))
                .LastChangedFilter_GeneratedColumn(begin, end)
                .When(!String.IsNullOrEmpty(additionalfilter), q => q.FilterAdditionalDataByCondition(additionalfilter));
                
        }


        //Return Where and Parameters for Rawdata
        public static Query RawdataWhereExpression(
            this Query query, IReadOnlyCollection<string> idlist, IReadOnlyCollection<string> sourceidlist, 
            IReadOnlyCollection<string> typelist, IReadOnlyCollection<string> sourcelist,
            string? additionalfilter,
            IEnumerable<string> userroles)
        {
            LogMethodInfo(
                System.Reflection.MethodBase.GetCurrentMethod()!,
                 "<query>", // not interested in query
                idlist, sourceidlist, typelist,
                sourcelist
            );

            return query
                .When(typelist != null, q => query.WhereIn("type", typelist))
                .When(sourcelist != null, q => query.WhereIn("datasource", sourcelist))
                 .When(idlist != null, q => query.WhereIn("id", idlist))
                 //.When(latest, )
                 //.When(filterClosedData, q => q.FilterClosedData_Raw());
                 .When(!String.IsNullOrEmpty(additionalfilter), q => q.FilterAdditionalDataByCondition(additionalfilter))
                 .FilterDataByAccessRoles(userroles);
            //TODO future opendata rules on 
            //.Anonymous_Logged_UserRule_GeneratedColumn(filterClosedData, !reducedData);
        }

    }
}

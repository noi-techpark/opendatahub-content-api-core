// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using ContentApiModels;
using DataModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    public class ODHTypeHelper
    {
        public static IEnumerable<string> GetAllTypeStrings()
        {
            return new List<string>()
            {
                "example",            
                "publisher",
                "source"               
            };
        }

        #region TypeObject2TypeStringANDPGTable

        /// <summary>
        /// Translates a ODH Type Object to the Type (Metadata) as String
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="odhtype"></param>
        /// <returns></returns>
        public static string TranslateType2TypeString<T>(T odhtype)
        {
            return odhtype switch
            {
                Example or ExampleLinked => "example",
                ExampleType => "exampletype",
                Publisher or PublisherLinked => "publisher",
                Source or SourceLinked => "source",
                _ => nameof(T).ToLower().Replace("linked","")
            };
        }

        /// <summary>
        /// Translates ODH Type Object to PG Table Name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="odhtype"></param>
        /// <returns></returns>
        public static string TranslateType2Table<T>(T odhtype)
        {
            return odhtype switch
            {
                Example or ExampleLinked => "examples",
                ExampleType => "exampletypes",
                Publisher or PublisherLinked => "publishers",
                Source or SourceLinked => "sources",
                _ => nameof(T).ToLower().Replace("linked", "")
            };
        }

        #endregion

        #region TypeString2TypeObjectANDPGTable

        /// <summary>
        /// Translates Type (Metadata) as String to PG Table Name
        /// </summary>
        /// <param name="odhtype"></param>
        /// <returns></returns>
        public static string TranslateTypeString2Table(string odhtype)
        {
            return odhtype switch
            {
                "example" => "examples",
                "exampletype" => "exampletypes",              
                "publisher" => "publishers",                
                _ => odhtype
            };
        }

        /// <summary>
        /// Translates Type (Metadata) as String to Type Object
        /// </summary>
        /// <param name="odhtype"></param>
        /// <returns></returns>
        public static Type TranslateTypeString2Type(string odhtype)
        {
            return odhtype switch
            {
                "example" => typeof(ExampleLinked),
                "exampletype" => typeof(ExampleType),
                "publisher" => typeof(PublisherLinked),
                "source" => typeof(SourceLinked),               
                _ => throw new Exception("not known odh type")
            };
        }

        #endregion

        #region PGTable2TypeObjectANDString

        /// <summary>
        /// Translates PG Table Name to Type (Metadata) as String
        /// </summary>
        /// <param name="odhtype"></param>
        /// <returns></returns>
        public static string TranslateTable2TypeString(string odhtype)
        {
            return odhtype switch
            {
                "examples" => "example",
                "exampletypes" => "exampletype",             
                "publishers" => "publisher",
                "sources" => "source",               
                _ => odhtype
            };
        }

        /// <summary>
        /// Translates PG Table Name to Type Object
        /// </summary>
        /// <param name="odhtype"></param>
        /// <returns></returns>
        public static Type TranslateTable2Type(string odhtype)
        {
            return odhtype switch
            {
                "examples" => typeof(ExampleLinked),
                "exampletypes" => typeof(ExampleType),             
                "publishers" => typeof(PublisherLinked),
                "sources" => typeof(SourceLinked),
                _ => throw new Exception("not known table name")
            };
        }

        #endregion

        #region Type2MetaGeneratorFunction

        //TODO Migrate from Metagenerator

        #endregion

        #region TypeIdConverter

        //TODO insert in IdGenerator
        public static string ConvertIdbyTypeString(string odhtype, string id)
        {
            return odhtype switch
            {
                "example" => id.ToUpper(),
                "exampletype" => id.ToUpper(),              
                "publisher" => id.ToLower(),
                "source" => id.ToLower(),
                _ => id
            };
        }

        #endregion

        #region JsonRaw2Type

        public static IIdentifiable ConvertJsonRawToObject(string odhtype, JsonRaw raw)
        {
            return odhtype switch
            {
                "example" => JsonConvert.DeserializeObject<ExampleLinked>(raw.Value)!,
                "exampletype" => JsonConvert.DeserializeObject<ExampleType>(raw.Value)!,            
                "publisher" => JsonConvert.DeserializeObject<PublisherLinked>(raw.Value)!,
                "source" => JsonConvert.DeserializeObject<SourceLinked>(raw.Value)!,               
                _ => throw new Exception("not known odh type")
            };
        }

        #endregion

        #region Search

        /// <summary>
        /// Gets all ODH Types to search on
        /// </summary>
        /// <param name="getall"></param>
        /// <returns></returns>
        public static string[] GetAllSearchableODHTypes(bool getall)
        {
            var odhtypes = new List<string>();

            odhtypes.Add("example");

            if (getall)
            {
                odhtypes.Add("publisher");
                odhtypes.Add("source");                
                odhtypes.Add("exampletype");
            }

            return odhtypes.ToArray();
        }

        public static Func<string, string[]> TranslateTypeToSearchField(string odhtype)
        {
            return odhtype switch
            {
                "accommodation" => PostgresSQLWhereBuilder.AccoTitleFieldsToSearchFor,
                "accommodationroom" => PostgresSQLWhereBuilder.AccoRoomNameFieldsToSearchFor,
                "ltsactivity" or "ltspoi" or "ltsgastronomy" or "event" or "odhactivitypoi" or "metaregion" or "region" or "tourismassociation" or "municipality"
                or "district" or "skiarea" or "skiregion" or "article" or "experiencearea" or "webcam" or "venue"
                => PostgresSQLWhereBuilder.TitleFieldsToSearchFor,
                //"measuringpoint" => PostgresSQLWhereBuilder.,
//                "webcam" => PostgresSQLWhereBuilder.WebcamnameFieldsToSearchFor,
                //"venue" => PostgresSQLWhereBuilder.TitleFieldsToSearchFor,
                //"eventshort" => "eventeuracnoi",           
                //"area" => "areas",
                //"wineaward" => "wines",
                _ => throw new Exception("not known odh type")
            };
        }

        public static string TranslateTypeToEndPoint(string odhtype)
        {
            return odhtype switch
            {
                "accommodation" => "Accommodation",
                "odhactivitypoi" => "ODHActivityPoi",
                "event" => "Event",
                "region" => "Region",
                "skiarea" => "SkiArea",
                "tourismassociation" => "TourismAssociation",
                "webcam" => "WebcamInfo",
                "venue" => "Venue",
                "accommodationroom" => "AccommodationRoom",
                "package" => "Package",
                "ltsactivity" => "Activity",
                "ltspoi" => "Poi",
                "ltsgastronomy" => "Gastronomy",
                "measuringpoint" => "Weather/Measuringpoint",
                "article" => "Article",
                "municipality" => "Municipality",
                "district" => "District",
                "skiregion" => "SkiRegion",
                "eventshort" => "EventShort",
                "experiencearea" => "ExperienceArea",
                "metaregion" => "MetaRegion",
                "area" => "Area",
                "wineaward" => "Wine",
                "odhmetadata" => "MetaData",
                "odhtag" => "ODHTag",
                "tag" => "Tag",
                "publisher" => "Publisher",
                "source" => "Source",
                "weatherhistory" => "WeatherHistory",
                "weatherdistrict" => "Weather/District",
                "weatherforecast" => "Weather/Forecast",
                "weatherrealtime" => "Weather/Realtime",
                "snowreport" => "Weather/SnowReport",
                "weather" => "Weather",

                _ => FirstLetterToUpper(odhtype)
            };
        }


        #endregion

        public static string TranslateTypeToTitleField(string odhtype, string language)
        {
            return odhtype switch
            {
                "example" => $"Detail.{language}.Title",
                "accommodation" => $"AccoDetail.{language}.Name",
                "accommodationroom" => $"AccoRoomDetail.{language}.Name",
                "ltsactivity" or "ltspoi" or "ltsgastronomy" or "event" or "odhactivitypoi" or "metaregion" or "region" or "tourismassociation" or "municipality"
                or "district" or "skiarea" or "skiregion" or "article" or "experiencearea" or "venue"
                => $"Detail.{language}.Title",
                "measuringpoint" => $"Shortname",
                "webcam" => $"Webcamname.{language}",                
                //"eventshort" => "eventeuracnoi",           
                //"area" => "areas",
                //"wineaward" => "wines",
                _ => throw new Exception("not known odh type")
            };
        }

        public static string TranslateTypeToBaseTextField(string odhtype, string language)
        {
            return odhtype switch
            {
                "example" => $"Detail.{language}.BaseText",
                "accommodation" => $"AccoDetail.{language}.Longdesc",
                "accommodationroom" => $"AccoRoomDetail.{language}.Longdesc",
                "ltsactivity" or "ltspoi" or "ltsgastronomy" or "event" or "odhactivitypoi" or "metaregion" or "region" or "tourismassociation" or "municipality"
                or "district" or "skiarea" or "skiregion" or "article" or "experiencearea"
                => $"Detail.{language}.BaseText",
                "measuringpoint" => "notextfield",
                "webcam" => "notextfield",
                "venue" => "notextfield",
                _ => throw new Exception("not known odh type")
            };
        }

        static string FirstLetterToUpper(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return char.ToUpper(input[0]) + input.Substring(1);
        }
    }
}

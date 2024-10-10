// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using JsonLDTransformer.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Schema.NET;
using System.Xml;
using HtmlAgilityPack;
using DataModel;
using Newtonsoft.Json;
using ContentApiModels;

namespace JsonLDTransformer
{
    public class TransformToSchemaNet
    {
        public static List<object> TransformDataToSchemaNet<T>(T data, string currentroute, string type,  string language, object parentobject = null, string idtoshow = "", string urltoshow = "", string imageurltoshow = "", bool showid = true)
        {
            var objectlist = new List<object>();

            switch (type)
            {
                case "example":
                    objectlist.Add(TransformExampleToLD((Example)(object)data, currentroute, language, idtoshow, urltoshow, imageurltoshow, showid));
                    break;             
            }


            return objectlist;
        }       

        #region Place

        private static Schema.NET.Place TransformExampleToLD(Example placetotrasform, string currentroute, string language, string passedid, string passedurl, string passedimage, bool showid)
        {
            string fallbacklanguage = "en";

            Schema.NET.Place place = new Schema.NET.Place();

            if (showid)
            {
                if (String.IsNullOrEmpty(passedid))
                    place.Id = new Uri(currentroute);
                else
                    place.Id = new Uri(passedid);
            }

            place.Description = placetotrasform.Detail.ContainsKey(language) ? placetotrasform.Detail[language].BaseText : placetotrasform.Detail.ContainsKey(fallbacklanguage) ? placetotrasform.Detail[fallbacklanguage].BaseText : "";
            place.Name = placetotrasform.Detail.ContainsKey(language) ? placetotrasform.Detail[language].Title : placetotrasform.Detail.ContainsKey(fallbacklanguage) ? placetotrasform.Detail[fallbacklanguage].Title : "";

            place.FaxNumber = placetotrasform.ContactInfos.ContainsKey(language) ? placetotrasform.ContactInfos[language].Faxnumber : placetotrasform.ContactInfos.ContainsKey(fallbacklanguage) ? placetotrasform.ContactInfos[fallbacklanguage].Faxnumber : "";
            place.Telephone = placetotrasform.ContactInfos.ContainsKey(language) ? placetotrasform.ContactInfos[language].Phonenumber : placetotrasform.ContactInfos.ContainsKey(fallbacklanguage) ? placetotrasform.ContactInfos[fallbacklanguage].Phonenumber : "";

            //Image Overwrite
            if (String.IsNullOrEmpty(passedimage))
            {
                if (placetotrasform.ImageGallery != null)
                    if (placetotrasform.ImageGallery.Count > 0)
                        if (!String.IsNullOrEmpty(placetotrasform.ImageGallery.FirstOrDefault().ImageUrl))
                            place.Image = new Uri(placetotrasform.ImageGallery.FirstOrDefault().ImageUrl);
            }
            else
                place.Image = new Uri(passedimage);

            // URL OVERWRITE
            if (String.IsNullOrEmpty(passedurl) && String.IsNullOrEmpty(passedid))
            {
                string url = placetotrasform.ContactInfos.ContainsKey(language) ? placetotrasform.ContactInfos[language].Url : placetotrasform.ContactInfos.ContainsKey(fallbacklanguage) ? placetotrasform.ContactInfos[fallbacklanguage].Url : "";
                if (CheckURLValid(url))
                    place.Url = new Uri(url);
            }
            else if (!String.IsNullOrEmpty(passedurl))
            {
                if (CheckURLValid(passedurl))
                    place.Url = new Uri(passedurl);
            }
            else if (!String.IsNullOrEmpty(passedid))
            {
                if (CheckURLValid(passedid))
                    place.Url = new Uri(passedid);
            }

            string logo = placetotrasform.ContactInfos.ContainsKey(language) ? placetotrasform.ContactInfos[language].LogoUrl : placetotrasform.ContactInfos.ContainsKey(fallbacklanguage) ? placetotrasform.ContactInfos[fallbacklanguage].LogoUrl : "";
            if (CheckURLValid(logo))
                place.Logo = new Uri(logo);


            PostalAddress myaddress = new PostalAddress();
            //myaddress.Type = "http://schema.org/PostalAddress";
            myaddress.StreetAddress = placetotrasform.ContactInfos.ContainsKey(language) ? placetotrasform.ContactInfos[language].Address : placetotrasform.ContactInfos.ContainsKey(fallbacklanguage) ? placetotrasform.ContactInfos[fallbacklanguage].Address : "";
            myaddress.PostalCode = placetotrasform.ContactInfos.ContainsKey(language) ? placetotrasform.ContactInfos[language].ZipCode : placetotrasform.ContactInfos.ContainsKey(fallbacklanguage) ? placetotrasform.ContactInfos[fallbacklanguage].ZipCode : "";
            myaddress.AddressLocality = placetotrasform.ContactInfos.ContainsKey(language) ? placetotrasform.ContactInfos[language].City : placetotrasform.ContactInfos.ContainsKey(fallbacklanguage) ? placetotrasform.ContactInfos[fallbacklanguage].City : "";
            myaddress.AddressRegion = placetotrasform.ContactInfos.ContainsKey(language) ? placetotrasform.ContactInfos[language].Region : placetotrasform.ContactInfos.ContainsKey(fallbacklanguage) ? placetotrasform.ContactInfos[fallbacklanguage].Region : "";
            myaddress.AddressCountry = placetotrasform.ContactInfos.ContainsKey(language) ? placetotrasform.ContactInfos[language].CountryName : placetotrasform.ContactInfos.ContainsKey(fallbacklanguage) ? placetotrasform.ContactInfos[fallbacklanguage].CountryName : "";
            myaddress.Telephone = placetotrasform.ContactInfos.ContainsKey(language) ? placetotrasform.ContactInfos[language].Phonenumber : placetotrasform.ContactInfos.ContainsKey(fallbacklanguage) ? placetotrasform.ContactInfos[fallbacklanguage].Phonenumber : "";

            string adressurl = placetotrasform.ContactInfos.ContainsKey(language) ? placetotrasform.ContactInfos[language].Url : placetotrasform.ContactInfos.ContainsKey(fallbacklanguage) ? placetotrasform.ContactInfos[fallbacklanguage].Url : "";
            if (CheckURLValid(adressurl))
                myaddress.Url = new Uri(adressurl);

            myaddress.Email = placetotrasform.ContactInfos.ContainsKey(language) ? placetotrasform.ContactInfos[language].Email : placetotrasform.ContactInfos.ContainsKey(fallbacklanguage) ? placetotrasform.ContactInfos[fallbacklanguage].Email : "";
            myaddress.FaxNumber = placetotrasform.ContactInfos.ContainsKey(language) ? placetotrasform.ContactInfos[language].Faxnumber : placetotrasform.ContactInfos.ContainsKey(fallbacklanguage) ? placetotrasform.ContactInfos[fallbacklanguage].Faxnumber : "";
            myaddress.AlternateName = placetotrasform.ContactInfos.ContainsKey(language) ? placetotrasform.ContactInfos[language].CompanyName : placetotrasform.ContactInfos.ContainsKey(fallbacklanguage) ? placetotrasform.ContactInfos[fallbacklanguage].CompanyName : "";
            myaddress.Name = placetotrasform.Detail.ContainsKey(language) ? placetotrasform.Detail[language].Title : placetotrasform.Detail.ContainsKey(fallbacklanguage) ? placetotrasform.Detail[fallbacklanguage].Title : "";


            place.Address = myaddress;

            GeoCoordinates mygeo = new GeoCoordinates();
            //mygeo.Type = "http://schema.org/GeoCoordinates";
            mygeo.Latitude = placetotrasform.GpsPoints.ContainsKey("position") ? placetotrasform.GpsPoints["position"].Latitude : 0;
            mygeo.Longitude = placetotrasform.GpsPoints.ContainsKey("position") ? placetotrasform.GpsPoints["position"].Longitude : 0;

            place.Geo = mygeo;

            return place;
        }
    
        #endregion

  
        public static bool CheckURLValid(string source)
        {
            Uri uriResult;
            return Uri.TryCreate(source, UriKind.Absolute, out uriResult) && uriResult.Scheme == Uri.UriSchemeHttp;
        }
    }

    public class RecipeDetails
    {
        public TimeSpan cookTime { get; set; }
        public string recipeYield { get; set; }
        public List<string> recipeIngredients { get; set; }
        public string recipeInstructions { get; set; }

        //NEW
        //"kalorien" --> nutrition (Nutritioninformation.... calories)
        public NutritionInformation recipeNutritionInfo { get; set; }
        //"vorbereitungszeit" --> prepTime (Duration)
        public TimeSpan prepTime { get; set; }
        //"keywords" --> keywords (TEXT)
        public string recipeKeywords { get; set; }
        //"kategorie" --> recipecategory  (TEXT)
        public string recipeCategory { get; set; }

        //"artkueche" --> recipecousine  (TEXT)
        public string recipeCuisinetype { get; set; }


        //"author" --> author (Organization or Person)
        public Organization author { get; set; }

        //"bewertung" --> aggregaterating (Aggregaterating)
        public AggregateRating aggregateRating { get; set; }


    }

    public class SpecialAnnouncement : Schema.NET.CreativeWork
    {
        [System.Runtime.Serialization.DataMemberAttribute(Name = "@type", Order = 1)]
        public new string Type { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute(Name = "datePosted")]
        [JsonConverter(typeof(DateTimeToIso8601DateValuesJsonConverter))]
        public Values<int?, DateTime?, DateTimeOffset?> DatePosted { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute(Name = "announcementLocation")]
        [JsonConverter(typeof(ValuesJsonConverter))]
        public Values<ICivicStructure, ILocalBusiness> AnnouncementLocation { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute(Name = "category")]
        [JsonConverter(typeof(ValuesJsonConverter))]
        public Values<IPhysicalActivity, string, IThing, Uri> Category { get; set; }

        //Webcontent is missing using text

        [System.Runtime.Serialization.DataMemberAttribute(Name = "diseasePreventionInfo")]
        [JsonConverter(typeof(ValuesJsonConverter))]
        public Values<string, Uri> DiseasePreventionInfo { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute(Name = "diseaseSpreadStatistics")]
        [JsonConverter(typeof(ValuesJsonConverter))]
        public Values<string, Uri> DiseaseSpreadStatistics { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute(Name = "gettingTestedInfo")]
        [JsonConverter(typeof(ValuesJsonConverter))]
        public Values<string, Uri> GettingTestedInfo { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute(Name = "governmentBenefitsInfo")]
        [JsonConverter(typeof(ValuesJsonConverter))]
        public OneOrMany<IGovernmentService> GovernmentBenefitsInfo { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute(Name = "newsUpdatesAndGuidelines")]
        [JsonConverter(typeof(ValuesJsonConverter))]
        public Values<string, Uri> NewsUpdatesAndGuidelines { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute(Name = "publicTransportClosuresInfo")]
        [JsonConverter(typeof(ValuesJsonConverter))]
        public Values<string, Uri> PublicTransportClosuresInfo { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute(Name = "quarantineGuidelines")]
        [JsonConverter(typeof(ValuesJsonConverter))]
        public Values<string, Uri> QuarantineGuidelines { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute(Name = "schoolClosuresInfo")]
        [JsonConverter(typeof(ValuesJsonConverter))]
        public Values<string, Uri> SchoolClosuresInfo { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute(Name = "travelBans")]
        [JsonConverter(typeof(ValuesJsonConverter))]
        public Values<string, Uri> TravelBans { get; set; }

        //Missing props of CreativeWork
        [System.Runtime.Serialization.DataMemberAttribute(Name = "abstract")]
        [JsonConverter(typeof(ValuesJsonConverter))]
        public OneOrMany<string> Abstract { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute(Name = "copyrightNotice")]
        [JsonConverter(typeof(ValuesJsonConverter))]
        public OneOrMany<string> CopyrightNotice { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute(Name = "sdPublisher")]
        [JsonConverter(typeof(ValuesJsonConverter))]
        public Values<IOrganization, IPerson> SdPublisher { get; set; }
    }
}

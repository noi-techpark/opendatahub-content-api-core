// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Swashbuckle.AspNetCore.Annotations;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json.Converters;
using DataModel.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Diagnostics;
using System.ComponentModel;
using System.Net;

namespace DataModel
{
    #region Interfaces

    //Common Interfaces (shared between all Entities)

    public interface IIdentifiable
    {
        string Id { get; set; }        
    }

    public interface IShortName
    {
        string? Shortname { get; set; }
    }

    public interface IMetaData
    {
        Metadata _Meta { get; set; }
    }

    public interface ILicenseInfo
    {
        LicenseInfo LicenseInfo { get; set; }
    }

    public interface ISource
    {
        [SwaggerSchema("Source of the Data")]
        string Source { get; set; }
    }

    public interface IImportDateassigneable
    {
        DateTime? FirstImport { get; set; }
        DateTime? LastChange { get; set; }
    }

    public interface IMappingAware
    {
        IDictionary<string, IDictionary<string, string>> Mapping { get; set; }
    }

    public interface IActivateable
    {
        bool Active { get; set; }
    }

    public interface ILanguage
    {
        string? Language { get; set; }
    }

    public interface IHasLanguage
    {
        ICollection<string>? HasLanguage { get; set; }
    }

    public interface IDetailInfosAware
    {
        IDictionary<string, Detail> Detail { get; set; }
    }

    public interface IDetailInfos
    {
        string? Header { get; set; }
        string? IntroText { get; set; }
        string? BaseText { get; set; }
        string? Title { get; set; }

        string? MetaTitle { get; set; }
        string? MetaDesc { get; set; }

        string? AdditionalText { get; set; }
        string? GetThereText { get; set; }
    }

    public interface IContactInfos
    {
        string? Address { get; set; }
        string? City { get; set; }
        string? ZipCode { get; set; }
        string? CountryCode { get; set; }
        string? CountryName { get; set; }
        string? Surname { get; set; }
        string? Givenname { get; set; }
        string? NamePrefix { get; set; }
        string? Email { get; set; }
        string? Phonenumber { get; set; }
        string? Faxnumber { get; set; }
        string? Url { get; set; }
        string? Vat { get; set; }
        string? Tax { get; set; }
    }

    public interface IContactInfosAware
    {
        IDictionary<string, ContactInfos> ContactInfos { get; set; }
    }

    public interface IImageGallery
    {
        string? ImageName { get; set; }
        string? ImageUrl { get; set; }
        int? Width { get; set; }
        int? Height { get; set; }
        string? ImageSource { get; set; }

        IDictionary<string, string> ImageTitle { get; set; }
        IDictionary<string, string> ImageDesc { get; set; }

        bool? IsInGallery { get; set; }
        int? ListPosition { get; set; }
        DateTime? ValidFrom { get; set; }
        DateTime? ValidTo { get; set; }
    }

    public interface IImageGalleryAware
    {
        ICollection<ImageGallery>? ImageGallery { get; set; }
    }

    public interface IVideoItems
    {
        string? Name { get; set; }
        string? Url { get; set; }
        string? VideoSource { get; set; }
        string? VideoType { get; set; }

        string? StreamingSource { get; set; }

        string VideoTitle { get; set; }
        string VideoDesc { get; set; }

        bool? Active { get; set; }
        string? CopyRight { get; set; }
        string? License { get; set; }
        string? LicenseHolder { get; set; }
    }

    public interface IVideoItemsAware
    {
        IDictionary<string, ICollection<VideoItems>>? VideoItems { get; set; }
    }

    public interface IGpsInfo
    {
        string? Gpstype { get; set; }
        double Latitude { get; set; }
        double Longitude { get; set; }
        double? Altitude { get; set; }
        string? AltitudeUnitofMeasure { get; set; }
    }

    public interface IGPSInfoAware
    {
        ICollection<GpsInfo> GpsInfo { get; set; }
    }

    public interface IGPSPointsAware
    {
        IDictionary<string, GpsInfo> GpsPoints { get; }
    }

    public interface IGpsTrack
    {
        string? Id { get; set; }
        IDictionary<string, string> GpxTrackDesc { get; set; }
        string? GpxTrackUrl { get; set; }
        string? Type { get; set; }

        string? Format { get; set; }
    }

    public interface IGpsPolygon
    {
        double Latitude { get; set; }
        double Longitude { get; set; }
    }

    public interface IGpsPolygonAware
    {
        ICollection<GpsPolygon>? GpsPolygon { get; set; }
    }

    public interface IOperationSchedules
    {
        IDictionary<string, string> OperationscheduleName { get; set; }
        //string OperationscheduleName { get; set; }
        DateTime Start { get; set; }
        DateTime Stop { get; set; }
        //bool? ClosedonPublicHolidays { get; set; }

        ICollection<OperationScheduleTime>? OperationScheduleTime { get; set; }
    }

    public interface IOperationScheduleTime
    {
        TimeSpan Start { get; set; }
        TimeSpan End { get; set; }
        bool Monday { get; set; }
        bool Tuesday { get; set; }
        bool Wednesday { get; set; }
        bool Thursday { get; set; }
        bool Friday { get; set; }
        bool Saturday { get; set; }
        bool Sunday { get; set; }
        int State { get; set; }
        int Timecode { get; set; }
    }
     
    public interface ITags
    {
        IDictionary<string, List<Tags>> Tags { get; set; }
    }

    public interface IPublishedOn
    {
        ICollection<string>? PublishedOn { get; set; }
    }

    #endregion
      
    #region CommonInfos

    public class Metadata
    {
        public string Id { get; set; }
        [SwaggerEnum(new[] { "accommodation", "accommodationroom", "event", "odhactivitypoi", "measuringpoint", "webcam", "article", "venue", "eventshort", "experiencearea", "region", "metaregion", "tourismassociation", "municipality", "district", "area", "wineaward", "skiarea", "skiregion", "odhtag", "publisher", "tag", "weatherhistory", "weather", "weatherdistrict", "weatherforecast", "weatherrealtime", "snowreport", "odhmetadata", "package", "ltsactivity", "ltspoi", "ltsgastronomy" })]
        public string Type { get; set; }
        public DateTime? LastUpdate { get; set; }
        public string Source { get; set; }
        public bool Reduced { get; set; }

        public UpdateInfo? UpdateInfo { get; set; }
    }

    public class UpdateInfo
    {
        public string? UpdatedBy { get; set; }

        public string? UpdateSource { get; set; }
    }

    public class LicenseInfo
    {
        [SwaggerEnum(new[] { "CC0", "CC-BY", "Closed" })]
        public string? License { get; set; }
        public string? LicenseHolder { get; set; }
        public string? Author { get; set; }
        [SwaggerSchema(Description = "readonly field", ReadOnly = true)]
        public bool ClosedData { get; set; }
    }

    public class Publisher : IIdentifiable, IImportDateassigneable, ILicenseInfo
    {
        public LicenseInfo? LicenseInfo { get; set; }

        public Publisher()
        {
            Name = new Dictionary<string, string>();

            //Mapping = new Dictionary<string, IDictionary<string, string>>();
        }

        public string? Id { get; set; }
        
        public string Key { get; set; }

        public IDictionary<string, string> Name { get; set; }

        public DateTime? FirstImport { get; set; }
        public DateTime? LastChange { get; set; }

        public string? Url { get; set; }

        //New fields to store Information on Push
        public ICollection<PushConfig>? PushConfig { get; set; }
    }

    public class PushConfig
    {
        public ICollection<string>? PathParam { get; set; }

        public string? BaseUrl { get; set; }

        public string? PushApiUrl
        {
            get
            {
                return String.Format("{0}/{1}", this.BaseUrl != null ? this.BaseUrl : "", String.Join("/", this.PathParam));
            }
        }
    }

    public class Source : IIdentifiable, IImportDateassigneable, ILicenseInfo
    {
        public LicenseInfo? LicenseInfo { get; set; }

        public Source()
        {
            Name = new Dictionary<string, string>();
            Description = new Dictionary<string, string>();
        }

        public string? Id { get; set; }
        
        public string Key { get; set; }

        public IDictionary<string, string> Name { get; set; }
        public IDictionary<string, string> Description { get; set; }

        public DateTime? FirstImport { get; set; }
        public DateTime? LastChange { get; set; }

        public string? Url { get; set; }

        [SwaggerSchema("Interfaces that are offered by the source")]
        public ICollection<string>? Interfaces { get; set; }

        public ICollection<string> Types { get; set; }
    }

    public class Detail : IDetailInfos, ILanguage
    {
        public string? Header { get; set; }
        //public string SiteHeader { get; set; }  
        public string? SubHeader { get; set; }
        public string? IntroText { get; set; }
        public string? BaseText { get; set; }
        public string? Title { get; set; }
        //OLT
        //public string Alttext { get; set; }
        public string? AdditionalText { get; set; }
        //NEW
        public string? MetaTitle { get; set; }
        public string? MetaDesc { get; set; }

        public string? GetThereText { get; set; }
        public string? Language { get; set; }

        public ICollection<string>? Keywords { get; set; }

        //New LTS Fields        
        public string? ParkingInfo { get; set; }
        public string? PublicTransportationInfo { get; set; }
        public string? AuthorTip { get; set; }
        public string? SafetyInfo { get; set; }
        public string? EquipmentInfo { get; set; }
    }

    public class GpsInfo : IGpsInfo
    {
        //[DefaultValue("position")]
        //[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        [SwaggerEnum(new[] { "position", "viewpoint", "startingandarrivalpoint", "startingpoint", "arrivalpoint", "carparking", "halfwaypoint", "valleystationpoint", "middlestationpoint", "mountainstationpoint" })]
        public string? Gpstype { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double? Altitude { get; set; }
        public string? AltitudeUnitofMeasure { get; set; }
    }    

    public class GpsTrack : IGpsTrack
    {
        public GpsTrack()
        {
            GpxTrackDesc = new Dictionary<string, string>();
        }

        public string? Id { get; set; }
        public IDictionary<string, string> GpxTrackDesc { get; set; }
        public string? GpxTrackUrl { get; set; }
        public string? Type { get; set; }
        public string? Format { get; set; }
    }

    public class GpsPolygon : IGpsPolygon
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class ImageGallery : IImageGallery
    {
        public ImageGallery()
        {
            ImageTitle = new Dictionary<string, string>();
            ImageDesc = new Dictionary<string, string>();
            ImageAltText = new Dictionary<string, string>();
        }

        public string? ImageName { get; set; }
        public string? ImageUrl { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public string? ImageSource { get; set; }

        public IDictionary<string, string> ImageTitle { get; set; }
        public IDictionary<string, string> ImageDesc { get; set; }
        public IDictionary<string, string> ImageAltText { get; set; }

        public bool? IsInGallery { get; set; }
        public int? ListPosition { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }

        //NEU
        public string? CopyRight { get; set; }
        public string? License { get; set; }
        public string? LicenseHolder { get; set; }
        public ICollection<string>? ImageTags { get; set; }
    }

    public class VideoItems : IVideoItems
    {
        public string? Name { get; set; }
        public string? Url { get; set; }
        public string? VideoSource { get; set; }
        public string? VideoType { get; set; }
        public string? StreamingSource { get; set; }
        public string VideoTitle { get; set; }
        public string VideoDesc { get; set; }
        public bool? Active { get; set; }
        public string? CopyRight { get; set; }
        public string? License { get; set; }
        public string? LicenseHolder { get; set; }
        public string? Language { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }

        //NEW
        public string? Definition { get; set; }
        public double? Duration { get; set; }
        public int? Resolution { get; set; }
        public int? Bitrate { get; set; }
    }

    public class ContactInfos : IContactInfos, ILanguage
    {
        [SwaggerSchema(Description = "Street Address")]
        public string? Address { get; set; }
        
        [SwaggerSchema(Description = "Region (Province / State / Departement / Canton etc...")] 
        public string? Region { get; set; }
        
        [SwaggerSchema(Description = "Regioncode")]
        public string? RegionCode { get; set; }

        [SwaggerSchema(Description = "Area (Additional Area Name)")] 
        public string? Area { get; set; }
        public string? City { get; set; }
        public string? ZipCode { get; set; }
        public string? CountryCode { get; set; }
        public string? CountryName { get; set; }
        public string? Surname { get; set; }
        public string? Givenname { get; set; }
        public string? NamePrefix { get; set; }
        public string? Email { get; set; }
        public string? Phonenumber { get; set; }
        public string? Faxnumber { get; set; }
        public string? Url { get; set; }
        public string? Language { get; set; }
        public string? CompanyName { get; set; }
        public string? Vat { get; set; }
        public string? Tax { get; set; }
        public string? LogoUrl { get; set; }
    }

    [SwaggerSchema("Wiki Article on <a href='https://github.com/noi-techpark/odh-docs/wiki/Operationschedule-Format' target='_blank'>Wiki Article</a>")]
    public class OperationSchedule : IOperationSchedules
    {
        public OperationSchedule()
        {
            OperationscheduleName = new Dictionary<string, string>();
        }
        public IDictionary<string, string> OperationscheduleName { get; set; }
        public DateTime Start { get; set; }
        public DateTime Stop { get; set; }
        /// <summary>
        /// Type: 1 - Standard, 2 - Only day + month recurring (year not to consider) 3 - only month recurring (season: year and day not to consider)
        /// </summary>        
        [SwaggerSchema("1 - Standard, 2 - Only day + month recurring (year not to consider) 3 - only month recurring (season: year and day not to consider), Wiki Article on <a href='https://github.com/noi-techpark/odh-docs/wiki/Operationschedule-Format' target='_blank'>Wiki Article</a>")]
        public string? Type { get; set; }

        public ICollection<OperationScheduleTime>? OperationScheduleTime { get; set; }
    }

    public class OperationScheduleTime : IOperationScheduleTime
    {
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
        public bool Monday { get; set; }
        public bool Tuesday { get; set; }
        public bool Wednesday { get; set; }
        // Here for compatibility reasons
        [SwaggerDeprecated("Will be removed within 2023-12-31")]
        public bool Thuresday { get { return Thursday; }  }
        public bool Thursday { get; set; }
        public bool Friday { get; set; }
        public bool Saturday { get; set; }
        public bool Sunday { get; set; }
        /// <summary>
        /// //1 = closed, 2 = open, 0 = undefined
        /// </summary>
        [SwaggerSchema("1 = closed, 2 = open, 0 = undefined, Wiki Article on <a href='https://github.com/noi-techpark/odh-docs/wiki/Operationschedule-Format' target='_blank'>Wiki Article</a>")]
        public int State { get; set; }

        /// <summary>
        /// 1 = General Opening Time, 2 = time range for warm meals, 3 = time range for pizza, 4 = time range for snack’s
        /// </summary>
        [SwaggerSchema("1 = General Opening Time, 2 = time range for warm meals, 3 = time range for pizza, 4 = time range for snack’s, Wiki Article on <a href='https://github.com/noi-techpark/odh-docs/wiki/Operationschedule-Format' target='_blank'>Wiki Article</a>")]
        public int Timecode { get; set; }
    }

    #endregion


    public class PushResponse
    {
        public string Id { get; set; }
        public string Publisher { get; set; }
        public DateTime Date { get; set; }
        public dynamic Result { get; set; }

        public PushObject? PushObject { get; set; }
    }

    public class PushObject
    {
        public string Id { get; set; }

        //Add the info for the pushed object
        public string Type { get; set; }
    }

    public class PushResult
    {
        public int? Messages { get; set; }
        public bool Success { get; set; }
        public string? Response { get; set; }
        public string? Error { get; set; }

        public static PushResult MergeMultipleFCMPushNotificationResponses(IEnumerable<PushResult> responses)
        {
            return new PushResult()
            {
                Messages = responses.Sum(x => x.Messages),
                Error = String.Join("|", responses.Select(x => x.Error)),
                Success = responses.Any(x => x.Success == true),
                Response = String.Join("|", responses.Select(x => x.Response))
            };
        }
    }

}

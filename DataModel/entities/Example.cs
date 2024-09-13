// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using DataModel.Annotations;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public class Example : IIdentifiable, IImportDateassigneable, IMappingAware, IHasLanguage, ISource, IActivateable, IMetaData, IPublishedOn, IDetailInfosAware, IGPSInfoAware, IImageGalleryAware, IShortName, IContactInfosAware, ILicenseInfo, IGPSPointsAware
    {
        public Example()
        {
            //Initialize Dictionaries
            Detail = new Dictionary<string, Detail>();
            ContactInfos = new Dictionary<string, ContactInfos>();
            Mapping = new Dictionary<string, IDictionary<string, string>>();
            AdditionalProperties = new Dictionary<string, dynamic>();
        }

        public Metadata _Meta { get; set; }
        public string Id { get; set; }
        public DateTime? FirstImport { get; set; }
        public DateTime? LastChange { get; set; }
        public string Source { get; set; }
        public bool Active { get; set; }
        public string? Shortname { get; set; }
        public IDictionary<string, IDictionary<string, string>> Mapping { get; set; }
        public ICollection<string>? HasLanguage { get; set; }        
        public ICollection<string>? PublishedOn { get; set; }
        public IDictionary<string, Detail> Detail { get; set; }
        public ICollection<GpsInfo> GpsInfo { get; set; }

        [SwaggerDeprecated("Deprecated, use GpsInfo")]
        [SwaggerSchema(Description = "generated field", ReadOnly = true)]
        public IDictionary<string, GpsInfo> GpsPoints
        {
            get
            {
                return this.GpsInfo.ToGpsPointsDictionary();
            }
        }

        public ICollection<ImageGallery>? ImageGallery { get; set; }        
        public IDictionary<string, ContactInfos> ContactInfos { get; set; }

        public ICollection<string>? ExampleTypeIds { get; set; }
        public LicenseInfo LicenseInfo { get; set; }

        public IDictionary<string, dynamic>? AdditionalProperties { get; set; }
    }

    public class ExampleType : IIdentifiable
    {
        public ExampleType()
        {
            TypeDesc = new Dictionary<string, string>();
        }

        public string? Id { get; set; }
        public long Bitmask { get; set; }
        public string? Type { get; set; }
        public string? Parent { get; set; }
        public string Key { get; set; }

        public IDictionary<string, string>? TypeDesc { get; set; }
    }

    public class ExampleLinked : Example
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
        public ICollection<ExampleTypes> ODHTags
        {
            get
            {
                return this.ExampleTypeIds != null ? this.ExampleTypeIds.Select(x => new ExampleTypes() { Id = x, Self = "ExampleType/" + x }).ToList() : new List<ExampleTypes>();
            }
        }
    }

    public class ExampleTypes
    {
        public string Id { get; set; }
        public string? Self { get; set; }
    }

}

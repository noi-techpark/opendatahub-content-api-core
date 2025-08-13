// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using DataModel;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentApiModels
{
    public class OpenDataHubContent : IIdentifiable, IMetaData, IImportDateassigneable, ISource, IActivateable, IMappingAware, ILicenseInfo, IPublishedOn, IGPSInfoAware, IHasLanguage, IDetailInfosAware
    {
        public string? Id { get; set; }
        public Metadata? _Meta { get; set; }
        public DateTime? FirstImport { get; set; }
        public DateTime? LastChange { get; set; }
        public string? Source { get; set; }
        public bool Active { get; set; }
        public IDictionary<string, IDictionary<string, string>>? Mapping { get; set; }
        public LicenseInfo? LicenseInfo { get; set; }
        public ICollection<string>? PublishedOn { get; set; }
        public ICollection<GpsInfo>? GpsInfo { get; set; }        
        public ICollection<string>? HasLanguage { get; set; }
        public IDictionary<string, Detail>? Detail { get; set; }
    }

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
}

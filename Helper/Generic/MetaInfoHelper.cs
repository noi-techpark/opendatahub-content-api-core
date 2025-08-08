// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using ContentApiModels;
using DataModel;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    public class MetadataHelper
    {
        //Simple Method to reset the Metainfo
        public static Metadata GetMetadata(
            string id,
            string type,
            string? source,
            DateTime? lastupdated = null,
            bool reduced = false
        )
        {
            return new Metadata()
            {
                Id = id,
                Type = type,
                LastUpdate = lastupdated,
                Source = source,
                Reduced = reduced,
            };
        }

        public static Metadata GetMetadata<T>(
            T data,
            string source,
            //DateTime? lastupdated = null,
            bool reduced = false
        )
            where T : IIdentifiable, IMetaData
        {
            string type = ODHTypeHelper.TranslateType2TypeString<T>(data);

            //If source is already set use the old source
            //if (data._Meta != null && !string.IsNullOrEmpty(data._Meta.Source))
            //    source = data._Meta.Source;

            return new Metadata()
            {
                Id = data.Id,
                Type = type,
                //LastUpdate = lastupdated,
                LastUpdate = DateTime.Now,
                Source = source,
                Reduced = reduced,
            };
        }

        public static Metadata GetMetadataobject<T>(T myobject, Func<T, Metadata> metadataganerator)
        {
            return metadataganerator(myobject);
        }

        //Discriminator if the function is not passed
        public static Metadata GetMetadataobject<T>(T myobject, bool reduced = false)
        {
            return myobject switch
            {
                ExampleLinked al => GetMetadataforExample(al, reduced),              
                PublisherLinked pbl => GetMetadataforPublisher(pbl),
                SourceLinked pbl => GetMetadataforSource(pbl),
                GeoShapeJson gj => GetMetadataForGeoShapeJson(gj),
                _ => throw new Exception("not known odh type")
            };            
        }

        public static Metadata GetMetadataforExample(ExampleLinked data, bool reduced)
        {
            return GetMetadata(data, "noi", reduced);
        }
      
        public static Metadata GetMetadataforPublisher(PublisherLinked data)
        {
            string sourcemeta = "noi";
            
            return GetMetadata(data, sourcemeta);
        }

        public static Metadata GetMetadataforSource(SourceLinked data)
        {
            string sourcemeta = "noi";

            return GetMetadata(data, sourcemeta);
        }

        public static Metadata GetMetadataForGeoShapeJson(GeoShapeJson data)
        {
            string type = ODHTypeHelper.TranslateType2TypeString<GeoShapeJson>(data);

            return new Metadata()
            {
                Id = data.Id.ToString(),
                Type = type,
                LastUpdate = DateTime.Now,
                Source = data.Source,
                Reduced = false,
            };
        }

        public static void SetUpdateHistory(Metadata? oldmetadata, Metadata newmetadata)
        {
            if (oldmetadata == null && newmetadata.UpdateInfo != null && !String.IsNullOrEmpty(newmetadata.UpdateInfo.UpdatedBy))
            {
                //New dataset
                newmetadata.UpdateInfo.UpdateHistory =
                [
                    new UpdateHistory() { LastUpdate = newmetadata.LastUpdate, UpdateSource = newmetadata.UpdateInfo.UpdateSource, UpdatedBy = newmetadata.UpdateInfo.UpdatedBy },
                ];
            }
            else if (oldmetadata != null)
            {
                //Compatibility Update Info not present
                if (oldmetadata.UpdateInfo == null)
                {
                    oldmetadata.UpdateInfo = new UpdateInfo();
                    newmetadata.UpdateInfo = new UpdateInfo();
                }

                //Compatibility UpdateHistory not present
                if (oldmetadata.UpdateInfo.UpdateHistory == null)
                    newmetadata.UpdateInfo.UpdateHistory = new List<UpdateHistory>();
                else
                    newmetadata.UpdateInfo.UpdateHistory = oldmetadata.UpdateInfo.UpdateHistory;

                if (!String.IsNullOrEmpty(newmetadata.UpdateInfo.UpdatedBy))
                {
                    if (newmetadata.UpdateInfo.UpdateHistory.Where(x => x.UpdatedBy == newmetadata.UpdateInfo.UpdatedBy && x.UpdateSource == newmetadata.UpdateInfo.UpdateSource).Count() > 0)
                    {
                        var updatehistorytoupdate = newmetadata.UpdateInfo.UpdateHistory.Where(x => x.UpdatedBy == newmetadata.UpdateInfo.UpdatedBy && x.UpdateSource == newmetadata.UpdateInfo.UpdateSource).FirstOrDefault();
                        updatehistorytoupdate.LastUpdate = newmetadata.LastUpdate;
                    }
                    else
                    {
                        newmetadata.UpdateInfo.UpdateHistory.Add(new UpdateHistory() { LastUpdate = newmetadata.LastUpdate, UpdateSource = newmetadata.UpdateInfo.UpdateSource, UpdatedBy = newmetadata.UpdateInfo.UpdatedBy });
                    }
                }
                //newmetadata.UpdateInfo.UpdateHistory.  TryAddOrUpdate(, );
            }
            else
                newmetadata.UpdateInfo.UpdateHistory = null;
        }

    }
}

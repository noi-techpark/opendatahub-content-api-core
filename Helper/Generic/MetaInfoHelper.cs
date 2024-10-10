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
        public static Metadata GetMetadata(string id, string type, string? source, DateTime? lastupdated = null, bool reduced = false)
        {
            return new Metadata() { Id = id, Type = type, LastUpdate = lastupdated, Source = source, Reduced = reduced };
        }

        public static Metadata GetMetadata<T>(T data, string source, DateTime? lastupdated = null, bool reduced = false) where T : IIdentifiable, IMetaData
        {
            string type = ODHTypeHelper.TranslateType2TypeString<T>(data);

            //If source is already set use the old source
            //if (data._Meta != null && !string.IsNullOrEmpty(data._Meta.Source))
            //    source = data._Meta.Source;            
            
            return new Metadata() { Id = data.Id, Type = type, LastUpdate = lastupdated, Source = source, Reduced = reduced };
        }
               
        public static Metadata GetMetadataobject<T>(T myobject, Func<T, Metadata> metadataganerator)
        {
            return metadataganerator(myobject);
        }

        //Discriminator if the function is not passed
        public static Metadata GetMetadataobject<T>(T myobject)
        {
            return myobject switch
            {
                ExampleLinked al => GetMetadataforExample(al),              
                PublisherLinked pbl => GetMetadataforPublisher(pbl),
                SourceLinked pbl => GetMetadataforSource(pbl),
                _ => throw new Exception("not known odh type")
            };            
        }

        public static Metadata GetMetadataforExample(ExampleLinked data)
        {
            bool reduced = false;
            if (data._Meta != null)
                reduced = (bool)data._Meta.Reduced;

            return GetMetadata(data, "noi", data.LastChange, reduced);
        }
      
        public static Metadata GetMetadataforPublisher(PublisherLinked data)
        {
            string sourcemeta = "noi";
            
            return GetMetadata(data, sourcemeta, data.LastChange);
        }

        public static Metadata GetMetadataforSource(SourceLinked data)
        {
            string sourcemeta = "noi";

            return GetMetadata(data, sourcemeta, data.LastChange);
        }
    }
}

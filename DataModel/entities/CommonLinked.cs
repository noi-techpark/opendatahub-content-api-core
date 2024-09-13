// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using DataModel.Annotations;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace DataModel
{
    /// <summary>
    /// This class contains the classes used by Open Data Hub PG Instance with linked data
    /// </summary>
    #region Linked Sub Classes
   
    public class Tags
    {
        public string Id { get; set; }

        public string Source { get; set; }
        public string? Self
        {
            get
            {
                return "Tag/" + this.Id;
            }
        }
    }

    #endregion

    #region Linked Main Classes    
    
    public class PublisherLinked : Publisher, IMetaData
    {
        public Metadata? _Meta { get; set; }

        [SwaggerSchema(Description = "generated field", ReadOnly = true)]
        public string? Self
        {
            get
            {
                return this.Id != null ? "Publisher/" + Uri.EscapeDataString(this.Id) : null;                
            }
        }
    }

    public class SourceLinked : Source, IMetaData
    {
        public Metadata? _Meta { get; set; }

        [SwaggerSchema(Description = "generated field", ReadOnly = true)]
        public string? Self
        {
            get
            {
                return this.Id != null ? "Source/" + Uri.EscapeDataString(this.Id) : null;                
            }
        }
    }

    #endregion        
}

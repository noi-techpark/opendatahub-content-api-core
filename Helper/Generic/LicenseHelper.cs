// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    public class LicenseHelper
    {
        public static LicenseInfo GetLicenseInfoobject(string licensetype, string author, string licenseholder, bool closeddata)
        {
            return new LicenseInfo() { Author = author, License = licensetype, LicenseHolder = licenseholder, ClosedData = closeddata };
        }

        //TODO Make a HOF and apply all the rules
        public static LicenseInfo GetLicenseInfoobject<T>(T myobject, Func<T, LicenseInfo> licensegenerator)
        {
            return licensegenerator(myobject);
        }

        public static LicenseInfo GetLicenseforExample(Example data)
        {
            var isopendata = true;
            var licensetype = "CC0";
            var licenseholder = @"https://noi.bz.it";
            
            return GetLicenseInfoobject(licensetype, "", licenseholder, !isopendata);
        }
    }
}

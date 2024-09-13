// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;

namespace ContentApiCore.Controllers.api
{
    public class ExampleHelper
    {
        public List<string> typelist;
        public List<string> idlist;
        public List<string> languagelist;
        public bool? active;
        public string? lastchange;
        public List<string> publishedonlist;
        public List<string> sourcelist;

        public static ExampleHelper Create(
            string? typefilter, string? idfilter, string? languagefilter, bool? activefilter, string? source, string? lastchange, string? publishedonfilter)
        {
            return new ExampleHelper(typefilter, idfilter, languagefilter, activefilter, source, lastchange, publishedonfilter);
        }

        private ExampleHelper(
            string? typefilter, string? idfilter, string? languagefilter,
            bool? activefilter, string? source, string? lastchange, string? publishedonfilter)
        {
            typelist = new List<string>();
            
            if (!String.IsNullOrEmpty(typefilter))
            {
                if (int.TryParse(typefilter, out int typeinteger))
                {                    
                    typelist = Helper.ExampleListCreator.CreateExampleTypefromFlag(typefilter);
                }
                else
                {
                    typelist.AddRange(Helper.CommonListCreator.CreateIdList(typefilter));
                }
            }
            
            idlist = Helper.CommonListCreator.CreateIdList(idfilter?.ToUpper());
            languagelist = Helper.CommonListCreator.CreateIdList(languagefilter);
            sourcelist = Helper.CommonListCreator.CreateSmgPoiSourceList(source);                        
            active = activefilter;            
            this.lastchange = lastchange;            
            publishedonlist = Helper.CommonListCreator.CreateIdList(publishedonfilter?.ToLower());
        }
    }
}

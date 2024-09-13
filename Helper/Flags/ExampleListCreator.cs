// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Text;

namespace Helper
{
    public class ExampleListCreator
    {
        public static List<string> CreateExampleTypefromFlag(string typefilter)
        {
            List<string> typelist = new List<string>();

            if (typefilter != "null")
            {
                int typefilterint = 0;
                if (int.TryParse(typefilter, out typefilterint))
                {
                    ExampleTypeFlag mypoitypeflag = (ExampleTypeFlag)typefilterint;

                    var myflags = mypoitypeflag.GetFlags().GetDescriptionList();

                    foreach (var myflag in myflags)
                    {
                        typelist.Add(myflag);
                    }
                }
                else
                    return new List<string>();
            }

            return typelist;
        }
    }
}

// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Helper
{
    public static class EnumHelper
    {
        public static IEnumerable<object> GetValues<T>()
        {
            foreach (object? value in System.Enum.GetValues(typeof(T)))
            {
                if (value != null)
                    yield return value;
            }
        }
    }
    //Packages Weekday 
    [Flags]
    public enum WeekdayFlag
    {
        Monday = 1,
        Tuesday = 2,
        Wednesday = 4,
        Thuresday = 8,
        Friday = 16,
        Saturday = 32,
        Sunday = 64
    }


    #region Articles

    [Flags]
    public enum ExampleTypeFlag
    {
        [Description("examplecategory1")]
        examplecategory1 = 1,  //1
        [Description("examplecategory2")]
        examplecategory2 = 1 << 1, //2
        [Description("examplecategory3")]
        examplecategory3 = 1 << 2, //4
        [Description("examplecategory4")]
        examplecategory4 = 1 << 3 //8   
    }

  
    #endregion

}



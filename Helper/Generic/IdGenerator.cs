// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentApiModels;
using DataModel;

namespace Helper
{
    public class IdGenerator
    {
        /// <summary>
        /// Translates a ODH Type Object to the Type (Metadata) as String
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="odhtype"></param>
        /// <returns></returns>
        public static string GenerateIDFromType<T>(T odhtype)
        {
            return CreateGUID(GetIDStyle(odhtype));
        }

        public static void CheckIdFromType<T>(T odhtype) where T : IIdentifiable
        {
            var style = GetIDStyle(odhtype);

            if (style == IDStyle.uppercase)
                odhtype.Id = odhtype.Id.ToUpper();
            else if (style == IDStyle.lowercase)
                odhtype.Id = odhtype.Id.ToLower();
        }

        public static string CheckIdFromType<T>(string id) where T : IIdentifiable
        {
            var style = GetIDStyle(typeof(T));

            if (style == IDStyle.uppercase)
                return id.ToUpper();
            else if (style == IDStyle.lowercase)
                return id.ToLower();

            return id;
        }

        private static string CreateGUID(IDStyle style)
        {
            var id = System.Guid.NewGuid().ToString();

            if (style == IDStyle.uppercase)
                id = id.ToUpper();
            else if(style == IDStyle.lowercase)
                id = id.ToLower();

            return id;
        }        

        public static IDStyle GetIDStyle<T>(T odhtype)
        {
            return odhtype switch
            {
                Example or ExampleLinked => IDStyle.uppercase,
                _ => throw new Exception("not known odh type")
            };
        }

        public static IDStyle GetIDStyle(Type odhtype)
        {
            return odhtype switch
            {
                Type _ when odhtype == typeof(Example) || odhtype == typeof(ExampleLinked) => IDStyle.uppercase,                      
                _ => throw new Exception("not known odh type")
            };
        }

        public static string TransformIDbyIdStyle(string id, IDStyle idstyle)
        {
            if (idstyle == IDStyle.uppercase)
                return id.ToUpper();
            else
                return id.ToLower();
        }
    }

    public enum IDStyle
    {
        uppercase,
        lowercase,
        mixed
    }
}

// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel;

namespace PushServer
{
    public class FCMMessageConstructor
    {
        public static FCMModels? ConstructMyMessage(string identifier, string language, IIdentifiable myobject)
        {
            var message = new FCMModels();

            //Construct your FCM Message here
           
            return message;
        }

        public static FCMessageV2? ConstructMyMessageV2(string identifier, string language, IIdentifiable myobject)
        {
            var message = new FCMessageV2();

            //Construct your FCM Message here

            return message;
        }
    }
}

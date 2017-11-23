using System;
using System.Collections.Generic;
using System.Text;

namespace IceBot.Classes
{
    public class Global
    {


        public static string OnlineOffline(bool tf)
        {

            if (tf == true)
            {
                return "Online";
            } else
            {
                return "Offline";
            }

            
        }

    }
}

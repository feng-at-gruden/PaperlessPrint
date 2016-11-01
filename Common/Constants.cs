using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class Constants
    {

        public const Int32 UdpPort = 12345;
        public const Int32 TcpPort = 12345;
        public const Int32 MaxClients = 5;


        ///Network Commands
        public static class NetCommand
        {
            public static String SHOW_BILL = "SHOW_BILL";
            public static String SIGN_DONE = "SIGN_DONE";

        }

    }

}

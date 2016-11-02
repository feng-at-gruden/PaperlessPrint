using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class Constants
    {
        public static bool DEBUG = true;
        public const String Version = "v0.1";

        public const String TabletIP = "192.168.31.22";  
        //public const String TabletIP = "172.23.0.33";           
        public const Int32 TabletPort = 12345;
        public const Int32 MaxClients = 5;

        public const int A4Width = 595;
        public const int A4Height = 842;
        
        

    }

    ///Network Commands
    public static class NetWorkCommand
    {
        public const String SHOW_BILL = "SHOW_BILL";
        public const String SIGN_DONE = "SIGN_DONE";
    }

}

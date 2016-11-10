using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class Constants
    {
        public static bool DEBUG = false;
        public const String Version = "v0.1";

        public const String TempFileFolder = "tmp";

        //public const String TabletIP = "192.168.31.22";  
        //public const String TabletIP = "172.23.0.33";
        public const String TabletIP = "192.168.1.101";
        public const Int32 TabletPort = 12345;
        public const Int32 MaxClients = 5;
        public const Int32 BufferSize = 65536;

        public const int A4Width = 595;
        public const int A4Height = 842;

        public const int PenWidth = 5;
        public const int DesktopSignatureScale = 1;     //1:1 Scale

    }

    ///Network Commands
    public static class NetWorkCommand
    {
        public const String OK = "OK";
        public const String FILE_RECEIVED = "FILE_RECEIVED";
        public const String CMD = "##";

        public const String QUIT = CMD + "QUIT";
        public const String DRAW = CMD + "DRAW";
        public const String CLEAN = CMD + "CLEAN";
        public const String SIGNATURE_DONE = CMD + "SIGNATURE_DONE";
        public const String SEND_FILE = CMD + "SEND_FILE";

        //public const String SHOW_BILL = CMD + "SHOW_BILL";
        //public const String SIGN_DONE = CMD + "SIGN_DONE";
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class Constants
    {

#if DEBUG
        public static bool DEBUG = true;
#else
        public static bool DEBUG = false;
#endif
        public const String Version = "v0.1";
        public const String TempFileFolder = "tmp";         //临时文件存储目录

        public const int SignatureDeviceIPPort = 12345;
        public const int BufferSize = 65536;
        public const int MaxTryConnect = 10;                 //前台连接平板尝试次数 5*0.5秒
        public const int MaxClients = 5;

        public const int A4Width = 595;
        public const int A4Height = 842;
        public const int PenWidth = 1;

    }

    ///Network Commands
    public static class NetWorkCommand
    {
        public const String OK = "OK";
        public const String FILE_RECEIVED = "FILE_RECEIVED";
        public const String CMD = "##";

        public const String QUIT = CMD + "QUIT";
        public const String DRAW = CMD + "DRAW";
        public const String STYLUS_ADD = CMD + "STYLUS_ADD";
        public const String STYLUS_REMOVE = CMD + "STYLUS_REMOVE";
        public const String STYLUS_END = CMD + "STYLUS_END";
        public const String CLEAN = CMD + "CLEAN";
        public const String PAGEUP = CMD + "PAGEUP";
        public const String PAGEDOWN = CMD + "PAGEDOWN";
        public const String SIGNATURE_DONE = CMD + "SIGNATURE_DONE";
        public const String SEND_FILE = CMD + "SEND_FILE";
        public const String RECEPTION_EXIT = CMD + "RECEPTION_EXIT";

        //public const String SHOW_BILL = CMD + "SHOW_BILL";
        //public const String SIGN_DONE = CMD + "SIGN_DONE";
    }

}

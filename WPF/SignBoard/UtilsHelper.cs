using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Common;

namespace SignBoard
{
    public static class UtilsHelper
    {

        public const int A4Width = 2604;
        public const int A4Height = 3507;
        //public const int A4Width = 595;
        //public const int A4Height = 842;

        public static Size GetPDFDisplayAreaSize()
        {

            double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;

            double w = A4Height * screenHeight / A4Width;

            return new Size(w, screenHeight);
        }

        public static Size GetA4DisplayAreaSize()
        {

            double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;

            double w = Constants.A4Height * screenHeight / Constants.A4Width;

            return new Size(w, screenHeight);
        }

    }
    
}

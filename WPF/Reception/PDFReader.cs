using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reception
{
    public partial class PDFReader : UserControl
    {
        public PDFReader()
        {
            InitializeComponent();

            foxitReader1.ShowToolBar(false);
            foxitReader1.ShowStatusBar(false);
            foxitReader1.ShowTitleBar(false);
            foxitReader1.ShowNavigationPanels(false);
            foxitReader1.ShowBookmark(false);
            //foxitReader1.UnLockActiveX("license_id","unlock_code");
        }


        public void LoadPDF(string filename)
        {
            foxitReader1.OpenFile(filename, null);
            foxitReader1.ShowNavigationPanels(false);
            foxitReader1.SetLayoutShowMode(FoxitReaderSDKProLib.BrowseMode.MODE_SINGLE, 1);
        }

        public void ClosePDF()
        {
            foxitReader1.CloseFile();
        }

        public void GotoPage(int p)
        {
            foxitReader1.GoToPage(p);
        }

        public void SetZoomLevel(int level)
        {
            foxitReader1.ZoomLevel = level;
        }


    }
}

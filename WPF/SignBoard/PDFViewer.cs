using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SignBoard
{
    public partial class PDFViewer : UserControl
    {
        public PDFViewer()
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
            foxitReader1.Rotate = 3;
            foxitReader1.ShowNavigationPanels(false);
            foxitReader1.SetLayoutShowMode(FoxitReaderSDKProLib.BrowseMode.MODE_SINGLE, 1);
        }

        public void GotoPage(int p)
        {
            foxitReader1.GoToPage(p);
        }

    }
}

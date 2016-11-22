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
            //foxitReader1.ShowToolBar(false);
            foxitReader1.ShowStatusBar(false);
            foxitReader1.UnLockActiveX("license_id","unlock_code");
            LoadPDF("D:\test.pdf");
        }


        public void LoadPDF(string filename)
        {
            foxitReader1.OpenFile(filename, null);
        }


    }
}

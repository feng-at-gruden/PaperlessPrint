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
        }


        public void LoadPDF(String filename)
        {
            foxitReader1.OpenFile(filename, null);
        }

    }
}

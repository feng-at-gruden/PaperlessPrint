using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PaperlessPrint
{

    public partial class MainForm : Form
    {
        string[] args = null;

        public MainForm()
        {
            InitializeComponent();
        }

        public MainForm(string[] args)
        {
            InitializeComponent();
            this.args = args;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            //检测参数， 先预览 并
        }



    }


}

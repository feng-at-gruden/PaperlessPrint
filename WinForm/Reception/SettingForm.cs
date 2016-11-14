using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Common.Utiles;
using Common.Model;

namespace PaperlessPrint
{
    public partial class SettingForm : Form
    {
        public SettingForm()
        {
            InitializeComponent();
        }

        private void SettingForm_Load(object sender, EventArgs e)
        {

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //Read form UI
            Configuration config = new Configuration
            {
                TabletAddress = this.txtTabletAddress.Text.Trim(),
            };

            ConfigurationHelper.SaveSettings(config);
            this.Close();
        }




        #region Private Functions

        

        #endregion

    }
}

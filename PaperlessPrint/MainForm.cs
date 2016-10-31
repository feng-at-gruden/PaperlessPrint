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
        long mLastShiftPressTime;

        public MainForm()
        {
            InitializeComponent();
        }

        public MainForm(string[] args)
        {
            InitializeComponent();
            this.args = args;
        }

        /// <summary>
        /// //检测参数， 本机预览并发送指令到Tablet 等待签名
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Load(object sender, EventArgs e)
        {
            if(this.args!=null)
            {

            }
        }


        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 给Tablet发指令 显示账单并签名确认
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTabletShow_Click(object sender, EventArgs e)
        {
            MessageBox.Show("等待客户确认");
        }

        /// <summary>
        /// 按住Ctrl 双击启动Setting Form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
            {
                new SettingForm().Show();
            }
        }



    }


}

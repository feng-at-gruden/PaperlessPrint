using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Globalization;
using System.IO;
using System.Threading;
using Common;
using Common.TCPServer;

namespace PaperlessPrint
{

    public partial class MainForm : Form
    {

        #region Fields

        private string[] args = null;
        private AsyncTcpClient client;
        private String currentFileName;


        int tempIndex = 0;
        #endregion


        public MainForm()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        public MainForm(string[] args)
        {
            InitializeComponent();
            this.args = args;
            currentFileName = this.args[0];
            CheckForIllegalCrossThreadCalls = false;
        }


        #region UI Events

        /// <summary>
        /// //检测参数， 本机预览并发送指令到Tablet 等待签名
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Load(object sender, EventArgs e)
        {
            InitNetWork();
            InitUI();

            if (this.args != null)
            {
                ReviewBill(currentFileName);
                picReview.ImageLocation = currentFileName;
            }
            else
            {
                this.Close();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseNetWork();
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
        /// 发送到打印机
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrint_Click(object sender, EventArgs e)
        {
            //SendPlaintText(NetWorkCommand.SHOW_BILL + ":" + currentFileName);
        }

        /// <summary>
        /// TODO, 笔迹合成 生成pdf  上传ftp
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConfirmSign_Click(object sender, EventArgs e)
        {
            //Local debug 
            if (currentFileName.IndexOf("test") >= 0)
            {
                string index = currentFileName.Substring(7, 1);
                currentFileName = currentFileName.Replace(index, tempIndex++.ToString());
                if (tempIndex > 4)
                    tempIndex = 0;
            }
            ReviewBill(currentFileName);
        }

        /// <summary>
        /// 按住Ctrl 双击启动Setting Form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void picReview_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
            {
                new SettingForm().Show();
            }
            else if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
            {
                Constants.DEBUG = !Constants.DEBUG;
                txtLog.Visible = Constants.DEBUG;
            }
        }
       
        

        #endregion


        #region Private Functions


        private void InitUI()
        {
            if(Constants.DEBUG)
            {
                txtLog.Visible = true;
            }
            else
            {
                txtLog.Visible = false;
            }
            //Update Form size
            System.Drawing.Rectangle rect = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
            int h = rect.Height - SystemInformation.CaptionHeight - SystemInformation.MenuHeight;   //Cut off title bar heigth and task bar heith;
            this.Height = h;
            this.Width = (int)Math.Floor((Double)Constants.A4Width * h / Constants.A4Height);
            btnConfirmSign.Left = btnPrint.Left = btnClose.Left = this.Width - 22 - btnClose.Width;
            toolStripStatusLabel1.Text = Constants.Version;
        }

        private void Log(String s)
        {
            txtLog.Text += DateTime.Now.ToString("HH:mm:ss") + " " + s + "\r\n";
            txtLog.SelectionStart = txtLog.Text.Length;
            txtLog.ScrollToCaret();
        }
        
        private void ReviewBill(String filepath)
        {
            //Open in local

            //Send to Tablet
            //SendPlaintText(NetWorkCommand.SHOW_BILL + ":" + currentFileName);
            SendFile(filepath);
        }

        #endregion



        #region NetWork

        /// <summary>
        /// 初始化TCP Client
        /// </summary>
        private void InitNetWork()
        {
            client = new AsyncTcpClient(IPAddress.Parse(Constants.TabletIP), Constants.TabletPort);
            client.Connect();
            client.ServerConnected += new EventHandler<TcpServerConnectedEventArgs>(Connected);
            client.ServerDisconnected += new EventHandler<TcpServerDisconnectedEventArgs>(Disconnected);
            client.PlaintextReceived += new EventHandler<TcpDatagramReceivedEventArgs<string>>(PlainTextReceived);
        }


        private void CloseNetWork()
        {
            if (client != null && client.Connected)
            {
                client.Close();
                //client.Dispose();
            }
        }

        private void SendPlaintText(String s)
        {
            if(client.Connected)
            {
                client.Send(s);
            }
        }

        private void SendFile(String path)
        {
            if (client.Connected)
            {
                FileStream fs = new FileStream(path, FileMode.Open);
                //获取文件大小
                long size = fs.Length;
                byte[] data = new byte[size];
                //将文件读到byte数组中
                fs.Read(data, 0, data.Length);
                fs.Close();
                client.Send(NetWorkCommand.SEND_FILE + ":" + size);
                Thread.Sleep(500);
                client.Send(data);
            }
        }

        private void Connected(object sender, TcpServerConnectedEventArgs e)
        {
            Log(string.Format(CultureInfo.InvariantCulture, "Connected:{0}", e.Addresses[0].ToString()));
        }

        private void Disconnected(object sender, TcpServerDisconnectedEventArgs e)
        {
            Log(string.Format(CultureInfo.InvariantCulture, "Server disconnected."));
        }

        private void PlainTextReceived(object sender, TcpDatagramReceivedEventArgs<string> e)
        {
            string cmd = e.Datagram;
            Log(string.Format(CultureInfo.InvariantCulture, "Received:{0}", cmd));
            if (cmd.IndexOf(NetWorkCommand.QUIT) >= 0)
            {
                this.Close();
            }
        }

        #endregion

        
        









    }


}

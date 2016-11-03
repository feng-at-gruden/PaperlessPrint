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
using System.Net.Sockets;
using System.Threading;
using System.Globalization;
using System.IO;
using Common;
using Common.Utiles;
using Common.TCPServer;


namespace Tablet
{

    public partial class MainForm : Form
    {

        # region Fields

        private AsyncTcpServer server;
        private byte[] TempBuffer = new byte[0];
        private long receiveFileSize = 0;
        private long currentFileSize = 0;

        #endregion


        public MainForm()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }


        #region UI Events

        private void MainForm_Load(object sender, EventArgs e)
        {
            InitServer();
            InitUI();
            if(!Directory.Exists(Constants.TempFileFolder))
            {
                Directory.CreateDirectory(Constants.TempFileFolder);
            }
        }



        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            //关闭Soket
            server.Stop();
            server.Dispose();

            CleanTempFiles();
        }

        private void txtTestContent_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (txtTestContent.Text != null && e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                server.SendAll(txtTestContent.Text);
                txtTestContent.Text = "";
            }
        }

        private void picPreview_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
            {
                Constants.DEBUG = !Constants.DEBUG;
                panel1.Visible = Constants.DEBUG;
            }
        }

        #endregion




        #region Private Functions


        /// <summary>
        /// 初始化窗体
        /// </summary>
        private void InitUI()
        {
            if (Constants.DEBUG)
            {
                panel1.Visible = true;
            }
            else
            {
                panel1.Visible = false;
            }
            //Update Form size
            System.Drawing.Rectangle rect = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
            int h = rect.Height - SystemInformation.CaptionHeight - SystemInformation.MenuHeight;   //Cut off title bar heigth and task bar heith;
            this.Height = h;
            this.Width = (int)Math.Floor((Double)Constants.A4Width * h / Constants.A4Height);
            
            toolStripStatusLabel1.Text = Constants.Version;
        }

        private void UpdateReceiveProgress(int v)
        {
            if(this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate { UpdateReceiveProgress(v); })); 
                return;
            }
            toolStripProgressBar1.Value = v;
            if (v>0 && v<100)
                toolStripProgressBar1.Visible = true;
            else
                toolStripProgressBar1.Visible = false;
            picPreview.Image = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="txt"></param>
        private void Log(String txt)
        {
            txtLog.Text += DateTime.Now.ToString("HH:mm:ss") + " " + txt + "\r\n";
            txtLog.SelectionStart = txtLog.Text.Length;
            txtLog.ScrollToCaret();
        }

        /// <summary>
        /// 
        /// </summary>
        private void CleanTempFiles()
        {
            //Clean local jpg files
            foreach(string d in Directory.GetFileSystemEntries(Constants.TempFileFolder))
            {
                if(File.Exists(d) && d.IndexOf(".jpg")>0)
                {
                    try
                    {
                        File.Delete(d);
                    }
                    catch { }
                }
            }

        }

        private byte[] CopyToByteArry(byte[] bBig, byte[] bSmall)
        {
            byte[] tmp = new byte[bBig.Length + bSmall.Length];
            System.Buffer.BlockCopy(bBig, 0, tmp, 0, bBig.Length);
            System.Buffer.BlockCopy(bSmall, 0, tmp, bBig.Length, bSmall.Length);
            return tmp;
        }



        #endregion



        #region TCP Server

        

        private void InitServer()
        {
            server = new AsyncTcpServer(Constants.TabletPort);
            server.Encoding = Encoding.UTF8;
            server.ClientConnected += new EventHandler<TcpClientConnectedEventArgs>(ClientConnected);
            server.ClientDisconnected += new EventHandler<TcpClientDisconnectedEventArgs>(ClientDisconnected);
            //server.PlaintextReceived += new EventHandler<TcpDatagramReceivedEventArgs<string>>(PlainTextReceived);
            server.DatagramReceived += new EventHandler<TcpDatagramReceivedEventArgs<byte[]>>(DatagramReceived);
            server.Start();
            Log("网络启动:" + NetworkHelper.GetLocalIP() + ":" + Constants.TabletPort);
        }

        private void ClientConnected(object sender, TcpClientConnectedEventArgs e)
        {
            Log(string.Format(CultureInfo.InvariantCulture, "{0} connected.", e.TcpClient.Client.RemoteEndPoint.ToString()));
        }

        private void ClientDisconnected(object sender, TcpClientDisconnectedEventArgs e)
        {
            Log(string.Format(CultureInfo.InvariantCulture, "{0} disconnected.", e.TcpClient.Client.RemoteEndPoint.ToString()));
        }

        /// <summary>
        /// Not sure not run this method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlainTextReceived(object sender, TcpDatagramReceivedEventArgs<string> e)
        {
            Log(string.Format(CultureInfo.InvariantCulture, "{0}:{1}", e.TcpClient.Client.RemoteEndPoint.ToString(), e.Datagram));
            if (e.Datagram != "Received")
            {
                Console.Write(string.Format("Client : {0} --> ", e.TcpClient.Client.RemoteEndPoint.ToString()));
                Console.WriteLine(string.Format("{0}", e.Datagram));
                server.Send(e.TcpClient, NetWorkCommand.OK);
            }
            if(e.Datagram.IndexOf(NetWorkCommand.SEND_FILE)>=0)
            {
                long size = long.Parse(e.Datagram.Split(':')[1]);
                TempBuffer = new byte[0];
                receiveFileSize = size;
            }
        }

        private void DatagramReceived(object sender, TcpDatagramReceivedEventArgs<byte[]> e)
        {

            if(e.Datagram[0] == 35)     // Start with # is plaint CMD
            {
                string cmd = System.Text.Encoding.Default.GetString(e.Datagram);
                if (cmd.IndexOf(NetWorkCommand.SEND_FILE) >= 0)
                {
                    long size = long.Parse(cmd.Split(':')[1]);
                    receiveFileSize = size;
                    UpdateReceiveProgress(0);
                }
                else
                {
                    Log(string.Format(CultureInfo.InvariantCulture, "{0}:{1}", e.TcpClient.Client.RemoteEndPoint.ToString(), cmd));
                    server.Send(e.TcpClient, NetWorkCommand.OK);
                }
                return;
            }
            
            if(currentFileSize < receiveFileSize)
            {
                currentFileSize += e.Datagram.Length;
                TempBuffer = CopyToByteArry(TempBuffer, e.Datagram);
                int p = (int)Math.Floor((double)currentFileSize * 100 / receiveFileSize);
                UpdateReceiveProgress(p);
            }
            if (currentFileSize == receiveFileSize && receiveFileSize > 0)
            {
                //Save to file
                string filename = DateTime.Now.ToString("yyyyMMddHHmmss") + ".jpg";
                FileStream fs = new FileStream(Constants.TempFileFolder + "\\" + filename, FileMode.Create);
                fs.Write(TempBuffer, 0, TempBuffer.Length);
                fs.Close();
                picPreview.ImageLocation = Constants.TempFileFolder + "\\" + filename;
                Log(string.Format(CultureInfo.InvariantCulture, "{0} received from {1}", filename, e.TcpClient.Client.RemoteEndPoint.ToString()));
                
                server.Send(e.TcpClient, NetWorkCommand.FILE_SAVED);
                
                currentFileSize = receiveFileSize = 0;
                TempBuffer = new byte[0];
            }

        }

        #endregion

        

        




    }

}

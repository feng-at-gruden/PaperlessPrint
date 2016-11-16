using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Globalization;
using System.IO;
using System.Drawing.Drawing2D;
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
        private TcpClient currentClient;

        private bool drawing;
        private int pX = -1;
        private int pY = -1;

        private Bitmap bitmap;

        #endregion



        public MainForm()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }


        #region UI Events

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Load(object sender, EventArgs e)
        {
            InitServer();
            InitUI();
            if(!Directory.Exists(Constants.TempFileFolder))
            {
                Directory.CreateDirectory(Constants.TempFileFolder);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            //关闭Soket
            server.Stop();
            server.Dispose();

            CleanTempFiles();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
            {
                //TODO, 显示退出确认密码框
                this.Close();
            }
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtTestContent_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (txtTestContent.Text != null && e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                server.SendAll(txtTestContent.Text);
                txtTestContent.Text = "";
            }
        }

        /// <summary>
        ///  屏蔽ctrl+F4
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.F4) && (e.Alt == true))
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void picPreview_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
            {
                Constants.DEBUG = !Constants.DEBUG;
                statusStrip1.Visible = panel1.Visible = Constants.DEBUG;
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
                statusStrip1.Visible = true;
            }
            else
            {
                panel1.Visible = false;
                statusStrip1.Visible = false;
            }
            //Update Form size
            System.Drawing.Rectangle rect = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
            int h = rect.Height - SystemInformation.CaptionHeight - SystemInformation.MenuHeight;   //Cut off title bar heigth and task bar heith;
            this.Height = h;
            this.Width = (int)Math.Floor((Double)Constants.A4Width * h / Constants.A4Height);
            this.Top = this.Left = 0;
            
            toolStripStatusLabel1.Text = Constants.Version;

            //Signature area
            picSignature.Parent = picPreview;

            bitmap = new Bitmap(picSignature.Width, picSignature.Height, picSignature.CreateGraphics());
            Graphics.FromImage(bitmap).Clear(Color.Transparent);
            btnClear.Left = this.Width - btnClear.Width - 22;
            btnClear.Top = 10;
        }

        /// <summary>
        /// 更新接受进度条
        /// </summary>
        /// <param name="v"></param>
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
        /// 打印调试信息
        /// </summary>
        /// <param name="txt"></param>
        private void Log(String txt)
        {
            txtLog.Text += DateTime.Now.ToString("HH:mm:ss") + " " + txt + "\r\n";
            txtLog.SelectionStart = txtLog.Text.Length;
            txtLog.ScrollToCaret();
        }

        /// <summary>
        /// 删除临时文件
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

        /// <summary>
        /// 字节数组拷贝
        /// </summary>
        /// <param name="bBig"></param>
        /// <param name="bSmall"></param>
        /// <returns></returns>
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
            server = new AsyncTcpServer(Constants.SignatureDeviceIPPort);
            server.Encoding = Encoding.UTF8;
            server.ClientConnected += new EventHandler<TcpClientConnectedEventArgs>(ClientConnected);
            server.ClientDisconnected += new EventHandler<TcpClientDisconnectedEventArgs>(ClientDisconnected);
            //server.PlaintextReceived += new EventHandler<TcpDatagramReceivedEventArgs<string>>(PlainTextReceived);
            server.DatagramReceived += new EventHandler<TcpDatagramReceivedEventArgs<byte[]>>(DatagramReceived);
            server.Start();
            Log("网络启动:" + NetworkHelper.GetLocalIP());
        }

        private void ClientConnected(object sender, TcpClientConnectedEventArgs e)
        {
            currentClient = e.TcpClient;
            Log(string.Format(CultureInfo.InvariantCulture, "{0} connected.", e.TcpClient.Client.RemoteEndPoint.ToString()));
        }

        private void ClientDisconnected(object sender, TcpClientDisconnectedEventArgs e)
        {
            currentClient = null;
            Log(string.Format(CultureInfo.InvariantCulture, "{0} disconnected.", e.TcpClient.Client.RemoteEndPoint.ToString()));
        }

        
        private void PlainTextReceived(object sender, TcpDatagramReceivedEventArgs<string> e)
        {
            currentClient = e.TcpClient;
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
            currentClient = e.TcpClient;
            if (e.Datagram[0] == 35 && e.Datagram[1] == 35 && e.Datagram.Length < 30)     // Start with ## is plaint CMD
            {
                string cmd = System.Text.Encoding.Default.GetString(e.Datagram);
                if (cmd.IndexOf(NetWorkCommand.SEND_FILE) >= 0)
                {
                    long size = long.Parse(cmd.Split(':')[1]);
                    receiveFileSize = size;
                    UpdateReceiveProgress(0);
                    CleanSignature();
                }
                else if (cmd.IndexOf(NetWorkCommand.SIGNATURE_DONE) >= 0)
                {
                    //CleanSignature();
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

                server.Send(e.TcpClient, NetWorkCommand.FILE_RECEIVED);
                
                currentFileSize = receiveFileSize = 0;
                TempBuffer = new byte[0];
            }

        }


        private void DrawLineToReception(int pX, int pY, int nX, int nY)
        {
            if (nX < 0 || nX > this.Width || nY < 0 || nY > this.Height)
                return;
            if( currentClient!=null )
                server.Send(currentClient, string.Format(CultureInfo.InvariantCulture, "{0}:{1}:{2}:{3}:{4}", NetWorkCommand.DRAW, pX, pY, nX, nY));
        }

        #endregion



        #region Signature

        private void picSignature_MouseDown(object sender, MouseEventArgs e)
        {
            drawing = true;
            pX = e.X;
            pY = e.Y;
        }

        private void picSignature_MouseUp(object sender, MouseEventArgs e)
        {
            drawing = false;
        }

        private void picSignature_MouseMove(object sender, MouseEventArgs e)
        {
            if (drawing)
            {
                Graphics panel = Graphics.FromImage(bitmap);
                Pen pen = new Pen(Color.Black, Constants.PenWidth);

                pen.EndCap = LineCap.Round;
                pen.StartCap = LineCap.Round;

                panel.DrawLine(pen, pX, pY, e.X, e.Y);
                DrawLineToReception(pX, pY, e.X, e.Y);

                picSignature.CreateGraphics().DrawImageUnscaled(bitmap, new Point(0, 0));
            }

            pX = e.X;
            pY = e.Y;
        }


        private void btnClear_Click(object sender, EventArgs e)
        {
            CleanSignature();
        }

        private void CleanSignature()
        {
            bitmap = new Bitmap(picSignature.Width, picSignature.Height, picSignature.CreateGraphics());
            Graphics.FromImage(bitmap).Clear(Color.Transparent);
            picSignature.Refresh();
            if (currentClient != null)
                server.Send(currentClient, NetWorkCommand.CLEAN);
        }

        #endregion

        












    }

}

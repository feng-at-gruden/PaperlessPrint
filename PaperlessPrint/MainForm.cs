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
using System.Drawing.Drawing2D;
using iTextSharp.text;
using iTextSharp.text.pdf;
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
        private Bitmap bitmap;
        private bool emptySignature = true;

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
        /// 合成 生成pdf  上传ftp
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConfirmSign_Click(object sender, EventArgs e)
        {
            if(bitmap != null && !emptySignature)
            {
                //Save signature jpg
                if (!Directory.Exists(Constants.TempFileFolder))
                {
                    Directory.CreateDirectory(Constants.TempFileFolder);
                }

                //保存临时签名图像文件
                string filename = Constants.TempFileFolder + "/" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".png";
                //Bitmap tempBitmap = new Bitmap(picSignature.Width, picSignature.Height);
                //Graphics g = Graphics.FromImage(tempBitmap);
                //g.Clear(System.Drawing.Color.White);
                //g.DrawImage(bitmap, 0, 0);
                //tempBitmap.Save(filename, System.Drawing.Imaging.ImageFormat.Png);
                bitmap.Save(filename, System.Drawing.Imaging.ImageFormat.Png);

                //合并生成PDF
                GeneratePDF(currentFileName, filename);

                //上传FTP TODO

                SendPlaintText(NetWorkCommand.SIGNATURE_DONE);

                //清除文件
                CleanTempFile(filename);
            }
            else
            {
                MessageBox.Show("签名未完成，请重试！");
            }

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

            //Signature preview area
            if(Constants.DesktopSignatureScale > 1)
            {
                picSignature.Width = (int) this.Width / Constants.DesktopSignatureScale;
                picSignature.Height = (int)this.Height / Constants.DesktopSignatureScale;
                picSignature.Top = this.Height - picSignature.Height - statusStrip1.Height - 54;
                picSignature.Left = this.Width - picSignature.Width - 40;
            }
            else
            {
                picSignature.Dock = DockStyle.Fill;
            }
            picSignature.Parent = picReview;
            bitmap = new Bitmap(picSignature.Width, picSignature.Height, picSignature.CreateGraphics());
            Graphics.FromImage(bitmap).Clear(Color.Transparent);
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
            picReview.ImageLocation = currentFileName;
            SendFile(filepath);
        }

        private void DrawLine(int pX, int pY, int nX, int nY)
        {
            Graphics panel = Graphics.FromImage(bitmap);

            Pen pen = new Pen(Color.Black, Math.Max(Constants.PenWidth / Constants.DesktopSignatureScale, 1));

            pen.EndCap = LineCap.Round;
            pen.StartCap = LineCap.Round;

            panel.DrawLine(pen, pX / Constants.DesktopSignatureScale, pY / Constants.DesktopSignatureScale, nX / Constants.DesktopSignatureScale, nY / Constants.DesktopSignatureScale);

            picSignature.CreateGraphics().DrawImageUnscaled(bitmap, new Point(0, 0));
        }

        private void GeneratePDF(string f1, string f2)
        {
            Document doc = new Document(PageSize.A4, 0, 0, 0, 0);
            PdfWriter.GetInstance(doc, new FileStream(f2.Replace(".png",".pdf"), FileMode.Create));
            doc.Open();

            iTextSharp.text.Image img1 = iTextSharp.text.Image.GetInstance(f1);
            img1.ScalePercent(100f);
            doc.Add(img1);

            iTextSharp.text.Image img2 = iTextSharp.text.Image.GetInstance(f2);
            img2.ScalePercent(100f);
            doc.Add(img2);

            doc.Close();
        }

        private void CleanTempFileFolder()
        {
            //Clean local jpg files
            foreach (string d in Directory.GetFileSystemEntries(Constants.TempFileFolder))
            {
                if (File.Exists(d))
                {
                    try
                    {
                        File.Delete(d);
                    }
                    catch { }
                }
            }
        }

        private void CleanTempFile(string filename)
        {
            if (File.Exists(filename))
            {
                try
                {
                    File.Delete(filename);
                }
                catch { }
            }
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
            client.DatagramReceived += new EventHandler<TcpDatagramReceivedEventArgs<byte[]>>(DatagramReceived);
        }


        private void CloseNetWork()
        {
            if (client != null && client.Connected)
            {
                client.Close();
                client.Dispose();
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
            else if(cmd.IndexOf(NetWorkCommand.DRAW)>=0)
            {
                emptySignature = false;
                String[] cmds = cmd.Split(NetWorkCommand.CMD.ToArray());
                foreach (String c in cmds)
                {
                    String[] arg = c.Split(':');
                    if (arg.Length == 5)
                        DrawLine(int.Parse(arg[1]), int.Parse(arg[2]), int.Parse(arg[3]), int.Parse(arg[4]));
                }
            }
            else if(cmd.IndexOf(NetWorkCommand.CLEAN)>=0)
            {
                emptySignature = true;
                bitmap = new Bitmap(picSignature.Width, picSignature.Height, picSignature.CreateGraphics());
                Graphics.FromImage(bitmap).Clear(Color.Transparent);
                picSignature.Refresh();
            }
        }

        private void DatagramReceived(object sender, TcpDatagramReceivedEventArgs<byte[]> e)
        {
            if (e.Datagram[0] == 35 && e.Datagram[1] == 35 )     // Start with ## is plaint CMD
            {
                string cmd = System.Text.Encoding.Default.GetString(e.Datagram);
            }
        }

        #endregion

        
    }


}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Globalization;
using System.IO;
using System.Threading;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Common;
using Common.TCPServer;

namespace Reception
{
    /// <summary>
    /// 前台 打印预览窗体
    /// </summary>
    public partial class MainWindow : Window
    {

        #region Fields

        private string[] args = null;
        private AsyncTcpClient client;
        private String currentFileName;
        private bool emptySignature = true;

        ImageBrush formBG;

        int tempIndex = -1;


        #endregion

        public MainWindow()
        {
            InitializeComponent();
            this.args = (Application.Current as App).args;

            if (this.args != null && this.args.Length>0)
            {
                currentFileName = this.args[0];
                InitNetWork();
                InitUI();

                ReviewBill(currentFileName);
            }
        }


        #region UI Events

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            //Local debug 
            if (currentFileName.IndexOf("test") >= 0)
            {
                string index = currentFileName.Substring(7, 1);
                if (tempIndex == -1)
                    tempIndex = int.Parse(index);

                tempIndex++;
                if (tempIndex > 4)
                    tempIndex = 0;

                currentFileName = currentFileName.Replace(index, tempIndex.ToString());
            }
            ReviewBill(currentFileName);
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CloseNetWork();
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
            if (client.Connected)
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

            if (cmd.IndexOf(NetWorkCommand.QUIT) >= 0)
            {
                Log(string.Format(CultureInfo.InvariantCulture, "Received:{0}", cmd));
                this.Close();
            }
            else if (cmd.IndexOf(NetWorkCommand.DRAW) >= 0)
            {
                emptySignature = false;
                String[] cmds = cmd.Split(NetWorkCommand.CMD.ToArray());
                foreach (String c in cmds)
                {
                    String[] arg = c.Split(':');
                    if (arg.Length == 5)
                    {
                        //TODO
                        //DrawLine(int.Parse(arg[1]), int.Parse(arg[2]), int.Parse(arg[3]), int.Parse(arg[4]));
                    }
                }
            }
            else if (cmd.IndexOf(NetWorkCommand.STYLUS) >= 0)
            {
                emptySignature = false;
                String[] cmds = cmd.Split(NetWorkCommand.CMD.ToArray());
                foreach (String c in cmds)
                {
                    String[] arg = c.Split(':');
                    int lX = 0, lY = 0;
                    int scw = 0, sch = 0, ssw = 0, ssh = 0;
                    foreach (var ps in arg)
                    {
                        String[] p = ps.Split(',');
                        if (p.Length == 5)
                        {
                            //接收签名设备屏幕信息
                            scw = int.Parse(p[1]);
                            sch = int.Parse(p[2]);
                            ssw = int.Parse(p[3]);
                            ssh = int.Parse(p[4]);
                        }

                        if (p.Length == 3)
                        {
                            lX = lX == 0 ? (int)double.Parse(p[0]) : lX;
                            lY = lY == 0 ? (int)double.Parse(p[1]) : lY;
                            //TODO
                            //DrawLine(scw, sch, ssw, ssh, lX, lY, (int)double.Parse(p[0]), (int)double.Parse(p[1]));
                            lX = (int)double.Parse(p[0]);
                            lY = (int)double.Parse(p[1]);
                        }
                    }
                }
            }
            else if (cmd.IndexOf(NetWorkCommand.CLEAN) >= 0)
            {
                Log(string.Format(CultureInfo.InvariantCulture, "Received:{0}", cmd));
                emptySignature = true;
                inkCanvas1.Strokes.Clear();
            }
        }

        private void DatagramReceived(object sender, TcpDatagramReceivedEventArgs<byte[]> e)
        {
            if (e.Datagram[0] == 35 && e.Datagram[1] == 35)     // Start with ## is plaint CMD
            {
                string cmd = System.Text.Encoding.Default.GetString(e.Datagram);
            }
        }

        #endregion




        #region Private Functions

        private void InitUI()
        {
            double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
            double h = screenHeight - SystemParameters.CaptionHeight - SystemParameters.MenuBarHeight;
            double w = Math.Floor((Double)Constants.A4Width * h * 1d / Constants.A4Height);

            this.SetValue(Window.WidthProperty, w);
            this.SetValue(Window.HeightProperty, h);
            this.SetValue(Window.TopProperty, 0d);
            this.SetValue(Window.LeftProperty, 0d);

            //Signature preview area
            //inkCanvas BG
            formBG = new ImageBrush();
            formBG.Stretch = Stretch.Fill;
        }

        private void Log(String s)
        {
            //TODO
        }

        private void ReviewBill(String filepath)
        {
            //Open in local
            BitmapImage bg = new BitmapImage();
            bg.BeginInit();
            bg.CacheOption = BitmapCacheOption.OnLoad;
            bg.UriSource = new Uri(currentFileName, UriKind.RelativeOrAbsolute);
            bg.EndInit();
            formBG.ImageSource = bg;
            inkCanvas1.Background = formBG;

            int retry = 0;
            while (!client.Connected)
            {
                Thread.Sleep(500);
                retry++;
                if (retry >= Constants.MaxTryConnect)
                {
                    MessageBox.Show("签字板连接错误");
                    this.Close();
                    return;
                }
            }
            
            //Send to Tablet            
            SendFile(filepath);
        }


        /// <summary>
        /// 转换为本设备坐标 画线
        /// </summary>
        /// <param name="sourceCanvasSizeW"></param>
        /// <param name="sourceCanvasSizeH"></param>
        /// <param name="sourceScreenW"></param>
        /// <param name="sourceScreenH"></param>
        /// <param name="pX"></param>
        /// <param name="pY"></param>
        /// <param name="nX"></param>
        /// <param name="nY"></param>
        private void DrawLine(int sourceCanvasSizeW, int sourceCanvasSizeH, int sourceScreenW, int sourceScreenH, int pX, int pY, int nX, int nY, double weight)
        {
            int cw = (int)imgBill.GetValue(System.Windows.Controls.Image.WidthProperty);
            int ch = (int)imgBill.GetValue(System.Windows.Controls.Image.HeightProperty);
            int nsx = 0, nsy = 0, ntx = 0, nty = 0;
            double feed = cw * 1d / sourceCanvasSizeW;
            nsx = (int)Math.Ceiling(pX * feed);
            nsy = (int)Math.Ceiling(pY * feed);
            ntx = (int)Math.Ceiling(nX * feed);
            nty = (int)Math.Ceiling(nY * feed);

            //TODO, add to local inkCanvas

        }

        private void GeneratePDF(string f1, string f2)
        {
            Document doc = new Document(PageSize.A4, 0, 0, 0, 0);
            PdfWriter.GetInstance(doc, new FileStream(f2.Replace(".png", ".pdf"), FileMode.Create));
            doc.Open();

            iTextSharp.text.Image img1 = iTextSharp.text.Image.GetInstance(f1);
            //img1.ScalePercent(1f);
            img1.ScaleToFit(doc.PageSize);
            img1.SetAbsolutePosition(0, 0);
            doc.Add(img1);

            iTextSharp.text.Image img2 = iTextSharp.text.Image.GetInstance(f2);
            img2.ScaleToFit(doc.PageSize);
            img2.SetAbsolutePosition(0, 0);
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

        


    }
}

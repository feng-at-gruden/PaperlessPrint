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
using System.Windows.Ink;
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

        double cWidth, cHeight;

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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cWidth = inkCanvas1.ActualWidth;
            cHeight = inkCanvas1.ActualHeight; 
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
            else if (cmd.IndexOf(NetWorkCommand.CLEAN) >= 0)
            {
                Log(string.Format(CultureInfo.InvariantCulture, "Received:{0}", cmd));
                CleanSignature();
            }
            else if (cmd.IndexOf(NetWorkCommand.STYLUS_ADD) >= 0 || cmd.IndexOf(NetWorkCommand.STYLUS_REMOVE) >= 0)
            {
                if (cmd.IndexOf(NetWorkCommand.CLEAN) >= 0)
                {
                    CleanSignature();
                    return;
                }
                
                String[] cmds = cmd.Split(NetWorkCommand.CMD.ToArray());
                foreach (String c in cmds)
                {
                    String[] arg = c.Split(':');
                    double lX = 0, lY = 0;
                    float lP = 0;
                    double scw = 0, sch = 0, ssw = 0, ssh = 0;
                    StylusPointCollection pts = new StylusPointCollection();
                    bool isAdd = true;
                    foreach (var ps in arg)
                    {
                        String[] p = ps.Split(',');
                        if (p.Length == 5)
                        {
                            isAdd = NetWorkCommand.STYLUS_ADD.IndexOf(p[0])>=0;
                            //接收签名设备屏幕信息
                            scw = double.Parse(p[1]);
                            sch = double.Parse(p[2]);
                            ssw = double.Parse(p[3]);
                            ssh = double.Parse(p[4]);
                        }

                        if (p.Length == 3)
                        {
                            double feedX = cWidth * 1d / scw;
                            double feedY = cHeight * 1d / sch;
                            
                            lX = double.Parse(p[0]);
                            lY = double.Parse(p[1]);
                            lP = float.Parse(p[2]);
                            pts.Add(new StylusPoint(lX * feedX, lY * feedY, lP));
                        }
                    }
                    if (pts.Count > 0)
                    {
                        if (!Dispatcher.CheckAccess())
                        {
                            Dispatcher.Invoke(
                                    () => DrawLine(pts, isAdd), System.Windows.Threading.DispatcherPriority.Normal);
                        }
                        else
                        {
                            DrawLine(pts, isAdd);
                        }
                    }
                }
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
            double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
            double h = screenHeight - SystemParameters.CaptionHeight - SystemParameters.MenuBarHeight;
            double w = Math.Floor(Constants.A4Width * h/ Constants.A4Height);

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
        private void DrawLine(StylusPointCollection pts, bool add)
        {
            Stroke s = new Stroke(pts);
            s.DrawingAttributes.Color = Colors.Red;
            if (add)
            {
                inkCanvas1.Strokes.Add(s);
            }
            else
            {
                //TODO;
                //inkCanvas1.Strokes.Remove(s);
            }
        }

        private void CleanSignature()
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(
                        () => inkCanvas1.Strokes.Clear(), System.Windows.Threading.DispatcherPriority.Normal);
            }
            else
            {
                inkCanvas1.Strokes.Clear();
            }
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

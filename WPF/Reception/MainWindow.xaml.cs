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
        private double billImageW, billImageH;          //账单图像文件尺寸
        //double cWidth, cHeight;                         //InkCanvas尺寸

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

        /// <summary>
        ///  保存临时图像，生成PDF，上传FTP
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if(inkCanvas1.Strokes.Count<=0)
            {
                MessageBox.Show("签名为空，请重试！");
                return;
            }

            //Save signature image
            if (!Directory.Exists(Constants.TempFileFolder))
            {
                Directory.CreateDirectory(Constants.TempFileFolder);
            }
            string filename = Constants.TempFileFolder + "/" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".png";

            double width = inkCanvas1.ActualWidth;
            double height = inkCanvas1.ActualHeight;
            double dpi = 96d;

            RenderTargetBitmap rtb = new RenderTargetBitmap((int)Math.Round(width), (int)Math.Round(height), dpi, dpi, System.Windows.Media.PixelFormats.Default);
            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(inkCanvas1);
                dc.DrawRectangle(vb, null, new Rect(new Point(), new System.Windows.Size(width, height)));
            }
            rtb.Render(dv);

            BitmapEncoder pngEncoder = new PngBitmapEncoder();
            pngEncoder.Frames.Add(BitmapFrame.Create(rtb));
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                pngEncoder.Save(ms);
                System.IO.File.WriteAllBytes(filename, ms.ToArray());
            }

            //合并生成PDF
            GeneratePDF(null, filename);

            //上传FTP TODO

            SendPlaintText(NetWorkCommand.SIGNATURE_DONE);

            //清除文件
            //CleanTempFile(filename);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //cWidth = inkCanvas1.ActualWidth;
            //cHeight = inkCanvas1.ActualHeight;

            double scaleX = ((Grid)this.Content).RenderSize.Width / billImageW;
            double scaleY = ((Grid)this.Content).RenderSize.Height / billImageH;
            ScaleTransform sf = new ScaleTransform(scaleX, scaleY);
            inkCanvas1.LayoutTransform = sf;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CloseNetWork();
            //清除文件
            //CleanTempFile(currentFileName);
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
                            double feedX = billImageW * 1d / scw;
                            double feedY = billImageH * 1d / sch;
                            
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
            //Signature preview area
            //inkCanvas BG
            BitmapImage bg = loadImage(currentFileName);
            Size imageSize = getImageSize(currentFileName);
            billImageW = imageSize.Width;
            billImageH = imageSize.Height;

            //设置为inkCanvas为图片实际尺寸
            inkCanvas1.SetValue(InkCanvas.WidthProperty, billImageW);
            inkCanvas1.SetValue(InkCanvas.HeightProperty, billImageH);

            formBG = new ImageBrush();
            formBG.Stretch = Stretch.Fill;
            //设置为背景
            formBG.ImageSource = bg;
            inkCanvas1.Background = formBG;
            //设置窗体按比例尺寸
            double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
            double h = screenHeight - SystemParameters.CaptionHeight - SystemParameters.MenuBarHeight;
            //double w = Math.Floor(Constants.A4Width * h/ Constants.A4Height);
            double w = Math.Floor(billImageW * h / billImageH);

            this.SetValue(Window.WidthProperty, w);
            this.SetValue(Window.HeightProperty, h);
            this.SetValue(Window.TopProperty, 0d);
            this.SetValue(Window.LeftProperty, 0d);
            //获取显示区域尺寸 并设置inkCanvas缩放比例
            double scaleX = ((Grid)this.Content).RenderSize.Width / billImageW;
            double scaleY = ((Grid)this.Content).RenderSize.Height / billImageH;
            ScaleTransform sf = new ScaleTransform(scaleX, scaleY);
            inkCanvas1.LayoutTransform = sf;

            //cWidth = inkCanvas1.ActualWidth;
            //cHeight = inkCanvas1.ActualHeight; 
        }

        private void Log(String s)
        {
            //TODO
        }


        private BitmapImage loadImage(String filepath)
        {
            //Open in local
            BitmapImage bg = new BitmapImage();
            bg.BeginInit();
            bg.CacheOption = BitmapCacheOption.OnLoad;
            bg.UriSource = new Uri(filepath, UriKind.RelativeOrAbsolute);
            bg.EndInit();
            return bg;
        }

        private Size getImageSize(string path)
        {
            using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                BitmapFrame frame = BitmapFrame.Create(fileStream, BitmapCreateOptions.DelayCreation, BitmapCacheOption.None);
                Size s = new Size(frame.PixelWidth, frame.PixelHeight);
                return s;
            }
        }

        private void ReviewBill(String filepath)
        {
            currentFileName = filepath;
            //Resize Window
            InitUI();

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
            SendFile(currentFileName);
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

            if (f1 != null)
            {
                iTextSharp.text.Image img1 = iTextSharp.text.Image.GetInstance(f1);
                //img1.ScalePercent(1f);
                img1.ScaleToFit(doc.PageSize);
                img1.SetAbsolutePosition(0, 0);
                doc.Add(img1);
            }

            if (f2 != null)
            {
                iTextSharp.text.Image img2 = iTextSharp.text.Image.GetInstance(f2);
                img2.ScaleToFit(doc.PageSize);
                img2.SetAbsolutePosition(0, 0);
                doc.Add(img2);
            }

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

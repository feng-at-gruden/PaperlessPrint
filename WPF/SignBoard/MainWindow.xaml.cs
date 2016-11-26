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
using System.Net.Sockets;
using System.Threading;
using System.Globalization;
using System.IO;
using Common;
using Common.Utiles;
using Common.TCPServer;

namespace SignBoard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        # region Fields
        public ContentWindow ContentWindow;

        private AsyncTcpServer server;
        private byte[] TempBuffer = new byte[0];
        private long receiveFileSize = 0;
        private long currentFileSize = 0;
        private string receiveFileName;
        private TcpClient currentClient;

        bool stylusInputEnabled = true;
        bool mouseInputEnabled = true;
        bool touchInputEnabled = false;

        ImageBrush formBG;

        bool WorkingWithPDF;
        bool ClientCleanClosed;
        string currentBillFile;

        # endregion



        public MainWindow()
        {
            InitializeComponent();
            InitServer();
            InitUI();
        }



        #region UI Events

        private void Window_Closed(object sender, EventArgs e)
        {
            //e.Cancel = true;
            CleanTempFiles();
            Application.Current.Shutdown();
        }

        private void Image_TouchDown(object sender, TouchEventArgs e)
        {
            CleanSignature();
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CleanSignature();
        }

        private void Strokes_StrokesChanged(object sender, StrokeCollectionChangedEventArgs e)
        {
            if (e.Added != null && e.Added.Count > 0)
            {
                //Log(e.Added.Count + " - added " + DateTime.Now.Ticks);
                foreach (var s in e.Added)
                {
                    UpdateLineToReception(s, true);
                }

            }

            if (e.Removed != null && e.Removed.Count > 0)
            {
                //Log(e.Removed.Count + " - removed " + DateTime.Now.Ticks);
                foreach (var s in e.Removed)
                {
                    UpdateLineToReception(s, false);
                }
            }
        }

        private void InkCanvas_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            if (!touchInputEnabled)
                e.Handled = true;
        }

        private void InkCanvas_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!mouseInputEnabled)
                e.Handled = true;
        }

        private void InkCanvas_PreviewStylusDown(object sender, StylusDownEventArgs e)
        {
            if (!stylusInputEnabled)
                e.Handled = true;
        }

        #endregion



        #region Private Functions


        /// <summary>
        /// 
        /// </summary>
        private void InitUI()
        {
            double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;

            //Update grid size
            grid1.SetValue(Grid.WidthProperty, screenHeight);
            grid1.SetValue(Grid.HeightProperty, screenWidth);
            RotateTransform rt = new RotateTransform(-90, 0.5, 0.5);
            grid1.LayoutTransform = rt;

            //inkCanvas BG
            formBG = new ImageBrush();
            formBG.Stretch = Stretch.Fill;

            //Update inkCanvas size;
            Size contentSize = UtilsHelper.GetA4DisplayAreaSize();
            inkCanvas1.SetValue(InkCanvas.WidthProperty, contentSize.Height - 20);
            inkCanvas1.SetValue(InkCanvas.HeightProperty, contentSize.Width - 100);

            inkCanvas1.Strokes.StrokesChanged += this.Strokes_StrokesChanged;
            inkCanvas1.PreviewTouchDown += this.InkCanvas_PreviewTouchDown;
            inkCanvas1.PreviewMouseDown += this.InkCanvas_PreviewMouseDown;
            inkCanvas1.PreviewStylusDown += this.InkCanvas_PreviewStylusDown;
        }


        private void Log(string log)
        {
            /*
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(
                        () => this.Title = log, System.Windows.Threading.DispatcherPriority.Normal);
            }
            else
            {
                this.Title = log;
            }*/
        }

        private void DisplayBill(string filename)
        {
            WorkingWithPDF = filename.IndexOf(".pdf", StringComparison.InvariantCultureIgnoreCase) >= 0;
            currentBillFile = string.Format("{0}\\{1}\\{2}", Directory.GetCurrentDirectory(), Constants.TempFileFolder, filename);
            if (WorkingWithPDF)
            {
                ContentWindow.LoadPDF(currentBillFile);
            }
            else
            {
                //Working with IMAGE
                BitmapImage bg = new BitmapImage();
                bg.BeginInit();
                bg.CacheOption = BitmapCacheOption.OnLoad;
                bg.UriSource = new Uri(currentBillFile, UriKind.RelativeOrAbsolute);
                bg.EndInit();
                formBG.ImageSource = bg;
                inkCanvas1.Background = formBG;
            }
            UpdateReceiveProgress(0);
        }

        private void CleanSignature()
        {
            UpdateReceiveProgress(0);
            if (inkCanvas1.Strokes != null)
                inkCanvas1.Strokes.Clear();

            if (currentClient != null)
                server.Send(currentClient, NetWorkCommand.CLEAN);
        }

        private void StartNewSignature(bool closeAD = true)
        {
            if (closeAD)
                this.ContentWindow.CloseADWindow();

            CleanSignature();
            //Clean opened content
            if(WorkingWithPDF)
            {
                ContentWindow.ClosePDF();
            }
            else
            {
                inkCanvas1.Background = (Brush)new System.Windows.Media.BrushConverter().ConvertFromString("#11FFFFFF");
            }
            EnableInkCanvas(true);
        }

        private void SignatureFinished(bool showThanks = true)
        {
            ClientCleanClosed = true;
            StartNewSignature(closeAD: false);
            EnableInkCanvas(false);

            //Display a thank message
            if (showThanks)
            {
                this.ContentWindow.ShowThanks();
            }
            else
            {
                this.ContentWindow.ShowAD();
            }

            currentClient.Close();
            currentClient = null;
        }

        private void EnableInkCanvas(bool enable)
        {
            //TODO, hide buttons
            inkCanvas1.SetValue(InkCanvas.IsEnabledProperty, enable);
            //btnClean.SetValue(Button.VisibilityProperty, enable ? Visibility.Visible : Visibility.Hidden);
        }

        /// <summary>
        /// 删除临时文件
        /// </summary>
        private void CleanTempFiles()
        {
            if (!Directory.Exists(Constants.TempFileFolder))
            {
                return;
            }
            //Clean local temp files
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

        private void UpdateReceiveProgress(int p)
        {
            progressBar1.SetValue(ProgressBar.ValueProperty, p + 0d);
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
            ClientCleanClosed = false;
            currentClient = e.TcpClient;
            Log(string.Format(CultureInfo.InvariantCulture, "{0} connected.", e.TcpClient.Client.RemoteEndPoint.ToString()));
        }

        private void ClientDisconnected(object sender, TcpClientDisconnectedEventArgs e)
        {
            if (!ClientCleanClosed)
            {
                if (!Dispatcher.CheckAccess())
                {
                    Dispatcher.Invoke(
                            () => SignatureFinished(showThanks: false), System.Windows.Threading.DispatcherPriority.Normal);
                }
                else
                {
                    SignatureFinished(showThanks: false);
                }
            }
            
            currentClient = null;
            //Log(string.Format(CultureInfo.InvariantCulture, "{0} disconnected.", e.TcpClient.Client.RemoteEndPoint.ToString()));
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
            if (e.Datagram.IndexOf(NetWorkCommand.SEND_FILE) >= 0)
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
                    receiveFileName = cmd.Split(':')[2];
                    if (!Dispatcher.CheckAccess())
                    {
                        Dispatcher.Invoke(
                                () => StartNewSignature(), System.Windows.Threading.DispatcherPriority.Normal);
                    }
                    else
                    {
                        StartNewSignature();
                    }
                    
                }
                else if (cmd.IndexOf(NetWorkCommand.SIGNATURE_DONE) >= 0)
                {
                    if (!Dispatcher.CheckAccess())
                    {
                        Dispatcher.Invoke(
                                () => SignatureFinished(), System.Windows.Threading.DispatcherPriority.Normal);
                    }
                    else
                    {
                        SignatureFinished();
                    }
                }
                else if (cmd.IndexOf(NetWorkCommand.RECEPTION_EXIT) >= 0)
                {
                    if (!Dispatcher.CheckAccess())
                    {
                        Dispatcher.Invoke(
                            () => SignatureFinished(showThanks:false), System.Windows.Threading.DispatcherPriority.Normal);
                    }
                    else
                    {
                        SignatureFinished(showThanks: false);
                    }
                }
                else
                {
                    Log(string.Format(CultureInfo.InvariantCulture, "{0}:{1}", e.TcpClient.Client.RemoteEndPoint.ToString(), cmd));
                    server.Send(e.TcpClient, NetWorkCommand.OK);
                }
                return;
            }

            if (currentFileSize < receiveFileSize)
            {
                currentFileSize += e.Datagram.Length;
                TempBuffer = CopyToByteArry(TempBuffer, e.Datagram);
                int p = (int)Math.Floor((double)currentFileSize * 100 / receiveFileSize);
                if (!Dispatcher.CheckAccess())
                {
                    Dispatcher.Invoke(
                            () => UpdateReceiveProgress(p), System.Windows.Threading.DispatcherPriority.Normal);
                }
                else
                {
                    UpdateReceiveProgress(p);
                }
            }
            if (currentFileSize == receiveFileSize && receiveFileSize > 0)
            {
                //Save to file
                if (!Directory.Exists(Constants.TempFileFolder))
                {
                    Directory.CreateDirectory(Constants.TempFileFolder);
                }
                string filename = DateTime.Now.ToString("yyyyMMddHHmmss") + receiveFileName.Substring(receiveFileName.LastIndexOf("."), 4);
                FileStream fs = new FileStream(Constants.TempFileFolder + "\\" + filename, FileMode.Create);
                fs.Write(TempBuffer, 0, TempBuffer.Length);
                fs.Close();
                if (!Dispatcher.CheckAccess())
                {
                    Dispatcher.Invoke(
                            () => DisplayBill(filename), System.Windows.Threading.DispatcherPriority.Normal);
                }
                else
                {
                    DisplayBill(filename);
                }
                
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
            if (currentClient != null)
                server.Send(currentClient, string.Format(CultureInfo.InvariantCulture, "{0}:{1}:{2}:{3}:{4}", NetWorkCommand.DRAW, pX, pY, nX, nY));
        }

        private void UpdateLineToReception(Stroke stroke, bool add)
        {
            string s = "";
            foreach (var p in stroke.StylusPoints)
            {
                s += string.Format("{0},{1},{2}:", p.X, p.Y, p.PressureFactor);
            }
            if (currentClient != null)
            {
                int w=0, h=0;           //传递本地inkCanvas尺寸
                w = (int)inkCanvas1.Width;
                h = (int)inkCanvas1.Height;
                int sw, sh;
                sw = (int)System.Windows.SystemParameters.PrimaryScreenWidth;
                sh = (int)System.Windows.SystemParameters.PrimaryScreenHeight;
                String t = add ? NetWorkCommand.STYLUS_ADD : NetWorkCommand.STYLUS_REMOVE;
                server.Send(currentClient, string.Format(CultureInfo.InvariantCulture, "{0},{1},{2},{3},{4}:{5}:{6}", t, w, h, sw, sh, s, NetWorkCommand.STYLUS_END));
            }
        }

        #endregion

       
    }
}

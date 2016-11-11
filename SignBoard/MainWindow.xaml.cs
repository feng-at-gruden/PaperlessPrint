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
using System.Threading.Tasks;
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

        private AsyncTcpServer server;
        private byte[] TempBuffer = new byte[0];
        private long receiveFileSize = 0;
        private long currentFileSize = 0;
        private TcpClient currentClient;

        private bool drawing;
        private int pX = -1;
        private int pY = -1;


        #endregion



        public MainWindow()
        {
            InitializeComponent();
            InitServer();
            InitUI();
        }




        #region UI Events

        private void btnClean_Click(object sender, RoutedEventArgs e)
        {
            CleanSignature();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CleanTempFiles();
        }

        #endregion



        #region Private Functions


        /// <summary>
        /// 
        /// </summary>
        private void InitUI()
        {
            //imgBill.Width = this.Width;
            //imgBill.Height = this.Height;
        }


        private void Log(string log)
        {
            //this.Title = log;
        }

        private void DisplayBill(string filename)
        {
            string path = string.Format("{0}\\{1}\\{2}", Directory.GetCurrentDirectory(), Constants.TempFileFolder, filename);
            BitmapImage b = new BitmapImage();
            b.BeginInit();
            b.UriSource = new Uri(path, UriKind.RelativeOrAbsolute);
            b.EndInit();
            imgBill.Source = b;
        }

        private void CleanSignature()
        {
            //TODO
            if (currentClient != null)
                server.Send(currentClient, NetWorkCommand.CLEAN);
        }

        /// <summary>
        /// 删除临时文件
        /// </summary>
        private void CleanTempFiles()
        {
            //Clean local jpg files
            foreach (string d in Directory.GetFileSystemEntries(Constants.TempFileFolder))
            {
                if (File.Exists(d) && d.IndexOf(".jpg") > 0)
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
                    //UpdateReceiveProgress(0);
                    //CleanSignature();
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

            if (currentFileSize < receiveFileSize)
            {
                currentFileSize += e.Datagram.Length;
                TempBuffer = CopyToByteArry(TempBuffer, e.Datagram);
                int p = (int)Math.Floor((double)currentFileSize * 100 / receiveFileSize);
                //UpdateReceiveProgress(p);
            }
            if (currentFileSize == receiveFileSize && receiveFileSize > 0)
            {
                //Save to file
                if (!Directory.Exists(Constants.TempFileFolder))
                {
                    Directory.CreateDirectory(Constants.TempFileFolder);
                }
                string filename = DateTime.Now.ToString("yyyyMMddHHmmss") + ".jpg";
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

        #endregion

        

        

    }
}

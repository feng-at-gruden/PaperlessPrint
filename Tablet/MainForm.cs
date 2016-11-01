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
using PaperlessPrint.Common;

namespace Tablet
{
    /// <summary>
    /// http://www.cnblogs.com/jamesping/articles/2071932.html
    /// https://github.com/LiveOrDevTrying/TcpAsyncServerClient/blob/master/AsynchronousServer.cs
    /// 开始用UDP 后考虑文件传输改用Tcp异步1对n方式
    /// </summary>
    public partial class MainForm : Form
    {

        # region Fields

        const bool Debug = true;
        Socket listener;

        public static ManualResetEvent allDone = new ManualResetEvent(false);
        
        Socket udpServer;
        String clientIP;

        #endregion


        public MainForm()
        {
            InitializeComponent();
        }


        #region UI Events

        private void MainForm_Load(object sender, EventArgs e)
        {
            InitServer();
        }



        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            //关闭Soket
            listener.Close();
        }


        private void btnTest_Click(object sender, EventArgs e)
        {
            if(txtTestContent.Text != null)
                SendMsg(txtTestContent.Text);
        }

        #endregion




        #region Private Functions

        /// <summary>
        /// 接收回调函数
        /// </summary>
        /// <param name="ar"></param>
        


        private void ReciveMsg()
        {
            while (true)
            {
                EndPoint point = new IPEndPoint(IPAddress.Any, 0);      //用来保存发送方的ip和端口号
                byte[] buffer = new byte[1024];
                int length = udpServer.ReceiveFrom(buffer, ref point);  //接收数据报
                string message = Encoding.UTF8.GetString(buffer, 0, length);
                Console.WriteLine(point.ToString() + message);
                Log("收到(" + point.ToString() + "):" + message);
            }
        }

        private void SendMsg(String msg)
        {
            EndPoint point = new IPEndPoint(IPAddress.Parse("169.254.202.67"), 6000);         //目标IP
            //while (true)
            {
                //string msg = Console.ReadLine();
                udpServer.SendTo(Encoding.UTF8.GetBytes(msg), point);
            }
        }

        private void Log(String txt)
        {
            toolStripStatusLabel1.Text = txt;
            txtLog.Text += DateTime.Now.ToString("hh:mm:ss") + " " + txt + "\r\n";
        }

        #endregion



        #region Tcp Server

        private void InitServer()
        {
            //打开端口 监听
            listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), Constants.TcpPort); //TODO
            listener.Bind(localEndPoint);
            listener.Listen(Constants.MaxClients);
            listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);
            Log("网络启动:" + localEndPoint.Address.ToString() + ":" +Constants.TcpPort);
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                TcpListener l = (TcpListener)ar.AsyncState;
                Socket client = l.EndAcceptSocket(ar);
            }
            catch { }
        }

        #endregion




    }


    public class StateObject
    {
        // Client  socket.
        public Socket WorkSocket = null;
        // Size of receive buffer.
        public const int BufferSize = 1024;
        // Receive buffer.
        public byte[] Buffer = new byte[BufferSize];
        // Received data string.
        public StringBuilder Sb = new StringBuilder();
    }
}

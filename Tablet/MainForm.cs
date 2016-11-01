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
using Common;
using Common.Utiles;
using Common.TCPServer;


namespace Tablet
{
    /// <summary>
    /// http://www.cnblogs.com/jamesping/articles/2071932.html
    /// https://github.com/LiveOrDevTrying/TcpAsyncServerClient/blob/master/AsynchronousServer.cs
    /// https://github.com/gaochundong/Gimela/blob/master/src/Foundation/Net/Gimela.Net.Sockets/TCP
    /// 开始用UDP 后考虑文件传输改用Tcp异步1对n方式
    /// </summary>
    public partial class MainForm : Form
    {

        # region Fields

        private const bool Debug = true;
        private AsyncTcpServer server;
        

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
        }



        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            //关闭Soket
            server.Stop();
            server.Dispose();
        }


        private void btnTest_Click(object sender, EventArgs e)
        {
            if (txtTestContent.Text != null)
            {
                server.SendAll(txtTestContent.Text);
            }
        }

        #endregion




        #region Private Functions

       

        private void Log(String txt)
        {
            toolStripStatusLabel1.Text = txt;
            txtLog.Text += DateTime.Now.ToString("HH:mm:ss") + " " + txt + "\r\n";
        }

        #endregion



        #region TCP Server

        

        private void InitServer()
        {
            server = new AsyncTcpServer(Constants.TcpPort);
            server.Encoding = Encoding.UTF8;
            server.ClientConnected += new EventHandler<TcpClientConnectedEventArgs>(server_ClientConnected);
            server.ClientDisconnected += new EventHandler<TcpClientDisconnectedEventArgs>(server_ClientDisconnected);
            server.PlaintextReceived += new EventHandler<TcpDatagramReceivedEventArgs<string>>(server_PlaintextReceived);
            server.Start();
            Log("网络启动:" + Constants.TcpPort);
        }

        private void server_ClientConnected(object sender, TcpClientConnectedEventArgs e)
        {
            Log(string.Format(CultureInfo.InvariantCulture, "TCP client {0} has connected.", e.TcpClient.Client.RemoteEndPoint.ToString()));
        }

        private void server_ClientDisconnected(object sender, TcpClientDisconnectedEventArgs e)
        {
            Log(string.Format(CultureInfo.InvariantCulture, "TCP client {0} has disconnected.", e.TcpClient.Client.RemoteEndPoint.ToString()));
        }

        private void server_PlaintextReceived(object sender, TcpDatagramReceivedEventArgs<string> e)
        {
            if (e.Datagram != "Received")
            {
                Console.Write(string.Format("Client : {0} --> ", e.TcpClient.Client.RemoteEndPoint.ToString()));
                Console.WriteLine(string.Format("{0}", e.Datagram));
                server.Send(e.TcpClient, "Server has received you text : " + e.Datagram);
            }
        }

        #endregion




    }

}

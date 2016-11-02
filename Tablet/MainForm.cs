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

            CleanTempFiles();
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
            if (Constants.DEBUG)
            {
                toolStripStatusLabel1.Text = txt;
                txtLog.Text += DateTime.Now.ToString("HH:mm:ss") + " " + txt + "\r\n";
                txtLog.SelectionStart = txtLog.Text.Length;
                txtLog.ScrollToCaret();
            }
        }

        private void CleanTempFiles()
        {
            //Clean local jpg files
            foreach(string d in Directory.GetFileSystemEntries("."))
            {
                if(File.Exists(d) && d.IndexOf(".jpg")>0)
                {
                    File.Delete(d);
                }
            }

        }

        #endregion



        #region TCP Server

        

        private void InitServer()
        {
            server = new AsyncTcpServer(Constants.TabletPort);
            server.Encoding = Encoding.UTF8;
            server.ClientConnected += new EventHandler<TcpClientConnectedEventArgs>(ClientConnected);
            server.ClientDisconnected += new EventHandler<TcpClientDisconnectedEventArgs>(ClientDisconnected);
            server.PlaintextReceived += new EventHandler<TcpDatagramReceivedEventArgs<string>>(PlainTextReceived);
            server.DatagramReceived += new EventHandler<TcpDatagramReceivedEventArgs<byte[]>>(DatagramReceived);
            server.Start();
            Log("网络启动:" + NetworkHelper.GetLocalIP() + ":" + Constants.TabletPort);
        }

        private void ClientConnected(object sender, TcpClientConnectedEventArgs e)
        {
            Log(string.Format(CultureInfo.InvariantCulture, "TCP client {0} has connected.", e.TcpClient.Client.RemoteEndPoint.ToString()));
        }

        private void ClientDisconnected(object sender, TcpClientDisconnectedEventArgs e)
        {
            Log(string.Format(CultureInfo.InvariantCulture, "TCP client {0} has disconnected.", e.TcpClient.Client.RemoteEndPoint.ToString()));
        }

        private void PlainTextReceived(object sender, TcpDatagramReceivedEventArgs<string> e)
        {
            Log(string.Format(CultureInfo.InvariantCulture, "Received:{0}", e.Datagram));
            if (e.Datagram != "Received")
            {
                Console.Write(string.Format("Client : {0} --> ", e.TcpClient.Client.RemoteEndPoint.ToString()));
                Console.WriteLine(string.Format("{0}", e.Datagram));
                server.Send(e.TcpClient, "OK");
            }
        }

        private void DatagramReceived(object sender, TcpDatagramReceivedEventArgs<byte[]> e)
        {
            String filename = DateTime.Now.ToString("yyyyMMddHHmmss") + ".jpg";
            Log(string.Format(CultureInfo.InvariantCulture, "Received:{0}", filename));
            FileStream fs = new FileStream(filename, FileMode.Create);
            fs.Write(e.Datagram, 0, e.Datagram.Length);
            fs.Close();

            server.Send(e.TcpClient, "File saved");
        }

        #endregion




    }

}

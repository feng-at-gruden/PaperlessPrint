using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Globalization;

namespace Common.TCPServer
{
    internal class TcpClientState
    {
        /// <summary>
        /// Constructor for a new Client
        /// </summary>
        /// <param name="tcpClient">The TCP client</param>
        /// <param name="buffer">The byte array buffer</param>
        public TcpClientState(TcpClient tcpClient, byte[] buffer)
        {
            if (tcpClient == null)
                throw new ArgumentNullException("tcpClient");
            if (buffer == null)
                throw new ArgumentNullException("buffer");

            this.TcpClient = tcpClient;
            this.Buffer = buffer;
        }

        /// <summary>
        /// Gets the TCP Client
        /// </summary>
        public TcpClient TcpClient { get; private set; }

        /// <summary>
        /// Gets the Buffer.
        /// </summary>
        public byte[] Buffer { get; private set; }

        /// <summary>
        /// Gets the network stream
        /// </summary>
        public NetworkStream NetworkStream
        {
            get { return TcpClient.GetStream(); }
        }
    }
}

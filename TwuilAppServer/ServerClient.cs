using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using TwuilAppLib.Data;

namespace TwuilAppServer
{
    public class ServerClient
    {

        public string UserName { get; private set; }

        private Server server;

        private TcpClient client;
        private Stream stream;
        private byte[] receiveBuffer;

        public bool IsActive { get; private set; }

        public ServerClient(TcpClient client, Server server)
        {

        }

        public void Send(DAbstract data)
        {

        }

        private void OnBytesReceived(IAsyncResult result)
        {

        }

        private void OnDataReceived(DAbstract data)
        {

        }

    }
}

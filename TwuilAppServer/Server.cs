using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using TwuilAppLib.Data;

namespace TwuilAppServer
{
    public class Server
    {

        private TcpListener listener;

        private List<ServerClient> clientList = new List<ServerClient>();

        public Server(ushort port)
        {
            this.listener = new TcpListener(IPAddress.Loopback, port);
            this.listener.Start();
            this.listener.BeginAcceptTcpClient(this.OnClientAccepted, null);
        }

        public void OnClientAccepted(IAsyncResult result)
        {
            TcpClient client = this.listener.EndAcceptTcpClient(result);

            if(client != null && client.Connected)
            {
                this.clientList.Add(new ServerClient(client, this));
            }
        }

        public void Broadcast(DAbstract data)
        {

        }

        public void SendToClient(ServerClient receiver, DAbstract data)
        {

        }

        public void SendToClients(List<ServerClient> receiverList, DAbstract data)
        {

        }
        
    }
}

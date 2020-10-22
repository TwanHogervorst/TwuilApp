using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using TwuilAppLib.Data;

namespace TwuilAppServer
{
    public class Server
    {

        private TcpListener listener;

        public Server()
        {

        }

        public void OnClientAccepted(IAsyncResult result)
        {

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

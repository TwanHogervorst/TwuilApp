using System;
using System.Collections.Generic;
using System.Text;
using TwuilAppLib.Interface;
using TwuilAppServer.Core;

namespace TwuilAppServer.States
{
    class ServerClientActiveState : IServerClientState
    {

        public string Username { get; }

        private ServerClient context;
        private Server server;

        public ServerClientActiveState(ServerClient context, Server server, string Username)
        {
            this.context = context;
            this.server = server;
        }

        public void SendPrivateMessage(string receiver, string message)
        {

        }

    }
}

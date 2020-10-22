using System;
using System.Collections.Generic;
using System.Text;
using TwuilAppLib.Interface;

namespace TwuilAppServer.States
{
    class ServerClientActiveState
    {

        private IStateContext<IServerClientState> context;
        private Server server;

        public ServerClientActiveState(IStateContext<IServerClientState> context, Server server)
        {
            this.context = context;
            this.server = server;
        }

        public void SendPrivateMessage(string username, string message)
        {

        }

    }
}

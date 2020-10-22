using System;
using System.Collections.Generic;
using System.Text;
using TwuilAppLib.Interface;

namespace TwuilAppServer.States
{
    public class ServerClientIdleState : IServerClientState
    {

        private IStateContext<IServerClientState> context;

        public ServerClientIdleState(IStateContext<IServerClientState> context)
        {
            this.context = context;
        }

        public bool Login(string username, string password)
        {
            bool result = false;



            return result;
        }

    }
}

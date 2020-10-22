using System;
using System.Collections.Generic;
using System.Text;
using TwuilAppLib.Data;
using TwuilAppLib.Interface;

namespace TwuilAppClient.States
{
    class ClientIdleState : IClientState
    {
        private Client context;

        public ClientIdleState(Client client)
        {
            this.context = client;
        }

        public void Login(string username, string password)
        {
            this.context.Send(new DLoginPacket
            {
                username = username,
                password = password
            });
        }
    }
}

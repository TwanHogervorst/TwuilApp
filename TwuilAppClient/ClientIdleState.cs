using System;
using System.Collections.Generic;
using System.Text;
using TwuilAppLib.Data;
using TwuilAppLib.Interface;

namespace TwuilAppClient
{
    class ClientIdleState
    {
        private Client context;

        public ClientIdleState(Client client)
        {
            this.context = client;
        }

        public bool Login(string username, string password)
        {
            this.context.Send(new DLoginPacket
            {
                username = username,
                password = password
            });
            return true;
        }
    }
}

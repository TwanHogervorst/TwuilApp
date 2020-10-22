using System;
using System.Collections.Generic;
using System.Text;
using TwuilAppClient.Core;
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

        public void SignUp(string username, string password)
        {
            this.context.Send(new DSignUpPacket
            {
                username = username,
                password = password
            });
        }
    }
}

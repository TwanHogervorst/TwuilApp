using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using TwuilAppClient.Core;

namespace TwuilAppClient.States
{
    class ClientActiveState : IClientState
    {

        private Client context;

        public ClientActiveState(Client context)
        {
            this.context = context;
        }

        public void SendPrivateMessage(string receiver, string message)
        {

        }

    }
}

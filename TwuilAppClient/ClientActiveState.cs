using System;
using System.Collections.Generic;
using System.Text;
using TwuilAppLib.Data;

namespace TwuilAppClient
{
    class ClientActiveState
    {
        private Client context;

        public ClientActiveState(Client client)
        {
            this.context = client;
        }

        public void SendPrivateMessage(string message)
        {
            this.context.Send(new DMessagePacket
            { 
                sender = this.context
                message = message 
            });
          }
    }
}

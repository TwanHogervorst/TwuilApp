using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using TwuilAppClient.Core;
using TwuilAppLib.Data;

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
            this.context.Send(new DPrivateMessagePacket
            {
                sender = this.context.Username,
                receiver = receiver,
                message = message
            });
        }

        public void CreateGroup(string groupName, List<string> usersToAdd, string welcomeMessage)
        {
            this.context.Send(new DGroupCreatePacket
            {
                groupName = groupName,
                usersToAdd = usersToAdd,
                welcomeMessage = welcomeMessage
            });
        }

    }
}

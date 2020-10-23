using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Text;
using TwuilAppLib.Data;
using TwuilAppLib.Interface;
using TwuilAppServer.Core;

namespace TwuilAppServer.States
{
    class ServerClientActiveState : IServerClientState
    {

        public string Username { get; }

        private ServerClient context;
        private Server server;

        public ServerClientActiveState(ServerClient context, Server server, string username)
        {
            this.context = context;
            this.server = server;
            this.Username = username;
        }

        public void SendPrivateMessage(string receiver, string message)
        {
            DPrivateMessageSendResponsePacket response = new DPrivateMessageSendResponsePacket();

            (bool, string) result = this.server.ChatManager.SendPrivate(this.Username, receiver, message);

            response.status = result.Item1 ? ResponsePacketStatus.Success : ResponsePacketStatus.Error;
            response.errorMessage = result.Item2;

            if (result.Item1) Console.WriteLine($"Message from {this.Username} to {receiver}: {message}");

            this.context.Send(response);
        }

        public void CreateGroup(string groupName, List<string> usersToAdd, string welcomeMessage)
        {
            DGroupCreateResponsePacket response = new DGroupCreateResponsePacket();

            (bool, string) result = this.server.ChatManager.CreateGroup(groupName, usersToAdd, welcomeMessage);

            response.status = result.Item1 ? ResponsePacketStatus.Success : ResponsePacketStatus.Error;
            response.errorMessage = result.Item2;

            if (result.Item1) Console.WriteLine($"Group {groupName} created");

            this.context.Send(response);
        }

        public void SendGroupMessage(string groupName, string message)
        {
            DGroupMessageSendResponsePacket response = new DGroupMessageSendResponsePacket();

            (bool, string) result = this.server.ChatManager.SendGroupMessage(this.Username, groupName, message);

            response.status = result.Item1 ? ResponsePacketStatus.Success : ResponsePacketStatus.Error;
            response.errorMessage = result.Item2;

            if (result.Item1) Console.WriteLine($"Group message from {this.Username} to {groupName}: {message}");

            this.context.Send(response);
        }

    }
}

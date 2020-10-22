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
            DPrivateMessageSendResponse response = new DPrivateMessageSendResponse();

            try
            {
                if (this.server.ClientManager.TryGetClientByUsername(receiver, out ServerClient clientReceiver))
                {
                    if (clientReceiver.Connected)
                    {
                        clientReceiver.Send(new DPrivateMessagePacket
                        {
                            sender = this.Username,
                            receiver = receiver,
                            message = message
                        });

                        response.status = ResponsePacketStatus.Success;
                    }
                }
                else
                {
                    response.status = ResponsePacketStatus.Error;
                    response.errorMessage = "Receiver not found!";
                }
            }
            catch (Exception ex)
            {
                response.status = ResponsePacketStatus.Error;
                response.errorMessage = "Internal server error";

                Console.WriteLine($"{ex.GetType().Name}: {ex.Message}");
            }

            this.context.Send(response);
        }

    }
}

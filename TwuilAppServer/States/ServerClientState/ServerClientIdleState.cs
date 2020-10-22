using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using TwuilAppLib.Data;
using TwuilAppLib.Interface;
using TwuilAppServer.Core;

namespace TwuilAppServer.States
{
    public class ServerClientIdleState : IServerClientState
    {

        public string Username { get; private set; } = null;

        private ServerClient context;

        public ServerClientIdleState(ServerClient context)
        {
            this.context = context;
        }

        public void Login(string username, string password)
        {
            DLoginResponsePacket response = new DLoginResponsePacket();

            try
            {
                if (username == password)
                {
                    this.Username = username;
                    this.context.SetState(typeof(ServerClientActiveState));

                    Console.WriteLine($"Client {this.Username} logged in!");

                    response.status = ResponsePacketStatus.Success;
                    response.username = username;
                }
                else
                {
                    response.status = ResponsePacketStatus.Error;
                    response.errorMessage = "Username and/or password is incorrect.";
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

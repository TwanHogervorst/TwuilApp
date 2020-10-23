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
        private Server server;

        public ServerClientIdleState(ServerClient context, Server server)
        {
            this.context = context;
            this.server = server;
        }

        public void Login(string username, string password)
        {
            DLoginResponsePacket response = new DLoginResponsePacket();

            try
            {
                (bool, string) authenticationResult = this.server.CredentialsManager.Authenticate(username, password);
                if (authenticationResult.Item1)
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
                    response.errorMessage = authenticationResult.Item2 ?? "Something went wrong while authenticating.";
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

        public void SignUp(string username, string password)
        {
            DLoginResponsePacket response = new DLoginResponsePacket();

            try
            {
                (bool, string) authenticationResult = this.server.CredentialsManager.CreateAccount(username, password);
                if (authenticationResult.Item1)
                {
                    this.Username = username;
                    this.context.SetState(typeof(ServerClientActiveState));

                    Console.WriteLine($"Client {this.Username} signed up and logged in!");

                    response.status = ResponsePacketStatus.Success;
                    response.username = username;
                }
                else
                {
                    response.status = ResponsePacketStatus.Error;
                    response.errorMessage = authenticationResult.Item2 ?? "Something went wrong while signing up.";
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

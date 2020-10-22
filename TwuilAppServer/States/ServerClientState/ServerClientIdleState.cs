using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using TwuilAppLib.Data;
using TwuilAppLib.Interface;

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

        public bool Login(string username, string password)
        {
            bool result = false;

            // Do check
            result = username == password;

            DLoginResponsePacket response = new DLoginResponsePacket();

            if(result)
            {
                this.Username = username;
                this.context.SetState(typeof(ServerClientActiveState));

                response.status = ResponsePacketStatus.Success;
            }
            else
            {
                response.status = ResponsePacketStatus.Error;
                response.errorMessage = "Username and/or password is incorrect.";
            }

            return result;
        }

    }
}

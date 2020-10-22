using System;
using System.Collections.Generic;
using System.Text;

namespace TwuilAppServer.States
{
    public interface IServerClientState
    {

        string Username { get { return null; } }

        void Login(string username, string password)
        {
        }

        void SendPrivateMessage(string receiver, string message)
        {

        }

    }
}

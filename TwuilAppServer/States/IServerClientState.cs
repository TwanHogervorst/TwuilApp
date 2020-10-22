using System;
using System.Collections.Generic;
using System.Text;

namespace TwuilAppServer.States
{
    public interface IServerClientState
    {

        string Username { get { return null; } }

        bool Login(string username, string password)
        {
            return false;
        }

        void SendPrivateMessage(string username, string message)
        {

        }

    }
}

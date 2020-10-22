using System;
using System.Collections.Generic;
using System.Text;

namespace TwuilAppServer.States
{
    public interface IServerClientState
    {

        bool Login(string username, string password)
        {
            return false;
        }

        void SendPrivateMessage(string username, string message)
        {

        }

    }
}

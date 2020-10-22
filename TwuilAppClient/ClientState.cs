using System;
using System.Collections.Generic;
using System.Text;

namespace TwuilAppClient
{
    public interface ClientState
    {

        bool Login(string username, string password)
        {
            return false;
        }

        void SenPrivateMessage(string username, string message)
        {

        }

    }
}

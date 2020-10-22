using System;
using System.Collections.Generic;
using System.Text;

namespace TwuilAppClient
{
    public interface IClientState
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

using System;
using System.Collections.Generic;
using System.Text;

namespace TwuilAppClient.States
{
    public interface IClientState
    {

        void Login(string username, string password)
        {
        }

        void SignUp(string username, string password)
        {
        }

        void SendPrivateMessage(string receiver, string message)
        {
        }

        void CreateGroup(string groupName, List<string> usersToAdd, string welcomeMessage)
        {
        }

        void SendGroupMessage(string groupName, string message)
        {
        }

    }
}

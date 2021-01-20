using System;
using System.Collections.Generic;
using System.Text;

namespace TwuilAppServer.Interface
{
    public interface IChatManager
    {

        public (bool, string) SendPrivate(string from, string to, string message);

        public (bool, string) CreateGroup(string groupName, List<string> users, string welcomeMessage);

        public (bool, string) SendGroupMessage(string from, string toGroup, string message);

    }
}

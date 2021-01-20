using System;
using System.Collections.Generic;
using System.Text;

namespace TwuilAppServer.Interface
{
    public interface ICredentialsManager
    {

        public (bool, string) Authenticate(string username, string password);

        public bool UserExists(string username);

        public (bool, string) CreateAccount(string username, string password);

    }
}

using System;
using System.Collections.Generic;
using System.Text;
using TwuilAppServer.Core;

namespace TwuilAppServer.Interface
{
    public interface IServerClientManager
    {

        List<ServerClient> ClientList { get; }

        bool Contains(ServerClient client);

        bool UserIsOnline(string username);

        void Add(ServerClient client);

        void Remove(ServerClient client);

        bool TryGetClientByUsername(string username, out ServerClient client);

    }
}

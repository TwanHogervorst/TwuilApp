using System;
using System.Collections.Generic;
using System.Text;
using TwuilAppLib.Data;
using TwuilAppServer.Core;

namespace TwuilAppServer.Interface
{
    public interface IServer
    {
        IServerClientManager ServerClientManager { get; }
        ICredentialsManager CredentialsManager { get; }
        IChatManager ChatManager { get; }

        void SendToClients(List<ServerClient> receiverList, DAbstract data);

    }
}

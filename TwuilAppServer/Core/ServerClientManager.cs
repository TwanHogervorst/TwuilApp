using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TwuilAppServer.Interface;

namespace TwuilAppServer.Core
{
    public class ServerClientManager : IServerClientManager
    {

        private List<ServerClient> clientList = new List<ServerClient>();
        public List<ServerClient> ClientList => clientList.ToList();

        public bool Contains(ServerClient client) => this.clientList.Contains(client);

        public bool UserIsOnline(string username) => this.clientList.FirstOrDefault(client => client.Username == username) != null;

        public void Add(ServerClient client)
        {
            if (!this.clientList.Contains(client)) this.clientList.Add(client);
        }

        public void Remove(ServerClient client) => this.clientList.Remove(client);

        public bool TryGetClientByUsername(string username, out ServerClient client)
        {
            bool result = false;

            client = this.ClientList.Where(c => c.IsActive).FirstOrDefault(c => c.Username == username);
            result = client != null;

            return result;
        }
    }
}

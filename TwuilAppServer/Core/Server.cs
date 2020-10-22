﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TwuilAppLib.Data;

namespace TwuilAppServer.Core
{
    public class Server : IDisposable
    {
        public ServerClientManager ClientManager { get; }
        public CredentialsManager CredentialsManager { get; }

        private TcpListener listener;

        private bool disposed;

        public Server(ushort port)
        {
            this.ClientManager = new ServerClientManager();
            this.CredentialsManager = new CredentialsManager(this);

            this.listener = new TcpListener(IPAddress.Loopback, port);
            this.listener.Start();
            this.listener.BeginAcceptTcpClient(this.OnClientAccepted, null);
        }

        public void OnClientAccepted(IAsyncResult result)
        {
            TcpClient client = this.listener.EndAcceptTcpClient(result);

            if(client != null && client.Connected)
            {
                this.ClientManager.Add(new ServerClient(client, this));
            }

            this.listener.BeginAcceptTcpClient(this.OnClientAccepted, null);
        }

        public bool UserIsOnline(string username) => this.ClientManager.Contains(username);

        public bool UserExists(string username) => this.CredentialsManager.UserExists(username);

        public void Broadcast(DAbstract data) => this.SendToClients(this.ClientManager.ClientList, data);

        public void SendToClients(List<ServerClient> receiverList, DAbstract data)
        {
            foreach (ServerClient receiver in receiverList) this.SendToClient(receiver, data);
        }

        private void SendToClient(ServerClient receiver, DAbstract data)
        {
            if (this.disposed) throw new ObjectDisposedException(nameof(Server)); 
            if (receiver != null && receiver.Connected) receiver.Send(data);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    this.Broadcast(new DServerClosingPacket
                    {
                        reason = "The server has been closed by the admin."
                    });
                }

                try
                {
                    foreach (ServerClient client in this.ClientManager.ClientList)
                        client.Disconnect();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{ex.GetType().Name}: {ex.Message}");
                }

                try
                {
                    this.listener.Stop();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{ex.GetType().Name}: {ex.Message}");
                }

                disposed = true;
            }
        }

        // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~Server()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
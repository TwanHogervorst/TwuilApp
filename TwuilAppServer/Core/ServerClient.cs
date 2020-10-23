using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using TwuilAppLib.Core;
using TwuilAppLib.Data;
using TwuilAppLib.Interface;
using TwuilAppServer.States;

namespace TwuilAppServer.Core
{
    public class ServerClient : IStateContext<IServerClientState>
    {

        public string Username => this.State.Username;
        public bool IsActive => this.State is ServerClientActiveState;
        public bool Connected => this.client.Connected && this.stream.CanRead && this.stream.CanWrite;

        public IServerClientState State { get; private set; }

        private Server server;

        private TcpClient client;
        private Stream stream;

        private int receivedBytes;
        private byte[] receiveBuffer;
        private bool receivePacketHeader;

        public ServerClient(TcpClient client, Server server)
        {
            this.client = client;
            this.server = server;

            this.stream = this.client.GetStream();

            this.State = new ServerClientIdleState(this, this.server);

            this.receivedBytes = 0;
            this.receiveBuffer = new byte[6];
            this.receivePacketHeader = true;

            this.stream.BeginRead(receiveBuffer, 0, receiveBuffer.Length, this.OnBytesReceived, null);
        }

        public void Disconnect()
        {
            try
            {
                if(this.client != null && this.client.Connected)
                {
                    this.client.Close();
                    this.client.Dispose();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.GetType().Name}: {ex.Message}");
            }
        }

        public void Send(DAbstract data)
        {
            DNetworkPacket<DAbstract> networkPacket = new DNetworkPacket<DAbstract>
            {
                type = data.GetType().Name,
                data = data
            };

            byte[] buffer = Encoding.ASCII.GetBytes(networkPacket.ToJson());
            this.stream.Write(new byte[]
            {
                0x69
            });
            this.stream.Write(BitConverter.GetBytes(buffer.Length));
            this.stream.Write(new byte[]{
                new PacketFlags().Result()
            });
            this.stream.Write(buffer);
            this.stream.Write(new byte[]
            {
                Utility.CalculateChecksum(buffer)
            });

            this.stream.Flush();
        }

        public void SetState(IServerClientState newState)
        {
            this.State = newState;
        }

        public void SetState(Type newStateType)
        {
            if(typeof(IServerClientState).IsAssignableFrom(newStateType))
            {
                if(newStateType == typeof(ServerClientActiveState))
                {
                    this.State = new ServerClientActiveState(this, this.server, this.State.Username);
                }
                else
                {
                    this.State = Activator.CreateInstance(newStateType) as IServerClientState;
                }
            }
        }

        private void OnBytesReceived(IAsyncResult result)
        {
            try
            {
                this.receivedBytes += this.stream.EndRead(result);
            }
            catch (Exception)
            {
                // disconnected
                Console.WriteLine($"Client {this.State.Username ?? this.client.Client.RemoteEndPoint.ToString()} disconnected!");
                this.server.ServerClientManager.Remove(this);
                this.client.Dispose();
                return;
            }

            if(this.receivedBytes < this.receiveBuffer.Length)
            {
                this.stream.BeginRead(this.receiveBuffer, receivedBytes, this.receiveBuffer.Length - this.receivedBytes, this.OnBytesReceived, null);
                return;
            }

            if(this.receivePacketHeader && this.receiveBuffer.Length >= 6)
            {
                byte protocolId = this.receiveBuffer[0];
                int length = BitConverter.ToInt32(this.receiveBuffer.Skip(1).Take(4).ToArray());
                PacketFlags flags = new PacketFlags(this.receiveBuffer[5]);

                if(protocolId == 0x69)
                {
                    this.receivedBytes = 0;
                    this.receiveBuffer = new byte[length + 1];
                    this.receivePacketHeader = false;

                    this.stream.BeginRead(this.receiveBuffer, 0, receiveBuffer.Length, this.OnBytesReceived, null);
                }
            }
            else
            {
                byte[] messageBytes = this.receiveBuffer.Take(this.receiveBuffer.Length - 1).ToArray();
                byte checksum = this.receiveBuffer.Last();

                if(checksum == Utility.CalculateChecksum(messageBytes))
                {
                    try
                    {
                        this.OnDataReceived(JsonConvert.DeserializeObject<DNetworkPacket>(Encoding.UTF8.GetString(messageBytes)));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"{ex.GetType().Name}: {ex.Message}");
                    }

                    this.receivedBytes = 0;
                    this.receiveBuffer = new byte[6];
                    this.receivePacketHeader = true;

                    if(this.Connected) this.stream.BeginRead(receiveBuffer, 0, receiveBuffer.Length, this.OnBytesReceived, null);
                }
                else
                {
                    Console.WriteLine("Got packet, but checksum is invalid");
                }
            }

        }

        private void OnDataReceived(DNetworkPacket packetRaw)
        {
            Console.WriteLine($"Got packet of type: {packetRaw.type}");

            switch(packetRaw.type)
            {
                // Authentication
                case nameof(DLoginPacket):
                    {
                        DNetworkPacket<DLoginPacket> packet = packetRaw.DataAsType<DLoginPacket>();

                        this.Login(packet.data.username, packet.data.password);
                    }
                    break;
                case nameof(DSignUpPacket):
                    {
                        DNetworkPacket<DSignUpPacket> packet = packetRaw.DataAsType<DSignUpPacket>();

                        this.SignUp(packet.data.username, packet.data.password);
                    }
                    break;
                // Private Message
                case nameof(DPrivateMessagePacket):
                    {
                        DNetworkPacket<DPrivateMessagePacket> packet = packetRaw.DataAsType<DPrivateMessagePacket>();

                        this.SendPrivateMessage(packet.data.receiver, packet.data.message);
                    }
                    break;
                // Group
                case nameof(DGroupCreatePacket):
                    {
                        DNetworkPacket<DGroupCreatePacket> packet = packetRaw.DataAsType<DGroupCreatePacket>();

                        this.CreateGroup(packet.data.groupName, packet.data.usersToAdd, packet.data.welcomeMessage);
                    }
                    break;
                case nameof(DGroupChatMessagePacket):
                    {
                        DNetworkPacket<DGroupChatMessagePacket> packet = packetRaw.DataAsType<DGroupChatMessagePacket>();

                        this.SendGroupMessage(packet.data.groupName, packet.data.message);
                    }
                    break;
                // Disconnect
                case nameof(DClientDisconnectPacket):
                    {
                        this.server.ServerClientManager.Remove(this);

                        Console.WriteLine($"Client {this.State.Username ?? this.client.Client.RemoteEndPoint.ToString()} disconnected!");

                        this.client.Close();
                        this.client.Dispose();
                    }
                    break;
            }
        }

        private void Login(string username, string password)
        {
            this.State.Login(username, password);
        }

        private void SignUp(string username, string password)
        {
            this.State.SignUp(username, password);
        }

        private void SendPrivateMessage(string receiver, string message)
        {
            this.State.SendPrivateMessage(receiver, message);
        }

        private void CreateGroup(string groupName, List<string> usersToAdd, string welcomeMessage)
        {
            this.State.CreateGroup(groupName, usersToAdd, welcomeMessage);
        }

        private void SendGroupMessage(string groupName, string message)
        {
            this.State.SendGroupMessage(groupName, message);
        }

    }
}

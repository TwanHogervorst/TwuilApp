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

            this.State = new ServerClientIdleState(this);

            this.receivedBytes = 0;
            this.receiveBuffer = new byte[6];
            this.receivePacketHeader = true;

            this.stream.BeginRead(receiveBuffer, 0, receiveBuffer.Length, this.OnBytesReceived, null);
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

            this.stream.FlushAsync();
        }

        public void SetState(IServerClientState newState)
        {
            this.State = newState;
        }

        public void SetState(Type newStateType)
        {
            if(newStateType.IsAssignableFrom(typeof(IServerClientState)))
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
            this.receivedBytes += this.stream.EndRead(result);

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

                    this.stream.BeginRead(receiveBuffer, 0, receiveBuffer.Length, this.OnBytesReceived, null);
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
                case nameof(DLoginPacket):
                    {
                        DNetworkPacket<DLoginPacket> packet = packetRaw.DataAsType<DLoginPacket>();
                        this.Login(packet.data.username, packet.data.password);
                    }
                    break;
                case nameof(DMessagePacket):
                    {
                        DNetworkPacket<DMessagePacket> packet = packetRaw.DataAsType<DMessagePacket>();
                        Console.WriteLine($"Bericht van {packet.data.sender}: {packet.data.message}");
                    }
                    break;
            }
        }

        private void Login(string username, string password)
        {
            this.State.Login(username, password);
        }
        
    }
}

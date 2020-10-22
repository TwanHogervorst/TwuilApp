using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using TwuilAppClient.States;
using TwuilAppLib.Core;
using TwuilAppLib.Data;
using TwuilAppLib.Interface;

namespace TwuilAppClient.Core
{
    public class Client : IStateContext<IClientState>
    {
        public string Username { get; private set; }
        public bool IsActive => this.State is ClientActiveState;
        public bool Connected => this.client.Connected && this.stream.CanRead && this.stream.CanWrite;

        public IClientState State { get; private set; }

        public event PrivateMessageReceived OnPrivateMessageReceived;
        public event LoginResponseReceived OnLoginResponseReceived;
        public event ServerClosingReceived OnServerClosing;
        public event PrivateMessageSendResponseReceived OnPrivateMessageSendResponse;

        private TcpClient client;
        private Stream stream;

        private int receivedBytes;
        private byte[] receiveBuffer;
        private bool receivePacketHeader;

        public Client(string ip, ushort port)
        {
            this.client = new TcpClient(ip, port);
            this.stream = this.client.GetStream();

            this.State = new ClientIdleState(this);

            this.receivedBytes = 0;
            this.receiveBuffer = new byte[6];
            this.receivePacketHeader = true;

            this.stream.BeginRead(receiveBuffer, 0, receiveBuffer.Length, this.OnBytesReceived, null);
        }

        internal void Send(DAbstract data)
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

        private void OnBytesReceived(IAsyncResult result)
        {
            try
            {
                this.receivedBytes += this.stream.EndRead(result);
            }
            catch (Exception ex)
            {
                // server forcebly closed
                this.OnServerClosing?.Invoke(this, "Server was forcebly closed");
                this.client.Close();
                this.client.Dispose();
                return;
            }

            if (this.receivedBytes < this.receiveBuffer.Length)
            {
                this.stream.BeginRead(this.receiveBuffer, receivedBytes, this.receiveBuffer.Length - this.receivedBytes, this.OnBytesReceived, null);
                return;
            }

            if (this.receivePacketHeader && this.receiveBuffer.Length >= 6)
            {
                byte protocolId = this.receiveBuffer[0];
                int length = BitConverter.ToInt32(this.receiveBuffer.Skip(1).Take(4).ToArray());
                PacketFlags flags = new PacketFlags(this.receiveBuffer[5]);

                if (protocolId == 0x69)
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

                if (checksum == Utility.CalculateChecksum(messageBytes))
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

            switch (packetRaw.type)
            {
                case nameof(DLoginResponsePacket):
                    {
                        DNetworkPacket<DLoginResponsePacket> packet = packetRaw.DataAsType<DLoginResponsePacket>();

                        if(packet.data.status == ResponsePacketStatus.Success)
                        {
                            this.Username = packet.data.username;
                        }

                        this.SetState(new ClientActiveState(this));

                        this.OnLoginResponseReceived?.Invoke(this, packet.data.status == ResponsePacketStatus.Success, packet.data.errorMessage);
                    }
                    break;
                case nameof(DPrivateMessagePacket):
                    {
                        DNetworkPacket<DPrivateMessagePacket> packet = packetRaw.DataAsType<DPrivateMessagePacket>();

                        this.OnPrivateMessageReceived?.Invoke(this, packet.data.sender, packet.data.message);
                    }
                    break;
                case nameof(DPrivateMessageSendResponse):
                    {
                        DNetworkPacket<DPrivateMessageSendResponse> packet = packetRaw.DataAsType<DPrivateMessageSendResponse>();

                        this.OnPrivateMessageSendResponse?.Invoke(this, packet.data.status == ResponsePacketStatus.Success, packet.data.errorMessage);
                    }
                    break;
                case nameof(DServerClosingPacket):
                    {
                        DNetworkPacket<DServerClosingPacket> packet = packetRaw.DataAsType<DServerClosingPacket>();

                        this.OnServerClosing?.Invoke(this, packet.data.reason);
                    }
                    break;
            }
        }

        public void SetState(IClientState newState)
        {
            this.State = newState;
        }

        public void SetState(Type newStateType)
        {
            if (newStateType.IsAssignableFrom(typeof(IClientState)))
            {
                if (false /* type check */)
                {
                }
                else
                {
                    this.State = Activator.CreateInstance(newStateType) as IClientState;
                }
            }
        }

        public void Login(string username, string password)
        {
            if(this.Connected) this.State.Login(username, password);
        }

        public void SendPrivateMessage(string receiver, string message)
        {
            if(this.Connected) this.State.SendPrivateMessage(receiver, message);
        }
    }

    public delegate void LoginResponseReceived(Client sender, bool success, string errorMessage);
    public delegate void PrivateMessageReceived(Client sender, string messageSender, string message);
    public delegate void ServerClosingReceived(Client sender, string reason);
    public delegate void PrivateMessageSendResponseReceived(Client sender, bool success, string errorMessage);
}

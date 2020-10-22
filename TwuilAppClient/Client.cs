using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using TwuilAppLib.Core;
using TwuilAppLib.Data;
using TwuilAppLib.Interface;

namespace TwuilAppClient
{
    public class Client : StateContext<ClientState> 
    {
        TcpClient client;
        Stream stream;
        private byte[] receiveBuffer;
        public string Username;

        public ClientState State { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Client()
        {
            this.client = new TcpClient("192.168.178.14", 8410);
            this.stream = this.client.GetStream();
            this.stream.BeginRead(this.receiveBuffer, 0, 1024, OnBytesReceived, null);
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
                0x00
            });
            this.stream.Write(buffer);
            this.stream.Write(new byte[]
            {
                Utility.CalculateChecksum(buffer)
            });
        }

        private void OnBytesReceived(IAsyncResult result)
        {
            int receiveBytes = this.stream.EndRead(result);
            int length = BitConverter.ToInt32(this.receiveBuffer.Skip(1).Take(4).ToArray());
            String message = BitConverter.ToString(this.receiveBuffer.Skip(6).Take(length).ToArray());
            this.stream.BeginRead(this.receiveBuffer, 0, 1024, OnBytesReceived, null);
            OnMessageReceived?.Invoke(this, message);
        }

        private void OnDataReceived(DNetworkPacket data)
        {

        }

        public void Login(string username, string password)
        {
            this.Username = username;
            this.State.Login(username, password);
        }

        public event MessageReceived OnMessageReceived;
    }

    public delegate void MessageReceived(Client sender, string message);
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using TwuilAppLib.Data;

namespace TwuilAppClient
{
    public class Client
    {
        TcpClient client;
        Stream stream;
        private byte[] receiveBuffer;

        

        public Client()
        {
            this.client = new TcpClient("192.168.178.14", 8410);
            this.stream = this.client.GetStream();
            this.stream.BeginRead(this.receiveBuffer, 0, 1024, OnBytesReceived, null);
        }

        public void Send(string message)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(message);
            this.stream.Write(buffer);
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

        public event MessageReceived OnMessageReceived;
    }

    public delegate void MessageReceived(Client sender, string message);
}

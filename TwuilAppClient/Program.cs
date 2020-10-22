using System;
using System.Threading;
using TwuilAppLib.Data;

namespace TwuilAppClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread.Sleep(1000);

            Client client = new Client();

            Console.WriteLine("Client Connected!");

            client.Send(new DLoginPacket { username = "test", password = "test" });

            Console.WriteLine("Login Send!");

            client.Send(new DMessagePacket { sender = "test", receiver = "test2", message = "Niggah" });

            Console.WriteLine("Message Send!");

            while (Console.ReadLine().ToLower() != "quit") { }
        }
    }
}

using System;
using System.Threading;
using TwuilAppClient.Core;
using TwuilAppLib.Data;

namespace TwuilAppClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread.Sleep(1000);

            Client client = new Client(Constants.SERVER_IP, Constants.SERVER_PORT);

            Console.WriteLine("Client Connected!");

            client.OnLoginResponseReceived += Client_OnLoginResponseReceived;
            client.OnServerClosing += Client_OnServerClosing;

            client.Send(new DLoginPacket { username = "test", password = "henk" });

            Console.WriteLine("Login Send!");

            /*client.Send(new DMessagePacket { sender = "test", receiver = "test2", message = "Niggah" });

            Console.WriteLine("Message Send!");*/

            while (Console.ReadLine().ToLower() != "quit") { }

            client.Send(new DClientDisconnectPacket());
        }

        private static void Client_OnServerClosing(Client sender, string reason)
        {
            Console.WriteLine($"ServerClosing => reason={reason}");

            Environment.Exit(0);
        }

        private static void Client_OnLoginResponseReceived(Client sender, bool success, string errorMessage)
        {
            Console.WriteLine($"Login => success={success}; errorMessage={errorMessage ?? "NULL"}");
        }
    }
}

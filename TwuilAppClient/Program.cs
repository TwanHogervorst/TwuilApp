using System;
using System.Collections.Generic;
using System.Linq;
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

            Console.Write("Username: ");
            string username = Console.ReadLine();
            Console.Write("Password: ");
            string password = Console.ReadLine();

            client.OnLoginResponseReceived += Client_OnLoginResponseReceived;
            client.OnServerClosing += Client_OnServerClosing;
            client.OnPrivateMessageSendResponse += Client_OnPrivateMessageSendResponse;
            client.OnPrivateMessageReceived += Client_OnPrivateMessageReceived;
            client.OnGroupCreatedResponse += Client_OnGroupCreatedResponse;
            client.OnGroupJoin += Client_OnGroupJoin;

            client.Login(username, password);

            Console.WriteLine("Login Send!");

            /*client.Send(new DMessagePacket { sender = "test", receiver = "test2", message = "Niggah" });

            Console.WriteLine("Message Send!");*/

            while (!client.IsActive) Thread.Sleep(10);

            client.CreateGroup("unittest", new List<string> { "test", "twan", "oegaboega" }, "Dit is een test groep!");

            string cmd;
            while ((cmd = Console.ReadLine()).ToLower() != "quit")
            {
                string[] msg = cmd.Split(';');

                if(msg.Length > 1) client.SendPrivateMessage(msg[0], msg.Skip(1).Aggregate("", (accu, elem) => accu + ";" + elem));
            }

            client.Dispose();
        }

        private static void Client_OnGroupJoin(Client sender, string groupName, List<string> usersInGroup, string welcomeMessage)
        {
            Console.WriteLine($"GroupJoin => groupName={groupName}; usersInGroup={string.Join(", ", usersInGroup)}; welcomeMessage={welcomeMessage}");
        }

        private static void Client_OnGroupCreatedResponse(Client sender, bool success, string errorMessage)
        {
            Console.WriteLine($"PrivateMessageSendResponse => success={success}; errorMessage={errorMessage ?? "NULL"}");
        }

        private static void Client_OnPrivateMessageReceived(Client sender, string messageSender, string message)
        {
            Console.WriteLine($"PrivateMessageReceived => messageSender={messageSender}; message={message}");
        }

        private static void Client_OnPrivateMessageSendResponse(Client sender, bool success, string errorMessage)
        {
            Console.WriteLine($"PrivateMessageSendResponse => success={success}; errorMessage={errorMessage ?? "NULL"}");
        }

        private static void Client_OnServerClosing(Client sender, string reason)
        {
            Console.WriteLine($"ServerClosing => reason={reason ?? "NULL"}");

            Environment.Exit(0);
        }

        private static void Client_OnLoginResponseReceived(Client sender, bool success, string errorMessage)
        {
            Console.WriteLine($"Login => success={success}; errorMessage={errorMessage ?? "NULL"}");
        }
    }
}

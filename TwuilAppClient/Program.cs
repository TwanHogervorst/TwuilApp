﻿using System;
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

            client.OnLoginResponseReceived += Client_OnLoginResponseReceived;
            client.OnServerClosing += Client_OnServerClosing;
            client.OnPrivateMessageSendResponse += Client_OnPrivateMessageSendResponse;
            client.OnPrivateMessageReceived += Client_OnPrivateMessageReceived;

            client.Send(new DLoginPacket { username = "test", password = "test" });

            Console.WriteLine("Login Send!");

            /*client.Send(new DMessagePacket { sender = "test", receiver = "test2", message = "Niggah" });

            Console.WriteLine("Message Send!");*/

            string cmd;
            while ((cmd = Console.ReadLine()).ToLower() != "quit")
            {
                string[] msg = cmd.Split(';');

                if(msg.Length > 1) client.SendPrivateMessage(msg[0], msg.Skip(1).Aggregate("", (accu, elem) => accu + ";" + elem));
            }

            client.Send(new DClientDisconnectPacket());
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

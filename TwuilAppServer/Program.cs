using System;
using System.Collections.Concurrent;
using TwuilAppLib.Data;
using TwuilAppServer.Core;

namespace TwuilAppServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server(Constants.IP_ADDRESS, Constants.SERVER_PORT);

            Console.WriteLine("Server Started!");

            while (Console.ReadLine().ToLower() != "quit") { }

            Console.WriteLine("Server Closing...");
            server.Dispose();
        }
    }
}

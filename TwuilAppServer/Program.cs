using System;
using TwuilAppLib.Data;

namespace TwuilAppServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server(Constants.SERVER_PORT);

            Console.WriteLine("Server Started!");

            while (Console.ReadLine().ToLower() != "quit") { }
        }
    }
}

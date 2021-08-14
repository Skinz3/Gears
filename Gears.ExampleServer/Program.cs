using System;
using System.Reflection;
using Gears.Protocol;
using Gears.ProtocolExample.Client;
using Gears.Sockets;
using Gears.Utils;

namespace Gears.ServerExample
{
    class Program
    {
        public const string Ip = "127.0.0.1";

        public const int Port = 999;

        static void Main(string[] args)
        {
            Console.Title = "Example Server";

            ProtocolManager.Initialize(Assembly.GetAssembly(typeof(HelloServerMessage)), Assembly.GetExecutingAssembly());

            MyServer server = new MyServer(Ip, Port);
            server.Start();

            Console.ReadLine();
        }
    }
}

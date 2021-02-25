using System;
using Tcp.Net.Sockets;
using Tcp.Net.Utils;

namespace Tcp.Net.ServerExample
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server("127.0.0.1", 999);
            server.Start();

            Logger.Write("Server started");

            Console.ReadLine();
        }
    }
}

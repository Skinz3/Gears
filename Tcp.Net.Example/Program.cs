using System;
using System.Reflection;
using Tcp.Net.Protocol;
using Tcp.Net.ProtocolExample.Client;

namespace Tcp.Net.Example
{
    class Program
    {
        public static void Main(string[] args)
        {
            ProtocolManager.Initialize(Assembly.GetAssembly(typeof(HelloServerMessage)), Assembly.GetExecutingAssembly());

            ExampleClient client = new ExampleClient();
            client.Connect("127.0.0.1", 999);

            Console.ReadLine();
        }
    }
}

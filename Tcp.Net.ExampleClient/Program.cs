using System;
using System.Reflection;
using Tcp.Net.Protocol;
using Tcp.Net.ProtocolExample.Client;

namespace Tcp.Net.ExampleClient
{
    class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "Example Client";

            ProtocolManager.Initialize(Assembly.GetAssembly(typeof(HelloServerMessage)), Assembly.GetExecutingAssembly());

            MyClient client = new MyClient();
            client.Connect("127.0.0.1", 999);

            Console.ReadLine();
        }
    }
}

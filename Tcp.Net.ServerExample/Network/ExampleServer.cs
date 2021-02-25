using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Sockets;
using System.Text;
using Tcp.Net.Sockets;
using Tcp.Net.Utils;

namespace Tcp.Net.ServerExample
{
    public class ExampleServer : Server<ExampleClient>
    {
        public ExampleServer(string ip, int port) : base(ip, port)
        {

        }

        public override ExampleClient CreateClient(Socket socket)
        {
            return new ExampleClient(socket);
        }

        public override void OnClientConnected(ExampleClient client)
        {
            Logger.Write("new client connected (" + client.Ip + ")");
        }

        public override void OnServerFailedToStart(Exception ex)
        {
            Logger.Write("Unable to start server : " + ex);
        }

        public override void OnServerStarted()
        {
            Logger.Write("Server started " + EndPoint, Channels.Info);
        }
    }
}

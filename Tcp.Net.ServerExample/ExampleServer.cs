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
    public class ExampleServer : Server
    {
        private ConcurrentBag<ExampleClient> _clients
        {
            get;
            set;
        }

        public ExampleServer(string ip, int port) : base(ip, port)
        {
            this._clients = new ConcurrentBag<ExampleClient>();
        }

        public override void OnServerFailedToStart(Exception ex)
        {
            Logger.Write("Unable to start server : " + ex);
        }

        public override void OnServerStarted()
        {
            Logger.Write("Server started " + EndPoint, Channels.Info);
        }

        public override void OnSocketConnected(Socket socket)
        {
            ExampleClient client = new ExampleClient(socket);
            _clients.Add(client);

            Logger.Write("new client connected (" + client.Ip + ")");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Tcp.Net.Sockets;
using Tcp.Net.Utils;

namespace Tcp.Net.ServerExample
{
    public class ExampleServer : Server
    {
        private ReadOnlyCollection<>
        public ExampleServer(string ip, int port) : base(ip, port)
        {
        }

        public override void OnServerFailedToStart(Exception ex)
        {
            throw new NotImplementedException();
        }

        public override void OnServerStarted()
        {
            Logger.Write("Server started " + EndPoint, Channels.Info);
        }

        public override void OnSocketConnected(Socket socket)
        {
            throw new NotImplementedException();
        }
    }
}

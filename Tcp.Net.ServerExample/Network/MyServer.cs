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
    public class MyServer : Server<MyClient>
    {
        public MyServer(string ip, int port) : base(ip, port)
        {

        }

        public override MyClient CreateClient(Socket socket)
        {
            return new MyClient(socket);
        }

        public override void OnClientConnected(MyClient client)
        {
            Logger.Write("new client connected (" + client.IPAddress + ")");
        }

        public override void OnServerFailedToStart(Exception ex)
        {
            Logger.Write("Unable to start server : " + ex, Channels.Warning);
        }

        public override void OnServerStarted()
        {
            Logger.Write("Server started " + EndPoint, Channels.Info);
        }
    }
}

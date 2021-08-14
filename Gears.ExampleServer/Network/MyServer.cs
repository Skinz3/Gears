using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Sockets;
using System.Text;
using Gears.Sockets;
using Gears.Utils;

namespace Gears.ServerExample
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

        public override void OnConnect(MyClient client)
        {
            Logger.Write("new client connected (" + client.IPAddress + ")");
        }

        public override void OnError(Exception ex)
        {
            Logger.Write("Unable to start server : " + ex, Channels.Warning);
        }

        public override void OnStart()
        {
            Logger.Write("Server started " + EndPoint, Channels.Info);
        }
    }
}

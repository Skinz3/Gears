using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Gears.Protocol;
using Gears.Sockets;
using Gears.Utils;

namespace Gears.ServerExample
{
    public class MyClient : Client
    {
        public MyClient(Socket socket) : base(socket)
        {

        }
        public override void OnConnect()
        {

        }

        public override void OnDisconnect()
        {
            Logger.Write("Client disconnected " + IPAddress);
        }

        public override void OnError(Exception ex)
        {

        }

        public override void OnReceive(Message message)
        {
            Logger.Write("Received " + message, Channels.Protocol);
        }

        public override void OnSend(IAsyncResult result)
        {
            Logger.Write("Sended " + result.AsyncState, Channels.Protocol);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Gears.Protocol;
using Gears.ProtocolExample.Client;
using Gears.Sockets;
using Gears.Utils;

namespace Gears.ExampleClient
{
    public class MyClient : Client
    {
        public override void OnConnect()
        {
            Logger.Write("Connected to server !");
            this.Send(new HelloServerMessage("test", "test@example.com"));
        }

        public override void OnDisconnect()
        {
            Logger.Write("Connection closed by server.");
        }

        public override void OnError(Exception ex)
        {
            Logger.Write("Unable to connect to server.");
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

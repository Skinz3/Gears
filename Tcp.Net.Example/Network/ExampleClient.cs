using System;
using System.Collections.Generic;
using System.Text;
using Tcp.Net.Protocol;
using Tcp.Net.ProtocolExample.Client;
using Tcp.Net.Sockets;
using Tcp.Net.Utils;

namespace Tcp.Net.Example
{
    public class ExampleClient : Client
    {
        public override void OnConnected()
        {
            Logger.Write("Connected to server !");
            this.Send(new HelloServerMessage("test", "test@example.com"));
        }

        public override void OnConnectionClosed()
        {
            Logger.Write("Connection closed !");
        }

        public override void OnDisconnected()
        {
            throw new NotImplementedException();
        }

        public override void OnFailToConnect(Exception ex)
        {
            Logger.Write("Unable to connect to server.");
        }

        public override void OnMessageReceived(Message message)
        {
            Logger.Write("Received " + message, Channels.Protocol);
        }

        public override void OnSended(IAsyncResult result)
        {
            Logger.Write("Sended " + result.AsyncState, Channels.Protocol);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Tcp.Net.Protocol;
using Tcp.Net.Sockets;
using Tcp.Net.Utils;

namespace Tcp.Net.ServerExample
{
    public class ExampleClient : Client
    {
        public ExampleClient(Socket socket) : base(socket)
        {

        }
        public override void OnConnected()
        {

        }

        public override void OnConnectionClosed()
        {

        }

        public override void OnDisconnected()
        {

        }

        public override void OnFailToConnect(Exception ex)
        {

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

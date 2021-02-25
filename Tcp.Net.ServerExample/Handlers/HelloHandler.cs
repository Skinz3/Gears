using System;
using System.Collections.Generic;
using System.Text;
using Tcp.Net.Protocol;
using Tcp.Net.ProtocolExample.Client;
using Tcp.Net.ProtocolExample.Server;

namespace Tcp.Net.ServerExample.Handlers
{
    class HelloHandler
    {
        [MessageHandler]
        public static void HandleHelloServerMessage(HelloServerMessage message, MyClient client)
        {
            client.Send(new HelloClientMessage("Hello client !"));
        }
    }
}

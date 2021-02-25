using System;
using System.Collections.Generic;
using System.Text;
using Tcp.Net.ExampleClient;
using Tcp.Net.Protocol;
using Tcp.Net.ProtocolExample.Server;
using Tcp.Net.Utils;

namespace Tcp.Net.Example.ExampleClient
{
    class HelloHandler
    {
        [MessageHandler]
        public static void HandleHelloClientMessage(HelloClientMessage message, MyClient client)
        {
            Logger.Write("We received : " + message.content);
        }
    }
}

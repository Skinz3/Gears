using System;
using System.Collections.Generic;
using System.Text;
using Gears.Protocol;
using Gears.ProtocolExample.Client;
using Gears.ProtocolExample.Server;

namespace Gears.ServerExample.Handlers
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

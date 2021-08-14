using System;
using System.Collections.Generic;
using System.Text;
using Gears.ExampleClient;
using Gears.Protocol;
using Gears.ProtocolExample.Server;
using Gears.Utils;

namespace Gears.Example.ExampleClient
{
    class HelloHandler
    {
        [MessageHandler]
        public static void HandleHelloClientMessage(HelloClientMessage message, MyClient client)
        {
            Logger.Write("We received : " + message.Content);
        }
    }
}

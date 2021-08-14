using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Gears.Protocol;

namespace Gears.ProtocolExample.Server
{
    public class HelloClientMessage : Message
    {
        public const ushort Id = 2;

        public override ushort MessageId => Id;

        public string Content;

        public HelloClientMessage(string content)
        {
            this.Content = content;
        }
        public HelloClientMessage()
        {

        }
      
    }
}

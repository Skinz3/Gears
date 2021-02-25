using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Tcp.Net.Protocol;

namespace Tcp.Net.ProtocolExample.Server
{
    public class HelloClientMessage : Message
    {
        public const ushort Id = 2;

        public override ushort MessageId => Id;

        public string content;

        public HelloClientMessage(string content)
        {
            this.content = content;
        }
        public HelloClientMessage()
        {

        }
        public override void Deserialize(BinaryReader reader)
        {
            content = reader.ReadString();
        }

        public override void Serialize(BinaryWriter writer)
        {
            writer.Write(content);
        }
    }
}

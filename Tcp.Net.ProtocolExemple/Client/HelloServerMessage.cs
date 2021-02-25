using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Tcp.Net.Protocol;

namespace Tcp.Net.ProtocolExample.Client
{
    public class HelloServerMessage : Message
    {
        public const ushort Id = 1;

        public override ushort MessageId => Id;

        public string username;
        public string mail;

        public HelloServerMessage(string userName,string mail)
        {
            this.username = userName;
            this.mail = mail;
        }
        public HelloServerMessage()
        {

        }
        public override void Deserialize(BinaryReader reader)
        {
            this.username = reader.ReadString();
            this.mail = reader.ReadString();
        }

        public override void Serialize(BinaryWriter writer)
        {
            writer.Write(username);
            writer.Write(mail);
        }
    }
}

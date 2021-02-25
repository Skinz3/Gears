using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Tcp.Net.Sockets;

namespace Tcp.Net.Protocol
{
    public abstract class Message
    {
        public abstract ushort MessageId
        {
            get;
        }
        public void Unpack(BinaryReader reader)
        {
            this.Deserialize(reader);
        }
        public void Pack(BinaryWriter writer)
        {
            writer.Write(MessageId);
            Serialize(writer);
        }
        public abstract void Serialize(BinaryWriter writer);
        public abstract void Deserialize(BinaryReader reader);

        public override string ToString()
        {
            return base.GetType().Name;
        }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Gears.Sockets;

namespace Gears.Protocol
{
    public abstract class Message
    {
        public abstract ushort MessageId
        {
            get;
        }

        public override string ToString()
        {
            return base.GetType().Name;
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(MessageId);
            var content = JsonConvert.SerializeObject(this);
            writer.Write(content);
        }
    }
}

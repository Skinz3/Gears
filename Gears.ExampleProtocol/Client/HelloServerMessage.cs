using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Gears.Protocol;

namespace Gears.ProtocolExample.Client
{
    public class HelloServerMessage : Message
    {
        public const ushort Id = 1;
        public override ushort MessageId => Id;

        public string Username;
        public string Mail;

        public HelloServerMessage(string username, string mail)
        {
            this.Username = mail;
            this.Mail = mail;
        }
        public HelloServerMessage()
        {

        }
       
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using Tcp.Net.Extensions;
using Tcp.Net.Sockets;
using Tcp.Net.Utils;

namespace Tcp.Net.Protocol
{
    public class ProtocolManager
    {
        public const string ID_MESSAGE_FIELD_NAME = "Id";

        private static readonly Type[] HandlerMethodParameterTypes = new Type[] { typeof(Message), typeof(TcpClient) };

        private static readonly Dictionary<ushort, Delegate> Handlers = new Dictionary<ushort, Delegate>();

        private static readonly Dictionary<ushort, Type> Messages = new Dictionary<ushort, Type>();

        private static readonly Dictionary<ushort, Func<Message>> Constructors = new Dictionary<ushort, Func<Message>>();

        public static Logger logger = new Logger();

        public static bool Initialized;

        public static void Initialize(Assembly messagesAssembly, Assembly handlersAssembly)
        {
            foreach (var type in messagesAssembly.GetTypes().Where(x => x.IsSubclassOf(typeof(Message))))
            {
                FieldInfo field = type.GetField(ID_MESSAGE_FIELD_NAME);

                if (field != null)
                {
                    ushort num = (ushort)field.GetValue(type);
                    if (Messages.ContainsKey(num))
                    {
                        throw new AmbiguousMatchException(string.Format("MessageReceiver() => {0} item is already in the dictionary, old type is : {1}, new type is  {2}",
                            num, Messages[num], type));
                    }

                    Messages.Add(num, type);

                    ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
                    if (constructor == null)
                    {
                        throw new Exception(string.Format("'{0}' doesn't implemented a parameterless constructor", type));
                    }
                    Constructors.Add(num, constructor.CreateDelegate<Func<Message>>());
                }
            }

            foreach (var item in handlersAssembly.GetTypes())
            {
                foreach (var subItem in item.GetMethods())
                {
                    var attribute = subItem.GetCustomAttribute(typeof(MessageHandlerAttribute));
                    if (attribute != null)
                    {
                        ParameterInfo[] parameters = subItem.GetParameters();
                        Type methodParameters = subItem.GetParameters()[0].ParameterType;
                        if (methodParameters.BaseType != null)
                        {
                            try
                            {
                                Delegate target = subItem.CreateDelegate(HandlerMethodParameterTypes);
                                FieldInfo field = methodParameters.GetField("Id");
                                Handlers.Add((ushort)field.GetValue(null), target);
                            }
                            catch
                            {
                                throw new Exception("Cannot register " + subItem.Name + " has message handler...");
                            }

                        }
                    }

                }
            }

            Logger.Write(Messages.Count + " Message(s) Loaded | " + Handlers.Count + " Handler(s) Loaded");

            Initialized = true;
        }
        /// <summary>
        /// Unpack message
        /// </summary>
        /// <param name="id">Id of the message</param>
        /// <param name="reader">Reader with the message datas</param>
        /// <returns></returns>
        private static Message ConstructMessage(ushort id, BinaryReader reader)
        {
            if (!Messages.ContainsKey(id))
            {
                return null;
            }
            Message message = Constructors[id]();
            if (message == null)
            {
                return null;
            }
            message.Unpack(reader);
            return message;
        }
        public static ushort GetNextMessageId()
        {
            return (ushort)(Messages.Keys.OrderByDescending(x => x).First() + 1);
        }
        public static bool HandleMessage(Message message, TcpClient client)
        {
            if (message == null)
            {
                client.Close();
                return false;
            }

            Delegate handler = null;
            Handlers.TryGetValue(message.MessageId, out handler);

            if (handler != null)
            {
                try
                {
                    handler.DynamicInvoke(null, message, client);
                    return true;

                }
                catch (Exception ex)
                {
                    Logger.Write(string.Format("Unable to handle message {0} {1} : '{2}'", message.ToString(), handler.Method.Name, ex.InnerException.ToString()), Channels.Warning);
                    return false;
                }
            }
            else
            {
                Logger.Write(string.Format("No Handler: ({0}) {1}", message.MessageId, message.ToString()), Channels.Warning);
                return true;
            }
        }

        public static Message BuildMessage(byte[] buffer)
        {
            using (MemoryStream stream = new MemoryStream(buffer))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    ushort messageId = reader.ReadUInt16();

                    Message message;
                    try
                    {
                        message = ConstructMessage(messageId, reader);
                        return message;
                    }
                    catch (Exception ex)
                    {
                        Logger.Write("Error while building Message :" + ex.Message, Channels.Warning);
                        return null;
                    }
                }
            }


        }
    }
}

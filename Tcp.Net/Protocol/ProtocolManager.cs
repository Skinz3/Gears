using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Tcp.Net.Extensions;
using Tcp.Net.Sockets;
using Tcp.Net.Utils;

namespace Tcp.Net.Protocol
{
    public class ProtocolManager
    {
        public const string MessageIdTargetField = "Id";

        private static readonly Type[] _handlerParameterTypes = new Type[] { typeof(Message), typeof(Client) };

        private static readonly Dictionary<ushort, Delegate> _handlers = new Dictionary<ushort, Delegate>();

        private static readonly Dictionary<ushort, Type> _messages = new Dictionary<ushort, Type>();

        private static readonly Dictionary<ushort, Func<Message>> _ctors = new Dictionary<ushort, Func<Message>>();

        public static bool _initialized = false;

        public static void Initialize(Assembly messagesAssembly, Assembly handlersAssembly)
        {
            foreach (var type in messagesAssembly.GetTypes().Where(x => x.IsSubclassOf(typeof(Message))))
            {
                FieldInfo field = type.GetField(MessageIdTargetField);

                if (field != null)
                {
                    ushort num = (ushort)field.GetValue(type);
                    if (_messages.ContainsKey(num))
                    {
                        throw new AmbiguousMatchException(string.Format("MessageReceiver() => {0} item is already in the dictionary, old type is : {1}, new type is  {2}",
                            num, _messages[num], type));
                    }

                    _messages.Add(num, type);

                    ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
                    if (constructor == null)
                    {
                        throw new Exception(string.Format("'{0}' doesn't implemented a parameterless constructor", type));
                    }
                    _ctors.Add(num, constructor.CreateDelegate<Func<Message>>());
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
                                Delegate target = subItem.CreateDelegate(_handlerParameterTypes);
                                FieldInfo field = methodParameters.GetField("Id");
                                _handlers.Add((ushort)field.GetValue(null), target);
                            }
                            catch
                            {
                                throw new Exception("Cannot register " + subItem.Name + " has message handler...");
                            }

                        }
                    }

                }
            }

            Logger.Write(_messages.Count + " Message(s) Loaded | " + _handlers.Count + " Handler(s) Loaded");

            _initialized = true;
        }

        private static Message ConstructMessage(ushort id, BinaryReader reader)
        {
            if (!_messages.ContainsKey(id))
            {
                return null;
            }
            Message message = _ctors[id]();
            if (message == null)
            {
                return null;
            }
            message.Unpack(reader);
            return message;
        }
        public static bool HandleMessage(Message message, Client client)
        {
            if (!_initialized)
            {
                throw new UnauthorizedAccessException("This function cannot be used until ProtocolManager.Intitialize() has been called.");
            }
            if (message == null)
            {
                client.Disconnect();
                return false;
            }

            Delegate handler = null;
            _handlers.TryGetValue(message.MessageId, out handler);

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
            if (!_initialized)
            {
                throw new UnauthorizedAccessException("This function cannot be used until ProtocolManager.Intitialize() has been called.");
            }

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

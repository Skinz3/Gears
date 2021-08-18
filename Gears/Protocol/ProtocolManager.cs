using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Gears.Extensions;
using Gears.Sockets;
using Gears.Utils;

namespace Gears.Protocol
{
    public class ProtocolManager
    {
        public const string MessageIdTargetField = "Id";

        private static readonly Type[] HandlerParameters = new Type[] { typeof(Message), typeof(Client) };

        private static readonly Dictionary<ushort, Delegate> Handlers = new Dictionary<ushort, Delegate>();

        private static readonly Dictionary<ushort, Type> MessageTypes = new Dictionary<ushort, Type>();

        public static bool _initialized = false;

        /*
         * Initialize protocol, (create binding)
         * messagesAssembly : The assembly where is located your protocol classes.
         * handlersAssembly : The assembly where is located your methods to handle messages.
         */
        public static void Initialize(Assembly messagesAssembly, Assembly handlersAssembly)
        {
            foreach (var type in messagesAssembly.GetTypes().Where(x => x.IsSubclassOf(typeof(Message))))
            {
                FieldInfo field = type.GetField(MessageIdTargetField);

                if (field != null)
                {
                    ushort num = (ushort)field.GetValue(type);

                    if (MessageTypes.ContainsKey(num))
                    {
                        throw new AmbiguousMatchException(string.Format("MessageReceiver() => {0} item is already in the dictionary, old type is : {1}, new type is  {2}",
                            num, MessageTypes[num], type));
                    }

                    MessageTypes.Add(num, type);

                    ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);

                    if (constructor == null)
                    {
                        throw new Exception(string.Format("'{0}' doesn't implemented a parameterless constructor", type));
                    }
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
                                Delegate target = subItem.CreateDelegate(HandlerParameters);
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

            Logger.Write(MessageTypes.Count + " Message(s) Loaded | " + Handlers.Count + " Handler(s) Loaded");

            _initialized = true;
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
            if (!_initialized)
            {
                throw new UnauthorizedAccessException("This function cannot be used until ProtocolManager.Intitialize() has been called.");
            }

            using (MemoryStream stream = new MemoryStream(buffer))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    ushort messageId = reader.ReadUInt16();

                    try
                    {
                        if (!MessageTypes.ContainsKey(messageId))
                        {
                            return null;
                        }

                        var content = reader.ReadString();

                        var queue = reader.ReadString();

                        var messageType = MessageTypes[messageId];

                        var message = (Message)JsonConvert.DeserializeObject(content, messageType);

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

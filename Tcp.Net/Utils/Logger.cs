using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Tcp.Net.Utils
{
    [Flags]
    public enum Channels
    {
        Info = 1,
        Warning = 2,
        Critical = 4,
        Protocol = 8,
        Database = 16,
    }
    public class Logger
    {
        private static Channels Channels = Channels.Info | Channels.Warning | Channels.Critical | Channels.Protocol | Channels.Database;

        private const ConsoleColor Color1 = ConsoleColor.Cyan;
        private const ConsoleColor Color2 = ConsoleColor.DarkCyan;

        private static Dictionary<Channels, ConsoleColor> ChannelsColors = new Dictionary<Channels, ConsoleColor>()
        {
            { Channels.Info,     ConsoleColor.Gray },
            { Channels.Warning,  ConsoleColor.Yellow },
            { Channels.Critical, ConsoleColor.Red },
            { Channels.Protocol, ConsoleColor.DarkGray },
        };

        public static void SetChannels(Channels channels)
        {
            Channels = channels;
        }
        public static void EnableChannel(Channels channels)
        {
            Channels |= channels;
        }
        public static void DisableChannel(Channels channels)
        {
            Channels &= ~channels;
        }

        public static void Write(object value, Channels state = Channels.Info)
        {
            if (Channels.HasFlag(state))
            {
                WriteColored(value, ChannelsColors[state]);
            }
        }
        public static void WriteColor1(object value)
        {
            WriteColored(value, Color1);
        }
        public static void WriteColor2(object value)
        {
            WriteColored(value, Color2);
        }
        private static void WriteColored(object value, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(value);
        }
        public static void NewLine()
        {
            Console.WriteLine(Environment.NewLine);
        }
    }
}

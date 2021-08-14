using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Gears.Protocol;
using Gears.Utils;

namespace Gears.Sockets
{
    public abstract class Client
    {
        private Socket m_socket;

        private byte[] m_buffer;

        public IPEndPoint EndPoint => (IPEndPoint)m_socket.RemoteEndPoint;

        public IPAddress IPAddress => EndPoint.Address;

        public bool Connected => m_socket != null && m_socket.Connected;

        public Client(int bufferLength = 512)
        {
            m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            m_buffer = new byte[bufferLength];
        }
        public Client(Socket socket, int bufferLength = 512)
        {
            this.m_buffer = new byte[bufferLength];
            this.m_socket = socket;
            BeginReceive();
        }

        public abstract void OnDisconnect();

        public abstract void OnConnect();

        public abstract void OnReceive(Message message);

        public abstract void OnError(Exception ex);

        public abstract void OnSend(IAsyncResult result);

        private void OnDataArrival(int dataSize)
        {
            Message message = ProtocolManager.BuildMessage(m_buffer);

            if (message != null)
            {
                OnReceive(message);
                ProtocolManager.HandleMessage(message, this);
            }
            else
            {
                Logger.Write("Received Unknown Data", Channels.Warning);
            }
        }

        public void Send(Message message)
        {
            if (m_socket == null || !m_socket.Connected)
            {
                Logger.Write("Attempt to send message to invalid socket.", Channels.Warning);
                return;
            }

            try
            {
                using (var stream = new MemoryStream())
                {
                    using (var writer = new BinaryWriter(stream))
                    {
                        message.Serialize(writer);
                        var data = stream.GetBuffer();
                        m_socket.BeginSend(data, 0, data.Length, SocketFlags.None, OnSend, message);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Unable to send message to " + IPAddress + "." + ex, Channels.Warning);
                Disconnect();
            }

        }

        public void Connect(string host, int port)
        {
            m_socket?.BeginConnect(new IPEndPoint(IPAddress.Parse(host), port), new AsyncCallback(OnConnectionResulted), m_socket);
        }

        public void OnConnectionResulted(IAsyncResult result)
        {
            try
            {
                m_socket.EndConnect(result);
                BeginReceive();
                OnConnect();
            }
            catch (Exception ex)
            {
                OnError(ex);
            }
        }
        private void BeginReceive()
        {
            try
            {
                m_socket?.BeginReceive(m_buffer, 0, m_buffer.Length, SocketFlags.None, OnReceived, null);
            }
            catch (Exception ex)
            {
                Logger.Write(string.Format("Unable to receive from {0} : {1}", IPAddress, ex), Channels.Warning);
                Disconnect();
            }
        }
        public void OnReceived(IAsyncResult result)
        {
            if (m_socket == null)
            {
                return;
            }

            int size;

            try
            {
                size = m_socket.EndReceive(result);

                if (size == 0)
                {
                    OnDisconnect();
                    Dispose();
                    return;
                }

            }
            catch (SocketException ex)
            {
                switch (ex.SocketErrorCode)
                {
                    case SocketError.ConnectionReset:
                        OnDisconnect();
                        Dispose();
                        return;
                }

                Logger.Write(string.Format("Unable to receive data from ip {0}: {1}", IPAddress, ex), Channels.Warning);
                OnDisconnect();
                Dispose();
                return;
            }

            OnDataArrival(size);
            BeginReceive();
        }
        public void Disconnect()
        {
            if (m_socket != null)
            {
                Dispose();
            }
        }

        private void Dispose()
        {
            m_socket?.Shutdown(SocketShutdown.Both);
            m_socket?.Close();
            m_socket?.Dispose();
            m_socket = null;

        }
    }
}

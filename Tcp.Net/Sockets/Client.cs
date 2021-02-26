using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Tcp.Net.Protocol;
using Tcp.Net.Utils;

namespace Tcp.Net.Sockets
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

        public abstract void OnMessageReceived(Message message);

        public abstract void OnFailToConnect(Exception ex);

        private void OnDataArrival(int dataSize)
        {
            Message message = ProtocolManager.BuildMessage(m_buffer);

            if (message != null)
            {
                OnMessageReceived(message);
                ProtocolManager.HandleMessage(message, this);
            }
            else
            {
                Logger.Write("Received Unknown Data", Channels.Warning);
            }
        }


        public void Send(Message message)
        {
            if (m_socket != null && m_socket.Connected)
            {
                try
                {
                    using (var stream = new MemoryStream())
                    {
                        using (var writer = new BinaryWriter(stream))
                        {
                            message.Pack(writer);
                            var data = stream.GetBuffer();
                            m_socket.BeginSend(data, 0, data.Length, SocketFlags.None, OnSended, message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Write("Unable to send message to " + IPAddress + "." + ex, Channels.Warning);
                    Disconnect();
                }
            }
            else
            {
                Logger.Write("Attempt was made to send data to disconnect socket.", Channels.Warning);
            }

        }
        public abstract void OnSended(IAsyncResult result);

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
                OnFailToConnect(ex);
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

            int size = 0;
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

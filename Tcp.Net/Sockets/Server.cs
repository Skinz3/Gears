using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Tcp.Net.Sockets
{
    public abstract class Server<T> where T : Client
    {
        public ConcurrentBag<T> Clients
        {
            get;
            private set;
        } = new ConcurrentBag<T>();

        public Socket Socket
        {
            get;
            private set;
        }
        public IPEndPoint EndPoint
        {
            get;
            private set;
        }
        public int Backlog
        {
            get;
            private set;
        }
        public Server(string ip, int port, int backlog = 50)
        {
            Backlog = backlog;
            EndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
        public Server(int port, int backlog = 50)
        {
            Backlog = backlog;
            EndPoint = new IPEndPoint(IPAddress.Any, port);
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
        public void Start()
        {
            try
            {
                Socket.Bind(EndPoint);
            }
            catch (Exception ex)
            {
                OnServerFailedToStart(ex);
                return;
            }
            Socket.Listen(Backlog);
            StartAccept(null);
            OnServerStarted();
        }
        protected void StartAccept(SocketAsyncEventArgs args)
        {
            if (args == null)
            {
                args = new SocketAsyncEventArgs();
                args.Completed += AcceptEventCompleted;
            }
            else
            {
                args.AcceptSocket = null;
            }

            bool willRaiseEvent = Socket.AcceptAsync(args);
            if (!willRaiseEvent)
            {
                ProcessAccept(args);
            }
        }
        private void AcceptEventCompleted(object sender, SocketAsyncEventArgs e)
        {
            ProcessAccept(e);
        }
        public void Stop()
        {
            Socket.Shutdown(SocketShutdown.Both);
        }
        void ProcessAccept(SocketAsyncEventArgs args)
        {
            T client = CreateClient(args.AcceptSocket);
            Clients.Add(client);
            OnClientConnected(client);
            StartAccept(args);
        }

        public abstract T CreateClient(Socket socket);
        public abstract void OnServerStarted();
        public abstract void OnServerFailedToStart(Exception ex);
        public abstract void OnClientConnected(T client);


    }
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Gears.Sockets
{
    public abstract class Server<TClient> where TClient : Client
    {
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
                OnError(ex);
                return;
            }

            Socket.Listen(Backlog);
            StartAccept(null);
            OnStart();
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
            TClient client = CreateClient(args.AcceptSocket);
            OnConnect(client);
            StartAccept(args);
        }

        public abstract TClient CreateClient(Socket socket);
        public abstract void OnStart();
        public abstract void OnError(Exception ex);
        public abstract void OnConnect(TClient client);


    }
}

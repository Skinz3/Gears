using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Tcp.Net.Sockets
{
    public abstract class Server
    {
        public Socket Socket
        {
            get;
            private set;
        }

        public IPEndPoint EndPoint
        {
            get;
            set;
        }

        public Server(string ip, int port)
        {
            EndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
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
            Socket.Listen(100);
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
            OnSocketConnected(args.AcceptSocket);
            StartAccept(args);
        }

        public abstract void OnServerStarted();
        public abstract void OnServerFailedToStart(Exception ex);
        public abstract void OnSocketConnected(Socket socket);


    }
}

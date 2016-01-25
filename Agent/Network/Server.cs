using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Agent.Network
{
    public class Server
    {
        public event EventHandler OnClientConnected;
        public event EventHandler OnClientDisconnected;

        private List<ClientHandler> clients = new List<ClientHandler>();
        private TcpListener listener = null;

        public Server(int port)
        {
            this.listener = new TcpListener(IPAddress.Any, port);
        }

        public bool IsRunning { get; private set; }

        /// <summary>
        /// Starts listening for clients.
        /// </summary>
        /// <exception cref="SocketException">Thrown if binding to local port failed.</exception>
        public void Start()
        {
            this.listener.Start();
            this.IsRunning = true;

            try
            {
                this.listener.BeginAcceptTcpClient(new AsyncCallback(this.AcceptTcpClientCallback), null);
            }
            catch (Exception ex)
            when (ex is ObjectDisposedException ||
                  ex is SocketException)
            {
            }
        }

        /// <summary>
        /// Stops listening for clients and closes all client connections.
        /// </summary>
        public void Stop()
        {
            this.IsRunning = false;

            try { this.listener.Stop(); }
            catch { }
            
            foreach (ClientHandler client in this.clients)
            {
                client.OnClientDisconnected -= this.Client_OnClientDisconnected;
                client.Stop();
            }

            this.clients.Clear();
        }
        
        /// <summary>
        /// Accepts a new TCP client and asynchronously handles the connection and its messages.
        /// </summary>
        public void AcceptTcpClientCallback(IAsyncResult ar)
        {
            try
            {
                TcpClient tcpClient = this.listener.EndAcceptTcpClient(ar);
                ClientHandler client = new ClientHandler(tcpClient);

                this.clients.Add(client);
                client.OnClientDisconnected += this.Client_OnClientDisconnected;
                client.Start();

                if (this.OnClientConnected != null)
                {
                    this.OnClientConnected(this, EventArgs.Empty);
                }

                this.listener.BeginAcceptTcpClient(this.AcceptTcpClientCallback, null);
            }
            catch (Exception ex)
            when (ex is ObjectDisposedException ||
                  ex is SocketException)
            {
            }
        }

        private void Client_OnClientDisconnected(object sender, EventArgs e)
        {
            this.clients.Remove((ClientHandler)sender);

            if (this.OnClientDisconnected != null)
            {
                this.OnClientDisconnected(this, EventArgs.Empty);
            }
        }
    }
}

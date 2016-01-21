using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;


// ***************************************************************
// TODO: ERROR HANDLING
// ***************************************************************


namespace Agent.Network
{
    public class Server
    {
        private List<ClientHandler> clients = new List<ClientHandler>();
        private TcpListener listener = null;

        public Server(int port)
        {
            this.listener = new TcpListener(IPAddress.Any, port);
        }

        public bool IsRunning { get; private set; }

        public void Start()
        {
            this.listener.Start();
            this.IsRunning = true;
            this.listener.BeginAcceptTcpClient(new AsyncCallback(this.AcceptTcpClientCallback), null);
        }

        public void Stop()
        {
            this.IsRunning = false;
            this.listener.Stop();

            foreach (ClientHandler client in this.clients)
            {
                client.Stop();
            }
        }
        
        public void AcceptTcpClientCallback(IAsyncResult ar)
        {
            TcpClient tcpClient = this.listener.EndAcceptTcpClient(ar);
            ClientHandler client = new ClientHandler(tcpClient);

            this.clients.Add(client);
            client.OnClientDisconnected += this.Client_OnClientDisconnected;
            client.Start();
            
            this.listener.BeginAcceptTcpClient(this.AcceptTcpClientCallback, null);
        }

        private void Client_OnClientDisconnected(object sender, EventArgs e)
        {
            //TODO
            Console.WriteLine("Client disconnected!");
            this.clients.Remove((ClientHandler)sender);
        }
    }
}

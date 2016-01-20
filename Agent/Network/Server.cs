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

        public void Start()
        {
            this.listener.Start();
            this.listener.BeginAcceptTcpClient(new AsyncCallback(this.AcceptTcpClientCallback), null);
        }

        public void Stop()
        {
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
            client.Start();
            
            this.listener.BeginAcceptTcpClient(this.AcceptTcpClientCallback, null);
        }
    }
}

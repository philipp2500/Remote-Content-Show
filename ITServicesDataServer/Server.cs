using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;
using System.Net;

namespace ITServicesDataServer
{
    public class Server
    {
        private int port;
        private Thread thread;
        private bool run = true;
        private TcpListener listener;

        public event EventHandler<RequestHandler> OnRequest;

        public Server(int port)
        {
            this.port = port;
            this.listener = new TcpListener(IPAddress.Any, this.port);
            this.thread = new Thread(new ThreadStart(WaitForConnection));
            this.thread.Start();
        }

        public void Stop()
        {
            try
            {
                this.run = false;
                this.listener.Stop();
            }
            catch
            {
            }
        } 

        protected void FireOnRequest(RequestHandler request)
        {
            if (this.OnRequest != null)
            {
                this.OnRequest(this, request);
            }
        }

        private void WaitForConnection()
        {
            try
            {
                this.listener.Start();
                while (this.run)
                {
                    Client client = new Client(this.listener.AcceptTcpClient());
                    client.OnRequest += Client_OnRequest;
                }
            }
            catch
            {
            }
            
        }

        private void Client_OnRequest(object sender, RequestHandler e)
        {
            this.FireOnRequest(e);
        }
    }
}

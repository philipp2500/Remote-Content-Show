using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Sockets;

namespace DisplayClient
{
    public class Server
    {
        private int port;

        private StreamSocketListener listener;

        public Server(int port)
        {
            this.port = port;
        }

        public delegate void ConnectionReceived(StreamSocket socket);

        public event ConnectionReceived OnConnectionReceived;

        public async void Start()
        {
            this.listener = new StreamSocketListener();
            this.listener.ConnectionReceived += Listener_ConnectionReceived;

            await listener.BindServiceNameAsync("1051");
        }

        private void Listener_ConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            if (this.OnConnectionReceived != null)
            {
                this.OnConnectionReceived(args.Socket);
            }
        }
    }
}

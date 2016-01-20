using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;

namespace DisplayClient
{
    public class Client
    {
        public Client()
        {
        }

        public async Task<StreamSocket> Connect(string address, int port)
        {
            StreamSocket socket = new StreamSocket();

            await socket.ConnectAsync(new HostName(address), port.ToString());
            
            return socket;
        }
    }
}

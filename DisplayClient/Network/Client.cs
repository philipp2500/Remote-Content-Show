using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
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

        public async Task<StreamSocket> Connect(IPAddress address, int port)
        {
            return await this.Connect(address.ToString(), port);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        /// <param name="port"></param>
        /// <param name="timeout">in milliseconds</param>
        /// <returns></returns>
        public async Task<StreamSocket> Connect(IPAddress address, int port, int timeout)
        {
            return await this.Connect(address.ToString(), port, timeout);
        }

        public async Task<StreamSocket> Connect(string address, int port)
        {
            StreamSocket socket = new StreamSocket();

            await socket.ConnectAsync(new HostName(address), port.ToString());
            
            return socket;
        }

        /// <summary>
        /// </summary>
        /// <param name="address"></param>
        /// <param name="port"></param>
        /// <param name="timeout">in milliseconds</param>
        /// <returns></returns>
        public async Task<StreamSocket> Connect(string address, int port, int timeout)
        {
            StreamSocket socket = new StreamSocket();

            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(timeout);

            var connectAsync = socket.ConnectAsync(new HostName(address), port.ToString());
            var connectTask = connectAsync.AsTask(cts.Token);

            await connectTask;

            return socket;
        }
    }
}

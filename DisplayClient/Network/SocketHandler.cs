using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Remote_Content_Show_Protocol;
using Windows.Storage.Streams;
using Remote_Content_Show_Container;

namespace DisplayClient.Network
{
    public class SocketHandler
    {
        private StreamSocket socket;

        private bool running;

        public SocketHandler(StreamSocket socket)
        {
            this.socket = socket;
        }

        public delegate void MessageBytesReceived(MessageCode code, byte[] bytes);

        public event MessageBytesReceived OnMessageBytesReceived;

        public void Start()
        {
            this.running = true;

            Task.Factory.StartNew(() =>
            {
                this.HandleSocket();
            });
        }

        public void Stop()
        {
            this.running = false;
        }

        public void Close()
        {
            this.socket.Dispose();
        }

        private async void HandleSocket()
        {
            DataReader reader = new DataReader(this.socket.InputStream);
            DataWriter writer = new DataWriter(this.socket.OutputStream);

            uint headerSize = Convert.ToUInt32(Remote_Content_Show_Header.HeaderLength);

            while (this.running)
            {
                if (await reader.LoadAsync(headerSize) == headerSize)
                {
                    byte[] headerBytes = new byte[headerSize];

                    reader.ReadBytes(headerBytes);

                    Remote_Content_Show_Header header = Remote_Content_Show_Header.FromByte(headerBytes);

                    uint contentSize = Convert.ToUInt32(header.Length);

                    if (await reader.LoadAsync(contentSize) == contentSize)
                    {
                        byte[] contentBytes = new byte[contentSize];

                        reader.ReadBytes(contentBytes);

                        if (this.OnMessageBytesReceived != null)
                        {
                            this.OnMessageBytesReceived(header.Code, contentBytes);
                        }
                    }
                }
                else
                {
                    await Task.Delay(1000);
                }
            }
        }
    }
}

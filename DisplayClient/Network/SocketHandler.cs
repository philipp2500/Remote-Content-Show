using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Remote_Content_Show_Protocol;
using Windows.Storage.Streams;
using Remote_Content_Show_Container;

namespace DisplayClient
{
    public class SocketHandler
    {
        private StreamSocket socket;

        private DataReader reader;
        private DataWriter writer;

        public struct SocketMessage
        {
            public MessageCode Code;
            public byte[] Content;
            public bool Empty;
        }

        private bool running;

        public SocketHandler(StreamSocket socket)
        {
            this.socket = socket;

            this.reader = new DataReader(this.socket.InputStream);
            this.writer = new DataWriter(this.socket.OutputStream);
        }

        public delegate void MessageBytesReceived(MessageCode code, byte[] bytes);

        public delegate void ConnectionLost();

        public event MessageBytesReceived OnMessageBytesReceived;

        public event ConnectionLost OnConnectionLost;

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
            try
            {
                this.Stop();
                this.socket.Dispose();
            }
            catch (Exception)
            {
            }
        }

        public async void SendMessage(MessageCode msgCode, byte[] data)
        {
            Remote_Content_Show_Header header = new Remote_Content_Show_Header(msgCode, data.Length, RemoteType.Client);
            byte[] headerBytes = header.ToByte;

            try
            {
                this.writer.WriteBytes(headerBytes);
                this.writer.WriteBytes(data);

                await this.writer.StoreAsync();

                await this.writer.FlushAsync();
            }
            catch (Exception)
            {
                this.Stop();

                if (this.OnConnectionLost != null)
                {
                    this.OnConnectionLost();
                }
            }
        }

        public async Task<SocketMessage> WaitForMessage()
        {
            uint headerSize = Convert.ToUInt32(Remote_Content_Show_Header.HeaderLength);

            SocketMessage msg;
            msg.Code = MessageCode.MC_Alive;
            msg.Content = new byte[] { };
            msg.Empty = true;

            try
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

                        msg.Code = header.Code;
                        msg.Content = contentBytes;
                        msg.Empty = false;
                    }
                }
            }
            catch (Exception ex)
            {
                this.Stop();

                if (this.OnConnectionLost != null)
                {
                    this.OnConnectionLost();
                }
            }

            return msg;
        }

        private async void HandleSocket()
        {
            while (this.running)
            {
                SocketMessage msg = await this.WaitForMessage();

                if (!msg.Empty)
                {
                    if (this.OnMessageBytesReceived != null)
                    {
                        this.OnMessageBytesReceived(msg.Code, msg.Content);
                    }
                }
            }

            /*DataReader reader = new DataReader(this.socket.InputStream);
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
            }*/
        }
    }
}

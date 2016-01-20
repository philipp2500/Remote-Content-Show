using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using Remote_Content_Show_Protocol;
using Remote_Content_Show_Container;
using System.Threading;

namespace ConfigManager
{
    public class NetworkConnection
    {
        public event EventHandler<MessageRecivedEventHandler> OnMessageReceived;
        public event EventHandler<ErrorEventHandler> OnError;

        private TcpClient client;
        private Thread thread;
        private NetworkStream stream;
        private bool run = true;

        public NetworkConnection(IPAddress ip)
        {
            try
            {
                this.client = new TcpClient();
                this.client.Connect(ip, NetworkConfiguration.Port);
                this.stream = this.client.GetStream();
            }
            catch
            {
                this.FireOnError("Fehler beim Verbinden.");
            }

            this.thread = new Thread(this.Run);
            this.thread.IsBackground = true;
        }

        public void Write(byte[] messageData, MessageCode code)
        {
            Remote_Content_Show_Header header = new Remote_Content_Show_Header(code, messageData.Length);
            this.stream.Write(header.ToByte, 0, Remote_Content_Show_Header.HeaderLength);
            this.stream.Write(messageData, 0, messageData.Length);
        }

        public void Close()
        {
            this.run = false;
            this.OnError = null;
            this.OnMessageReceived = null;

            this.stream.Close();
            this.client.Close();
        }

        protected void FireOnMessageReceived(byte[] messageData, MessageCode code)
        {
            if (this.OnMessageReceived != null)
            {
                OnMessageReceived(this, new MessageRecivedEventHandler(messageData, code));
            }
        }

        protected void FireOnError(string message)
        {
            if (this.OnError != null)
            {
                OnError(this, new ErrorEventHandler(message));
            }
        }

        private void Run()
        {
            try
            {
                while (this.run)
                {
                    byte[] data = new byte[Remote_Content_Show_Header.HeaderLength];
                    this.stream.Read(data, 0, data.Length);
                    Remote_Content_Show_Header header = Remote_Content_Show_Header.FromByte(data);

                    data = new byte[header.Length];
                    this.stream.Read(data, 0, data.Length);
                    this.FireOnMessageReceived(data, header.Code);
                }
            }
            catch
            {
                this.FireOnError("Fehler beim Empfangen der Daten.");
            }                
        }
    }
}

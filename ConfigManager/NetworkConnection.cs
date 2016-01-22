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
        private IPAddress ip;
        private int port;

        public NetworkConnection(IPAddress ip, int port)
        {
            this.ip = ip;
            this.port = port;
            this.client = new TcpClient();

            this.thread = new Thread(this.Run);
            this.thread.IsBackground = true;
        }

        public void Connect()
        {                     
            try
            {
                this.client.Connect(this.ip, this.port);
                this.stream = this.client.GetStream();
                this.thread.Start();
            }
            catch
            {
                this.FireOnError("Fehler beim Verbinden.");
            }
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

            try
            {
                this.client.Close();
                this.stream.Close();
            } 
            catch
            {

            }
        }

        protected void FireOnMessageReceived(byte[] messageData, MessageCode code, IPAddress ip)
        {
            if (this.OnMessageReceived != null)
            {
                OnMessageReceived(this, new MessageRecivedEventHandler(messageData, code, ip));
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
                if (this.run)
                {
                    byte[] data = new byte[Remote_Content_Show_Header.HeaderLength];
                    this.stream.Read(data, 0, data.Length);
                    Remote_Content_Show_Header header = Remote_Content_Show_Header.FromByte(data);

                    data = new byte[header.Length];
                    this.stream.Read(data, 0, data.Length);
                    this.FireOnMessageReceived(data, header.Code, this.ip);
                }                
            }
            catch
            {
                if (this.run)
                {
                    this.FireOnError("Fehler beim Empfangen der Daten.");
                }
            }

            this.Close();
        }
    }
}

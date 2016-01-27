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
        private int port;

        public NetworkConnection(IPAddress ip, int port)
        {
            this.Ip = ip;
            this.port = port;
            this.client = new TcpClient();

            this.thread = new Thread(this.Run);
            this.thread.IsBackground = true;
        }

        public IPAddress Ip
        {
            get;
            private set;
        }

        public void Connect()
        {                     
            try
            {
                this.client.Connect(this.Ip, this.port);
                this.stream = this.client.GetStream();                
            }
            catch
            {
                this.FireOnError("Fehler beim Verbinden.");
            }
        }

        public void StartRead()
        {
            this.thread.Start();
        }

        public void Write(byte[] messageData, MessageCode code)
        {
            Remote_Content_Show_Header header = new Remote_Content_Show_Header(code, messageData.Length, RemoteType.Configurator);
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
                    List<byte> erg = new List<byte>();
                    byte[] data = new byte[Remote_Content_Show_Header.HeaderLength];
                    this.stream.Read(data, 0, data.Length);
                    Remote_Content_Show_Header header = Remote_Content_Show_Header.FromByte(data);

                    while (erg.Count() < header.Length)
                    {
                        data = new byte[header.Length];
                        int selected = this.stream.Read(data, 0, data.Length);
                        erg.AddRange(data.Where((x, i) => i < selected));
                    }

                    this.FireOnMessageReceived(erg.ToArray(), header.Code, this.Ip);
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Remote_Content_Show_Protocol;
using Remote_Content_Show_Container;

namespace ConfigManager
{
    public class NetworkManager
    {
        public event EventHandler<MessageRecivedEventHandler> OnMessageReceived;
        public event EventHandler<ErrorEventHandler> OnError;

        private List<NetworkConnection> connections = new List<NetworkConnection>();

        public NetworkConnection ConnectToRead(IPAddress ip, int port)
        {
            NetworkConnection netCon = new NetworkConnection(ip, port);
            netCon.OnError += NetCon_OnError;
            netCon.OnMessageReceived += NetCon_OnMessageReceived;
            netCon.Connect();
            netCon.StartRead();
            this.connections.Add(netCon);

            return netCon;
        }

        public NetworkConnection ConnectTo(IPAddress ip, int port)
        {
            NetworkConnection netCon = new NetworkConnection(ip, port);
            netCon.OnError += NetCon_OnError;
            netCon.OnMessageReceived += NetCon_OnMessageReceived;
            netCon.Connect();
            this.connections.Add(netCon);

            return netCon;
        }

        public void Write(NetworkConnection netCon, byte[] messageData, MessageCode code)
        {
            netCon.Write(messageData, code);
        }

        public void CloseAllConnection()
        {
            foreach(NetworkConnection c in this.connections)
            {
                c.Close();
            }
            this.connections = new List<NetworkConnection>();
        }

        protected void FireOnMessageReceived(MessageRecivedEventHandler e)
        {
            if (this.OnMessageReceived != null)
            {
                OnMessageReceived(this, e);
            }
        }

        protected void FireOnError(ErrorEventHandler e)
        {
            if (this.OnError != null)
            {
                OnError(this, e);
            }
        }

        private void NetCon_OnMessageReceived(object sender, MessageRecivedEventHandler e)
        {
            this.FireOnMessageReceived(e);
        }

        private void NetCon_OnError(object sender, ErrorEventHandler e)
        {
            this.connections.Remove((NetworkConnection)sender);
            this.FireOnError(e);
        }
    }
}

using Remote_Content_Show_Container;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Sockets;

namespace DisplayClient.DisplayManager
{
    public class ExternalAdmin
    {
        private SocketHandler socketHandler;

        public ExternalAdmin(StreamSocket socket)
        {
            this.socketHandler = new SocketHandler(socket);

            this.socketHandler.OnMessageBytesReceived += SocketHandler_OnMessageBytesReceived;
        }

        private void SocketHandler_OnMessageBytesReceived(Remote_Content_Show_Container.MessageCode code, byte[] bytes)
        {
            if (code == MessageCode.MC_Job)
            {

            }
        }
    }
}

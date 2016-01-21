using Remote_Content_Show_Container;
using System.Net;

namespace ConfigManager
{
    public class MessageRecivedEventHandler
    {
        public MessageRecivedEventHandler(byte[] messageData, MessageCode code, IPAddress ip)
        {
            this.MessageData = messageData;
            this.Code = code;
            this.Ip = ip;
        }

        public IPAddress Ip
        {
            get;
            private set;
        }

        public MessageCode Code
        {
            get;
            private set;
        }

        public byte[] MessageData
        {
            get;
            private set;
        }
    }
}
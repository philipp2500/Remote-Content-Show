using Remote_Content_Show_Container;

namespace ConfigManager
{
    public class MessageRecivedEventHandler
    {
        public MessageRecivedEventHandler(byte[] messageData, MessageCode code)
        {
            this.MessageData = messageData;
            this.Code = code;
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
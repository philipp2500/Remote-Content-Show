using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remote_Content_Show_Protocol
{
    public class Remote_Content_Show_Message
    {
        public Remote_Content_Show_Message(IRemote_Content_Show_MessageContent content)
        {
            this.Content = content;
        }

        public Guid Id
        {
            get;
            private set;
        }

        public const double Version = 1.0;

        public IRemote_Content_Show_MessageContent Content
        {
            get;
            private set;
        }
    }
}

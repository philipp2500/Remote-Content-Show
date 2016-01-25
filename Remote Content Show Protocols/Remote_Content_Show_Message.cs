using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Remote_Content_Show_Container;

namespace Remote_Content_Show_Protocol
{
    public class Remote_Content_Show_Message
    {
        public Remote_Content_Show_Message(RemoteType remote)
        {
            this.Id = Guid.NewGuid();
            this.Remote = remote;
        }

        public Guid Id
        {
            get;
            private set;
        }

        public RemoteType Remote
        {
            get;
            private set;
        }

        public double Version = 1.0;
    }
}

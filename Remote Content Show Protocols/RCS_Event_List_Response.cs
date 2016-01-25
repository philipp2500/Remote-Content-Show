using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

using Remote_Content_Show_Container;

namespace Remote_Content_Show_Protocol
{
    public class RCS_Event_List_Response : Remote_Content_Show_Message
    {
        public RCS_Event_List_Response(Event_List event_List, RemoteType remote) : base(remote)
        {
            this.Event_List = event_List;
        }

        public Event_List Event_List
        {
            get;
            private set;
        }
    }
}

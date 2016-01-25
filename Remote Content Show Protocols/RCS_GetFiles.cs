using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Remote_Content_Show_Container;
using System.Threading.Tasks;

namespace Remote_Content_Show_Protocol
{
    public class RCS_GetFiles : Remote_Content_Show_Message
    {
        public RCS_GetFiles(RemoteType remote) : base(remote)
        {
        }
    }
}

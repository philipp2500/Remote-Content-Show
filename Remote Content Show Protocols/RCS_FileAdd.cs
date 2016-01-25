using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Remote_Content_Show_Container;

namespace Remote_Content_Show_Protocol
{
    public class RCS_FileAdd : Remote_Content_Show_Message
    {
        public RCS_FileAdd(RemoteType remote) : base(remote)
        {
        }
    }
}

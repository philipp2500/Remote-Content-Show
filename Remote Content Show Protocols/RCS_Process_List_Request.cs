﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Remote_Content_Show_Container;

namespace Remote_Content_Show_Protocol
{
    public class RCS_Process_List_Request : Remote_Content_Show_Message
    {
        public RCS_Process_List_Request(RemoteType remote) : base(remote)
        {
        }
    }
}

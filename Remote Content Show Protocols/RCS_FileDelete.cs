﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Remote_Content_Show_Container;

namespace Remote_Content_Show_Protocol
{
    public class RCS_FileDelete : Remote_Content_Show_Message
    {
        public RCS_FileDelete(RemoteType remote) : base(remote)
        {
        }

        public string Name
        {
            get;
            set;
        }
    }
}

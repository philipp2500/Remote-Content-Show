﻿using Remote_Content_Show_Container;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remote_Content_Show_Protocol
{
    public class RCS_Job : IRemote_Content_Show_MessageContent
    {

        public Job_Configuration Configuration
        {
            get;
            private set;
        }

        public string TypeToString()
        {
            return this.GetType().ToString();
        }       
    }
}

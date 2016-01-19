﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remote_Content_Show_Container
{
    public class Job
    {
        public int Order
        {
            get;
            set;
        }

        public int WindowLayoutNumber
        {
            get;
            set;
        }

        public IResource Resource
        {
            get;
            set;
        }
    }
}
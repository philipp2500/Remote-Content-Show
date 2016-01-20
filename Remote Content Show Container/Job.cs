﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remote_Content_Show_Container
{
    public class Job
    {
        public int OrderingNumber
        {
            get;
            set;
        }

        public int Duration
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

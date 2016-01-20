﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Remote_Content_Show_Container
{
    public class ProcessDescription
    {

        public ProcessDescription(byte[] windowPicture, int processId)
        {
            this.WindowPicture = windowPicture;
            this.ProcessId = processId;
        }
            

        public byte[] WindowPicture
        {
            get;
            private set;
        }

        public int ProcessId
        {
            get;
            private set;
        }
    }
}

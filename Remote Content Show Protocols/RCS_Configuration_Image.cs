﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Remote_Content_Show_Container;

namespace Remote_Content_Show_Protocol
{
    public class RCS_Configuration_Image : Remote_Content_Show_Message
    {
        public RCS_Configuration_Image(byte[] picture, RemoteType remote) : base(remote)
        {
            this.Picture = picture;
        }

        public byte[] Picture
        {
            get;
            private set;
        }
    }
}

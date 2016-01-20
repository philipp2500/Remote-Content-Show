using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remote_Content_Show_Protocol
{
    public class RSC_Configuration_Image : Remote_Content_Show_Message
    {
        public RSC_Configuration_Image(byte[] picture)
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Remote_Content_Show_Container.Enums;

namespace Remote_Content_Show_Protocols
{
    
    public class Remote_Content_Show_Header
    {
        public const string ProtocolName = "RCS";  

        public Remote_Content_Show_Header(MessageCode code, long length)
        {
            this.Length = length;
        }
        
        public MessageCode Code
        {
            get;
            set;
        }     

        public long Length
        {
            get;
            set;
        }

        public byte[] ToByte
        {
            get
            {
                List<byte> erg
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remote_Content_Show_Protocols
{
    
    public class Remote_Content_Show_Header
    {
        public const string ProtocolName = "RCS";   
        
        public Remote_Content_Show_Header(long length)
        {
            this.Length = length;
        }     

        public long Length
        {
            get;
            set;
        }

        public byte[] To
    }
}

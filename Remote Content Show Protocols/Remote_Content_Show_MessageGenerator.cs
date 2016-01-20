using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remote_Content_Show_Protocols
{
    public static class Remote_Content_Show_MessageGenerator
    {
        public static Remote_Content_Show_Header GetHeaderFromByte(byte[] data)
        {

        }

        public static Remote_Content_Show_Header CreateHeader(byte[] message)
        {
            return new Remote_Content_Show_Header(message.Length);
        }

        public static Remote_Content_Show_Header GetHeaderFromByte(byte[] data)
        {
        }
    }
}

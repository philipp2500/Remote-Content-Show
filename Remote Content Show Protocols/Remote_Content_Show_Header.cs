using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Remote_Content_Show_Container;

namespace Remote_Content_Show_Protocol
{
    
    public class Remote_Content_Show_Header
    {
        public static int HeaderLength = 15;

        public const string ProtocolName = "RCS";  

        public Remote_Content_Show_Header(MessageCode code, long length)
        {
            this.Length = length;
        }
        
        public MessageCode Code
        {
            get;
            private set;
        }     

        public long Length
        {
            get;
            private set;
        }

        public static Remote_Content_Show_Header FromByte(byte[] data)
        {
            MessageCode code = (MessageCode)BitConverter.ToInt32(data, 3);
            long length = (long)BitConverter.ToUInt64(data, 10);

            return new Remote_Content_Show_Header(code, length);
        }

        public static bool IsValidHeader(byte[] data)
        {
            if (data.Length == 15)
            {
                if (Encoding.UTF8.GetString(data, 0, 3) == ProtocolName)
                {
                    return true;
                }
            }

            return false;
        }

        public byte[] ToByte
        {
            get
            {
                List<byte> erg = new List<byte>();
                erg.AddRange(Encoding.UTF8.GetBytes(ProtocolName));
                erg.AddRange(BitConverter.GetBytes((int)this.Code));
                erg.AddRange(BitConverter.GetBytes(this.Length));

                return erg.ToArray();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remote_Content_Show_Protocol
{
    public class Remote_Content_Show_Message
    {
        public Remote_Content_Show_Message()
        {
            this.Id = Guid.NewGuid();
        }

        public Guid Id
        {
            get;
            private set;
        }

        public double Version = 1.0;
    }
}

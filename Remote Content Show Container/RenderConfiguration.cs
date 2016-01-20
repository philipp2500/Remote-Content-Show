using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remote_Content_Show_Container
{
    public class RenderConfiguration
    {
        public Guid RenderJobID
        {
            get;
            set;
        }

        public int RenderWidth
        {
            get;
            set;
        }

        public int RenderHeight
        {
            get;
            set;
        }

        public double UpdateInterval
        {
            get;
            set;
        }
    }
}

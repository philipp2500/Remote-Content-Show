using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remote_Content_Show_Container
{
    public class WindowLayout
    {
        public WindowLayout()
        {
            this.Items = new List<LayoutItem>();
        }

        public string Name
        {
            get;
            set;
        }

        public byte[] Color
        {
            get;
            set;
        }

        public List<LayoutItem> Items
        {
            get;
            set;
        }
    }
}

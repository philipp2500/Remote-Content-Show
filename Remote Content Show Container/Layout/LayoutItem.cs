using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remote_Content_Show_Container
{
    public class LayoutItem
    {
        public LayoutItem(double marginLeft, double marginTop, double width, double height)
        {
            this.MarginLeft = marginLeft;
            this.MarginTop = marginTop;
            this.Width = width;
            this.Height = height;
        }

        public double MarginLeft
        {
            get;
            private set;
        }

        public double MarginTop
        {
            get;
            private set;
        }
        public double Width
        {
            get;
            private set;
        }

        public double Height
        {
            get;
            private set;
        }

    }
}

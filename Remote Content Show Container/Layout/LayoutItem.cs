using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remote_Content_Show_Container
{
    public class LayoutItem
    {
        public LayoutItem(double marginLeft, double marginTop, double width, double height, int number)
        {
            this.MarginLeft = marginLeft;
            this.MarginTop = marginTop;
            this.Width = width;
            this.Height = height;
            this.Number = number;
        }

        public int Number
        {
            get;
            set;
        }

        public double MarginLeft
        {
            get;
            set;
        }

        public double MarginTop
        {
            get;
            set;
        }
        public double Width
        {
            get;
            set;
        }

        public double Height
        {
            get;
            set;
        }
    }
}

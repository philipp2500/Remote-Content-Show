﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remote_Content_Show_Container
{
    public static class LayoutHelper
    {
        public static double GetActuelValue(double percent, double sizeParent)
        {
            return sizeParent * percent;
        }
    }
}

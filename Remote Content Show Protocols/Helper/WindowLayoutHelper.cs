using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Remote_Content_Show_Container;

namespace Remote_Content_Show_Protocol
{
    public static class WindowLayoutHelper
    {
        public static int GetWindows(WindowLayout code)
        {
            return GetMap()[code];
        }

        private static Dictionary<WindowLayout, int> GetMap()
        {
            Dictionary<WindowLayout, int> dict = new Dictionary<WindowLayout, int>();

            dict.Add(WindowLayout.SingleWindow, 1);
            dict.Add(WindowLayout.DoubleWindowVertikalSplitted, 2);
            dict.Add(WindowLayout.DoubleWindowHorizontalSplitted, 2);

            return dict;
        }
    }
}

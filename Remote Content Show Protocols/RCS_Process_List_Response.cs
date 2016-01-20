using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Remote_Content_Show_Container;

namespace Remote_Content_Show_Protocol
{
    public class RCS_Process_List_Response : Remote_Content_Show_Message
    {
        public Process_List ProcesseList
        {
            get;
            set;
        }
    }
}

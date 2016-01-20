using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remote_Content_Show_Container
{
    public class Event
    {
        public Job_EventType Type
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public Guid ConcernedJobID
        {
            get;
            set;
        }
    }
}

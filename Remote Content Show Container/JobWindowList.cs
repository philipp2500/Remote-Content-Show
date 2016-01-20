using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remote_Content_Show_Container
{
    public class JobWindowList
    {
        public JobWindowList()
        {
            this.Jobs = new List<Job>();
        }

        public bool Looping
        {
            get;
            set;
        }

        public List<Job> Jobs
        {
            get;
            set;
        }

    }
}

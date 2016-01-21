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

        /// <summary>
        /// If the PI has to cancel the job, the agent also has to cancel the job.
        /// </summary>
        public Guid JobID
        {
            get;
            set;
        }

        public Job JobToDOo
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

        public bool IgnoreEqualImages
        {
            get;
            set;
        }
    }
}

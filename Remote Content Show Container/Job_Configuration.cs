using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Remote_Content_Show_Container
{
    public class Job_Configuration
    {
        public Job_Configuration()
        {
            this.Jobs = new Dictionary<int, JobWindowList>();
            this.Agents = new List<Agent>();
        }

        /// <summary>
        // Find out connection between the job and the error events.
        /// </summary>
        public Guid JobID
        {
            get;
            set;
        }

        /// <summary>
        /// int = WindowLayoutNumber vom Job
        /// </summary>
        public Dictionary<int, JobWindowList> JobLists
        {
            get;
            private set;
        }

        public WindowLayout Layout
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public List<Agent> Agents
        {
            get;
            set;
        }

    }
}

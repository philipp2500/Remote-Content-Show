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
            this.JobLists = new Dictionary<int, JobWindowList>();
            this.Agents = new List<Agent>();
            this.JobID = Guid.NewGuid();
        }

        /// <summary>
        // Find out connection between the job and the error events.
        /// </summary>
        public Guid JobID
        {
            get;
            private set;
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

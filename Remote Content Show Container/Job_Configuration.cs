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
            this.Jobs = new Dictionary<int, List<Job>>();
            this.Agents = new List<Agent>();
        }

        public Dictionary<int, List<Job>> Jobs
        {
            get;
            private set;
        }

        public WindowLayout Layout
        {
            get;
            set;
        }

        public bool Looping
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Remote_Content_Show_Container;

namespace ConfigManager
{
    public class ProcessDescriptionWrapper
    {
        public ProcessDescriptionWrapper(ProcessDescription pdescription, IPAddress ip, string agentName)
        {
            this.Id = Guid.NewGuid();
            this.PDescription = pdescription;
            this.Ip = ip;
            this.AgentName = agentName;
        }

        public ProcessDescription PDescription
        {
            get;
            private set;
        }

        public Guid Id
        {
            get;
            private set;
        }

        public IPAddress Ip
        {
            get;
            private set;
        }

        public string AgentName
        {
            get;
            private set;
        }
    }
}

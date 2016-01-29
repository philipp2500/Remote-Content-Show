using Remote_Content_Show_Container;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisplayClient
{
    public class AgentSelector
    {
        private Agent currentCompatibleAgent;

        public AgentSelector(Job jobToDo, List<Agent> availableAgents)
        {
            this.JobToDo = jobToDo;
            this.AvailableAgents = availableAgents.ToList();
        }

        public delegate void AgentNotReachable(AgentSelector sender, Agent agent);

        public event AgentNotReachable OnAgentNotReachable;

        public Job JobToDo
        {
            get;
            private set;
        }

        public List<Agent> AvailableAgents
        {
            get;
            private set;
        }

        public async Task<WorkingAgent> GetCompatibleAgent(RenderConfiguration configuration)
        {
            List<Agent> allAgents = new List<Agent>() { };

            if (currentCompatibleAgent != null)
            {
                allAgents.Add(currentCompatibleAgent);
            }

            allAgents.AddRange(this.AvailableAgents.ToList());

            return await this.GetCompatibleAgentFromList(configuration, allAgents);
        }

        public async Task<WorkingAgent> GetCompatibleAgentFromList(RenderConfiguration configuration, List<Agent> list)
        {
            WorkingAgent compatible = null;            

            foreach (Agent info in list)
            {
                if (compatible == null)
                {
                    WorkingAgent worker = new WorkingAgent(configuration);

                    try
                    {
                        RenderMessage msg = await worker.Connect(info);

                        if (msg == RenderMessage.Supported)
                        {
                            compatible = worker;
                            this.currentCompatibleAgent = info;
                        }
                    }
                    catch (AgentNotReachableException)
                    {
                        if (this.OnAgentNotReachable != null)
                        {
                            this.OnAgentNotReachable(this, info);
                        }
                    }
                }
            }

            return compatible;
        }
    }
}

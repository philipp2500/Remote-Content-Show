using Remote_Content_Show_Container;
using Remote_Content_Show_Container.Resouces;
using Remote_Content_Show_Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml.Media.Imaging;

namespace DisplayClient
{
    public class ContentDisplayManager
    {
        private Size renderSize;

        private JobWindowList jobForWindow;

        private CoreDispatcher dispatcher;

        private List<Agent> availableAgents;

        private List<WorkingAgent> workingAgents;

        public ContentDisplayManager(JobWindowList jobForWindow)
        {
            this.jobForWindow = jobForWindow;

            this.availableAgents = new List<Agent>();
            this.workingAgents = new List<WorkingAgent>();

            this.jobForWindow = new JobWindowList();
            this.jobForWindow.Jobs.Add(new Job() { Duration = 5, OrderingNumber = 1, Resource = new FileResource() { Path = "http://www.google.at" } });
            this.jobForWindow.Jobs.Add(new Job() { Duration = 5, OrderingNumber = 1, Resource = new FileResource() { Path = "http://img.pr0gramm.com/2016/01/20/341c9285b24bd3f8.jpg" } });
        }

        // Display request events

        public delegate void ImageDisplayRequested(BitmapImage image);

        public delegate void WebsiteDisplayRequested(Uri uri);

        public event ImageDisplayRequested OnImageDisplayRequested;

        public event WebsiteDisplayRequested OnWebsiteDisplayRequested;

        // Error events

        public delegate void ResourceNotSupportedByAgent(Job job, Agent agent);

        public delegate void AgentWithProcessNotFound(Job job);

        public event ResourceNotSupportedByAgent OnResourceNotSupportedByAgent;

        public event AgentWithProcessNotFound OnAgentWithProcessNotFound;

        public List<Agent> AvailableAgents
        {
            get
            {
                return this.availableAgents;
            }

            set
            {
                this.availableAgents = value;
            }
        }

        public void SetRenderSize(Size size)
        {
            this.renderSize = size;
        }

        public CoreDispatcher Dispatcher
        {
            set
            {
                this.dispatcher = value;
            }
        }

        public void Start()
        {            
            if (jobForWindow != null)
            {
                Task.Factory.StartNew(async () =>
                {
                    List<Job> ordered = jobForWindow.Jobs.OrderBy(x => x.OrderingNumber).ToList();
                    
                    do
                    {
                        foreach (Job job in ordered)
                        {
                            this.RunJob(job);

                            await Task.Delay(job.Duration * 1000);


                        }
                    }
                    while (true);
                });
            }
        }

        public void Cancel()
        {
            foreach (WorkingAgent agent in this.workingAgents)
            {
                agent.CancelRenderJob();
            }
        }

        private void RunJob(Job job)
        {
            if (job.Resource is FileResource)
            {
                FileResource fr = (FileResource)job.Resource;

                this.HandleFileResource(fr);
            }
            else if (job.Resource is ProcessResource)
            {
                ProcessResource pr = (ProcessResource)job.Resource;

                this.HandleProcessResource(job, pr);
            }
        }

        private async void HandleFileResource(FileResource resource)
        {
            if (resource.Path.EndsWith(".jpg"))
            {
                if (this.OnImageDisplayRequested != null)
                {
                    await this.dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        BitmapImage img = new BitmapImage(new Uri(resource.Path));

                        this.OnImageDisplayRequested(img);
                    });
                }
            }
            else if (this.OnWebsiteDisplayRequested != null)
            {
                await this.dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    this.OnWebsiteDisplayRequested(new Uri(resource.Path));
                });
            }
        }

        private void HandleProcessResource(Job job, ProcessResource resource)
        {
            List<Agent> foundAgents = this.availableAgents.Where(x => x.IP.Equals(resource.ProcessAgent.IP)).ToList();

            if (foundAgents.Count == 1)
            {

            }
            else
            {
                if (this.OnAgentWithProcessNotFound != null)
                {
                    this.OnAgentWithProcessNotFound(job);
                }
            }
        }

        private async void HandleAgent(Agent agent, RenderConfiguration configuration)
        {
            WorkingAgent worker = new WorkingAgent(null);

            try
            {
                RenderMessage msg = await worker.Connect(agent);

                if (msg == RenderMessage.Supported)
                {
                    worker.OnResultReceived += Worker_OnResultReceived;
                    

                    this.workingAgents.Add(worker);
                }
                else if (msg == RenderMessage.NotSupported)
                {
                    if (this.OnResourceNotSupportedByAgent != null)
                    {
                        this.OnResourceNotSupportedByAgent(configuration.JobToDo, agent);
                    }
                }
            }
            catch (AgentNotReachableException)
            {
                if (this.OnAgentWithProcessNotFound != null)
                {
                    this.OnAgentWithProcessNotFound(configuration.JobToDo);
                }
            }
        }

        private void Worker_OnResultReceived(RCS_Render_Job_Result result)
        {

        }
    }
}

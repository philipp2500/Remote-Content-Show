using Remote_Content_Show_Container;
//using Remote_Content_Show_Container.Resouces;
using Remote_Content_Show_Protocol;
using System;
using System.Collections.Generic;
using System.IO;
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

        private CoreDispatcher displayDispatcher;

        private List<Agent> availableAgents;

        private List<WorkingAgent> workingAgents;

        private Job currentlyTreatedJob;

        public ContentDisplayManager(JobWindowList jobForWindow)
        {
            this.jobForWindow = jobForWindow;

            this.availableAgents = new List<Agent>();
            this.workingAgents = new List<WorkingAgent>();

            this.jobForWindow = new JobWindowList();
            this.jobForWindow.Jobs.Add(new Job() { Duration = 5, OrderingNumber = 1, Resource = new WebResource() { Path = "http://www.google.at" } });
            this.jobForWindow.Jobs.Add(new Job() { Duration = 5, OrderingNumber = 2, Resource = new FileResource() { Path = "http://img.pr0gramm.com/2016/01/20/341c9285b24sadfasdfbd3f8.jpg" } });
            this.jobForWindow.Jobs.Add(new Job() { Duration = 5, OrderingNumber = 3, Resource = new WebResource() { Path = "http://www.google.at" } });
            this.jobForWindow.Jobs.Add(new Job() { Duration = 5, OrderingNumber = 4, Resource = new FileResource() { Path = "http://img.pr0gramm.com/2016/01/20/341c9285b24bd3f8.jpg" } });
        }

        // Display request events

        public delegate void ImageDisplayRequested(BitmapImage image);

        public delegate void WebsiteDisplayRequested(Uri uri);

        public delegate void DisplayAbortRequested();

        public event ImageDisplayRequested OnImageDisplayRequested;

        public event WebsiteDisplayRequested OnWebsiteDisplayRequested;

        public event DisplayAbortRequested OnDisplayAbortRequested;

        // Error events

        public delegate void NoResourceCompatibleAgentFound(IResource resource);

        public delegate void ResourceNotAvailable(IResource resource);

        public delegate void AgentNotReachable(Job job, Agent agent);

        public delegate void AgentWithProcessNotFound(Job job);

        public event NoResourceCompatibleAgentFound OnNoResourceCompatibleAgentFound;

        public event ResourceNotAvailable OnResourceNotAvailable;

        public event AgentNotReachable OnAgentNotReachable;

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

        public CoreDispatcher DisplayDispatcher
        {
            set
            {
                this.displayDispatcher = value;
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
                            this.currentlyTreatedJob = job;

                            this.RunJob(job);

                            await Task.Delay(job.Duration * 1000);

                            this.CancelJob(job);
                        }
                    }
                    while (false);
                });
            }
        }

        public void Cancel()
        {
            this.CancelJob(this.currentlyTreatedJob);

            this.currentlyTreatedJob = null;
        }

        private async void CancelJob(Job job)
        {
            // TODO
            if (this.OnDisplayAbortRequested != null)
            {
                await this.displayDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    this.OnDisplayAbortRequested();
                });
            }

            List<WorkingAgent> jobTreatingAgents = this.workingAgents.Where(x => x.Configuration.JobToDo.OrderingNumber == job.OrderingNumber).ToList();

            foreach (WorkingAgent agent in jobTreatingAgents)
            {
                agent.CancelRenderJob();
            }

            this.workingAgents.RemoveAll(x => jobTreatingAgents.Contains(x));
        }

        private void RunJob(Job job)
        {
            if (job.Resource is FileResource)
            {
                FileResource fr = (FileResource)job.Resource;

                this.HandleFileResource(fr);
            }
            else if (job.Resource is WebResource)
            {
                WebResource wr = (WebResource)job.Resource;

                this.HandleWebResource(wr);
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
                    await this.displayDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        BitmapImage img = new BitmapImage(new Uri(resource.Path));

                        this.OnImageDisplayRequested(img);
                    });
                }
            }
        }

        private async void HandleWebResource(WebResource resource)
        {
            if (this.OnWebsiteDisplayRequested != null)
            {
                await this.displayDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
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
                this.LookForResourceCompatibleAgent(foundAgents, resource);
            }
            else
            {
                if (this.OnAgentWithProcessNotFound != null)
                {
                    this.OnAgentWithProcessNotFound(job);
                }
            }
        }

        private async void LookForResourceCompatibleAgent(List<Agent> givenAgents, IResource resource)
        {
            RenderConfiguration configuration = null; // TODO
            bool found = false;

            foreach (Agent agent in givenAgents)
            {
                if (!found)
                {
                    WorkingAgent worker = new WorkingAgent(configuration);

                    try
                    {
                        RenderMessage msg = await worker.Connect(agent);

                        if (msg == RenderMessage.Supported)
                        {
                            worker.OnResultReceived += Worker_OnResultReceived;
                            worker.OnMessageReceived += Worker_OnMessageReceived;
                            worker.OnAgentGotUnreachable += Worker_OnAgentGotUnreachable;

                            this.workingAgents.Add(worker);
                        }
                    }
                    catch (AgentNotReachableException)
                    {
                        if (this.OnAgentNotReachable != null)
                        {
                            this.OnAgentNotReachable(configuration.JobToDo, agent);
                        }
                    }
                }
            }

            if (!found)
            {
                if (this.OnNoResourceCompatibleAgentFound != null)
                {
                    this.OnNoResourceCompatibleAgentFound(resource);
                }
            }
        }

        private void Worker_OnMessageReceived(WorkingAgent agent, RCS_Render_Job_Message message)
        {
            // TODO: handling a process, which has been exited suddenly.
            if (message.Message == RenderMessage.ProcessExited)
            {
                // Solution 1: Fuck it. We just say that the resource is not available anymore.
                /*if (this.OnResourceNotAvailable != null)
                {
                    this.OnResourceNotAvailable(agent.Configuration.JobToDo.Resource);
                }*/

                // Solution 2:
                this.LookForResourceCompatibleAgent(this.availableAgents.Where(x => !x.IP.Equals(agent.Agent.IP)).ToList(), agent.Configuration.JobToDo.Resource);
            }
        }

        private void Worker_OnAgentGotUnreachable(WorkingAgent agent)
        {
            if (this.OnAgentNotReachable != null)
            {
                this.OnAgentNotReachable(agent.Configuration.JobToDo, agent.Agent);
            }
        }

        private void Worker_OnResultReceived(WorkingAgent agent, RCS_Render_Job_Result result)
        {
            this.HandleImageResult(result.Picture);
        }

        private async void HandleImageResult(byte[] bytes)
        {
            if (this.OnImageDisplayRequested != null)
            {
                await this.displayDispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    BitmapImage image = new BitmapImage();

                    using (MemoryStream ms = new MemoryStream(bytes))
                    {
                        await image.SetSourceAsync(ms.AsRandomAccessStream());
                    }

                    this.OnImageDisplayRequested(image);
                });
            }
        } 
    }
}

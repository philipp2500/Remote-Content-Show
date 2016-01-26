using DisplayClient.Storage;
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
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml.Media.Imaging;

namespace DisplayClient
{
    public class ContentDisplayManager
    {
        private Size renderSize;

        private JobWindowList jobForWindow;

        private CoreDispatcher displayDispatcher;

        private List<WorkingAgent> workingAgents;

        private Job currentlyTreatedJob;

        private Dictionary<Job, AgentSelector> agentSelectors;

        // TODO: remove it
        private Guid jobID;

        public ContentDisplayManager(JobWindowList jobForWindow, List<Agent> availableAgents, Guid jobID)
        {
            this.jobForWindow = jobForWindow;
            this.jobID = jobID;

            this.workingAgents = new List<WorkingAgent>();
            this.agentSelectors = new Dictionary<Job, AgentSelector>();

            foreach (Job job in jobForWindow.Jobs)
            {
                AgentSelector selector = new AgentSelector(job, availableAgents);
                selector.OnAgentNotReachable += Selector_OnAgentNotReachable;

                this.agentSelectors[job] = selector;
            }

            /*this.jobForWindow.Jobs.Add(new Job() { Duration = 5, OrderingNumber = 1, Resource = new WebResource() { Path = "http://www.google.at" } });
            this.jobForWindow.Jobs.Add(new Job() { Duration = 5, OrderingNumber = 2, Resource = new FileResource() { Path = "http://img.pr0gramm.com/2016/01/20/341c9285b24sadfasdfbd3f8.jpg" } });
            this.jobForWindow.Jobs.Add(new Job() { Duration = 5, OrderingNumber = 3, Resource = new WebResource() { Path = "http://www.google.at" } });
            this.jobForWindow.Jobs.Add(new Job() { Duration = 5, OrderingNumber = 4, Resource = new FileResource() { Path = "http://img.pr0gramm.com/2016/01/20/341c9285b24bd3f8.jpg" } });*/
        }

        private void Selector_OnAgentNotReachable(AgentSelector sender, Agent agent)
        {
            if (this.OnAgentNotReachable != null)
            {
                this.OnAgentNotReachable(sender.JobToDo, agent);
            }
        }

        // Display request events

        public delegate void ImageDisplayRequested(BitmapImage image);

        public delegate void JobResultDisplayRequested(BitmapImage image);

        public delegate void VideoDisplayRequested(Uri videoPath);

        public delegate void WebsiteDisplayRequested(Uri uri);

        public delegate void DisplayAbortRequested();

        public event ImageDisplayRequested OnImageDisplayRequested;

        public event JobResultDisplayRequested OnJobResultDisplayRequested;

        public event VideoDisplayRequested OnVideoDisplayRequested;

        public event WebsiteDisplayRequested OnWebsiteDisplayRequested;

        public event DisplayAbortRequested OnDisplayAbortRequested;

        // Error events

        public delegate void NoResourceCompatibleAgentFound(Job job);

        public delegate void ResourceNotAvailable(Job job);

        public delegate void AgentNotReachable(Job job, Agent agent);

        public delegate void AgentWithProcessNotFound(Job job);

        public delegate void AgentAborted(Job job, Agent agent);

        public event NoResourceCompatibleAgentFound OnNoResourceCompatibleAgentFound;

        public event ResourceNotAvailable OnResourceNotAvailable;

        public event AgentNotReachable OnAgentNotReachable;

        public event AgentWithProcessNotFound OnAgentWithProcessNotFound;

        public event AgentAborted OnAgentAborted;

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
                    while (jobForWindow.Looping);
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

                if (!fr.Local)
                {
                    this.HandleFileResource(job, fr);
                }
                else
                {
                    this.HandleFileResource(job, new FileResource() { Path = Path.Combine(PersistenceManager.GetWriteablePath(), fr.Path) });
                }
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

        private async void HandleFileResource(Job job, FileResource resource)
        {
            // Todo: handle powerpoint files
            /*if (resource.Path.EndsWith(".jpg"))
            {
                if (this.OnImageDisplayRequested != null)
                {
                    await this.displayDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        BitmapImage img = new BitmapImage(new Uri(resource.Path));

                        this.OnImageDisplayRequested(img);
                    });
                }
            }*/

            if (CompatibilityManager.IsCompatibleImage(resource))
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
            else if (CompatibilityManager.IsCompatibleVideo(resource))
            {
                if (this.OnVideoDisplayRequested != null)
                {
                    await this.displayDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        this.OnVideoDisplayRequested(new Uri(resource.Path));
                    });
                }
            }
            else
            {
                RenderConfiguration config = this.GetNewRenderConfiguration(job);

                WorkingAgent worker = await this.agentSelectors[job].GetCompatibleAgent(config);

                this.HandleFoundWorkingAgent(job, worker);
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
        
        private async void HandleProcessResource(Job job, ProcessResource resource)
        {
            List<Agent> foundAgents = this.agentSelectors[job].AvailableAgents.Where(x => x.IP.Equals(resource.ProcessAgent.IP)).ToList();

            if (foundAgents.Count == 1)
            {
                //this.LookForResourceCompatibleAgent(foundAgents, resource);
                RenderConfiguration config = this.GetNewRenderConfiguration(job);
                WorkingAgent worker = await this.agentSelectors[job].GetCompatibleAgentFromList(config, foundAgents);

                this.HandleFoundWorkingAgent(job, worker);
            }
            else
            {
                if (this.OnAgentWithProcessNotFound != null)
                {
                    this.OnAgentWithProcessNotFound(job);
                }
            }
        }

        private async void Worker_OnMessageReceived(WorkingAgent agent, RCS_Render_Job_Message message)
        {
            // TODO: handling a process, which has been exited suddenly.
            if (message.Message == RenderMessage.ProcessExited)
            {
                Job todo = agent.Configuration.JobToDo;

                if (this.OnAgentAborted != null)
                {
                    this.OnAgentAborted(todo, agent.Agent);
                }

                // Solution 1: Fuck it. We just say that the resource is not available anymore.
                /*if (this.OnResourceNotAvailable != null)
                {
                    this.OnResourceNotAvailable(agent.Configuration.JobToDo.Resource);
                }*/

                // Solution 2: try it with another agent, except the current.
                //this.LookForResourceCompatibleAgent(this.availableAgents.Where(x => !x.IP.Equals(agent.Agent.IP)).ToList(), agent.Configuration.JobToDo.Resource);
                this.workingAgents.Remove(agent);

                List<Agent> excluded = this.agentSelectors[todo].AvailableAgents.Where(x => !x.IP.Equals(agent.Agent.IP)).ToList();

                RenderConfiguration config = this.GetNewRenderConfiguration(todo);
                WorkingAgent worker = await this.agentSelectors[todo].GetCompatibleAgentFromList(config, excluded);

                this.HandleFoundWorkingAgent(todo, worker);
            }
        }

        private void HandleFoundWorkingAgent(Job job, WorkingAgent worker)
        {
            if (worker == null)
            {
                if (this.OnNoResourceCompatibleAgentFound != null)
                {
                    this.OnNoResourceCompatibleAgentFound(job);
                }
            }
            else
            {
                worker.OnResultReceived += Worker_OnResultReceived;
                worker.OnMessageReceived += Worker_OnMessageReceived;
                worker.OnAgentGotUnreachable += Worker_OnAgentGotUnreachable;

                this.workingAgents.Add(worker);
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
            //if (this.workingAgents.Contains(agent))
            //{
                this.HandleImageResult(result.ConcernedRenderJobID, result.Picture);
            //}
            //PersistenceManager.SaveBytes(result.Picture, "test.jpg");
        }

        private async void HandleImageResult(Guid renderID, byte[] bytes)
        {
            BitmapImage image = null;

            await this.displayDispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                image = new BitmapImage();

                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    await image.SetSourceAsync(ms.AsRandomAccessStream());
                }

                if (this.workingAgents.Exists(x => x.Configuration.RenderJobID.Equals(renderID)))
                {
                    if (this.OnJobResultDisplayRequested != null)
                    {
                        this.OnJobResultDisplayRequested(image);
                    }
                }
            });

            int f = 0;

            /*if (this.OnImageDisplayRequested != null)
            {
                await this.displayDispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    BitmapImage image = new BitmapImage();
                    IRandomAccessStream iras = null;

                    using (MemoryStream ms = new MemoryStream(bytes))
                    {
                        iras = ms.AsRandomAccessStream();
                    }

                    await image.SetSourceAsync(iras);

                    this.OnImageDisplayRequested(image);
                });
            }*/
        } 

        private RenderConfiguration GetNewRenderConfiguration(Job jobToDo)
        {
            RenderConfiguration config = new RenderConfiguration();
            config.RenderJobID = Guid.NewGuid();
            config.JobID = this.jobID;
            config.JobToDo = jobToDo;
            config.RenderWidth = (int)this.renderSize.Width;
            config.RenderHeight = (int)this.renderSize.Height;

            config.UpdateInterval = 30;
            config.IgnoreEqualImages = true;

            return config;
        }
    }
}

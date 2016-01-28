using DisplayClient.Log;
using DisplayClient.Storage;
using Remote_Content_Show_Container;
//using Remote_Content_Show_Container.Resouces;
using Remote_Content_Show_Protocol;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
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
        private Job_Configuration configuration;

        public ContentDisplayManager(JobWindowList jobForWindow, List<Agent> availableAgents, Job_Configuration configuration)
        {
            this.jobForWindow = jobForWindow;
            this.configuration = configuration;

            this.workingAgents = new List<WorkingAgent>();
            this.agentSelectors = new Dictionary<Job, AgentSelector>();

            foreach (Job job in jobForWindow.Jobs)
            {
                AgentSelector selector = new AgentSelector(job, availableAgents);
                selector.OnAgentNotReachable += Selector_OnAgentNotReachable;

                this.agentSelectors[job] = selector;
            }

            this.Running = false;
        }

        public bool Running
        {
            get;
            private set;
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
            this.Running = true;

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

                            await Task.Factory.StartNew(() =>
                            {
                                this.RunJob(job);
                            });

                            Debug.WriteLine("--> start delaying...");

                            await Task.Delay(job.Duration * 1000);

                            Debug.WriteLine("--> start canceling...");

                            this.CancelJob(job);
                        }
                    }
                    while (jobForWindow.Looping && this.Running);
                });
            }
        }

        public void Cancel()
        {
            this.Running = false;
            this.CancelJob(this.currentlyTreatedJob);

            this.currentlyTreatedJob = null;
        }

        private async void CancelJob(Job job)
        {
            List<WorkingAgent> jobTreatingAgents = this.workingAgents.Where(x => x.Configuration.JobToDo.OrderingNumber == job.OrderingNumber).ToList();

            foreach (WorkingAgent agent in jobTreatingAgents)
            {
                agent.CancelRenderJob();
            }
            
            this.workingAgents.RemoveAll(x => jobTreatingAgents.Contains(x));

            if (this.OnDisplayAbortRequested != null)
            {
                await this.displayDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    this.OnDisplayAbortRequested();
                });
            }
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
                    this.HandleFileResource(job, new FileResource() {
                        Local = fr.Local,
                        Path = Path.Combine(
                            PersistenceManager.GetWriteablePath(), 
                            PersistenceManager.SavedCustomFilesDirectoryName, 
                            fr.Path) });
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
            bool accessable = false;

            try
            {
                // Try to access file at the given path
                StorageFile file = await StorageFile.GetFileFromPathAsync(resource.Path);

                accessable = true;
            }
            catch (Exception ex)
            {
                // It is a file, but it could not be found or accessed
                if (ex is FileNotFoundException)
                {
                    if (this.OnResourceNotAvailable != null)
                    {
                        this.OnResourceNotAvailable(job);
                    }
                }
                else if (ex is UnauthorizedAccessException)
                {
                    EventsManager.Log(Job_EventType.Error, this.configuration, "Could not access " + resource.Path);
                }
                else
                {
                    // non html web resources are also file resources
                    if (await CompatibilityManager.IsAvailableWebResource(resource.Path))
                    {
                        accessable = true;
                    }
                }
            }

            if (accessable)
            {
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
            if (await CompatibilityManager.IsAvailableWebResource(resource.Path))
            {
                if (this.OnWebsiteDisplayRequested != null)
                {
                    await this.displayDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        this.OnWebsiteDisplayRequested(new Uri(resource.Path));
                    });
                }
            }
            else
            {
                // give it to the agent?
            }
        }        
        
        private async void HandleProcessResource(Job job, ProcessResource resource)
        {
            List<Agent> foundAgents = this.agentSelectors[job].AvailableAgents.Where(x => x.IP.Equals(resource.ProcessAgent.IP)).ToList();

            if (foundAgents.Count >= 1)
            {
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
                Debug.WriteLine("--> Found agent for job " + job.Resource.Name);

                if (this.currentlyTreatedJob != null && worker.Configuration.JobToDo.OrderingNumber == this.currentlyTreatedJob.OrderingNumber)
                {
                    worker.OnResultReceived += Worker_OnResultReceived;
                    worker.OnMessageReceived += Worker_OnMessageReceived;
                    worker.OnAgentGotUnreachable += Worker_OnAgentGotUnreachable;

                    this.workingAgents.Add(worker);
                }
                else
                {
                    worker.CancelRenderJob();
                }
            }
        }

        private async void Worker_OnMessageReceived(WorkingAgent agent, RCS_Render_Job_Message message)
        {
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
                this.CancelJob(todo);

                List<Agent> excluded = this.agentSelectors[todo].AvailableAgents.Where(x => !x.IP.Equals(agent.Agent.IP)).ToList();

                RenderConfiguration config = this.GetNewRenderConfiguration(todo);
                WorkingAgent worker = await this.agentSelectors[todo].GetCompatibleAgentFromList(config, excluded);

                this.HandleFoundWorkingAgent(todo, worker);
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
            if (this.workingAgents.Exists(x => x.Configuration.RenderJobID.Equals(result.ConcernedRenderJobID)))
            {
                this.HandleImageResult(result.ConcernedRenderJobID, result.Picture);
            }
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

                if (this.OnJobResultDisplayRequested != null)
                {
                    if (this.workingAgents.Exists(x => x.Configuration.RenderJobID.Equals(renderID)))
                    {
                        this.OnJobResultDisplayRequested(image);
                    }
                }
            });
        } 

        private RenderConfiguration GetNewRenderConfiguration(Job jobToDo)
        {
            RenderConfiguration config = new RenderConfiguration();
            config.RenderJobID = Guid.NewGuid();
            config.JobID = this.configuration.JobID;
            config.JobToDo = jobToDo;
            config.RenderWidth = (int)this.renderSize.Width;
            config.RenderHeight = (int)this.renderSize.Height;

            config.UpdateInterval = 30;
            config.IgnoreEqualImages = true;

            return config;
        }
    }
}

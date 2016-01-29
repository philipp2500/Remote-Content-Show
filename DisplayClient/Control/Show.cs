using DisplayClient.Layout;
using DisplayClient.Log;
using Remote_Content_Show_Container;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace DisplayClient
{
    public class Show
    {
        //private List<ContentDisplayManager> displayManagers;

        private List<ContentDisplay> displays;

        public Show(Job_Configuration configuration, Size showSize)
        {
            this.Configuration = configuration;

            //this.displayManagers = new List<ContentDisplayManager>();
            this.displays = new List<ContentDisplay>();

            CustomWindow window = new CustomWindow(this.Configuration.Layout);

            window.Generate();
            window.ApplySize(showSize);

            this.ContentWindow = window;

            /*switch (configuration.Layout)
            {
                case WindowLayout.SingleWindow:
                    this.ContentWindow = new SingleWindow();
                    break;
                case WindowLayout.DoubleWindowHorizontalSplitted:
                    this.ContentWindow = new DoubleWindowHorizontalSplittedWindow();
                    break;
                case WindowLayout.DoubleWindowVertikalSplitted:
                    this.ContentWindow = new PiPWindow(); // new DoubleWindowVerticalSplittedWindow();
                    break;
                default:
                    this.ContentWindow = new SingleWindow();
                    break;
            }*/
        }

        public Job_Configuration Configuration
        {
            get;
            private set;
        }

        public IContentWindow ContentWindow
        {
            get;
            private set;
        }

        public void UpdateSize(Size size)
        {
            if (this.ContentWindow is CustomWindow)
            {
                ((CustomWindow)this.ContentWindow).ApplySize(size);
            }
        }

        public void UpdateConfigImage()
        {
            foreach (ContentDisplay display in this.displays)
            {
                display.UpdateConfigImage();
            }
        }

        public void Start()
        {
            this.displays.Clear();
            List<Agent> copy = this.Configuration.Agents.ToList();

            foreach (int windowLayoutID in this.Configuration.JobLists.Keys)
            {
                JobWindowList windowJob = this.Configuration.JobLists[windowLayoutID];
                ContentDisplayManager manager = new ContentDisplayManager(windowJob, copy, this.Configuration);

                if (copy.Count > 0)
                {
                    Agent temp = copy[0];
                    copy.RemoveAt(0);
                    copy.Add(temp);
                }

                ContentDisplay display = new ContentDisplay();

                manager.DisplayDispatcher = display.Dispatcher;

                manager.OnAgentNotReachable += Manager_OnAgentNotReachable;
                manager.OnResourceNotAvailable += Manager_OnResourceNotAvailable;
                manager.OnAgentWithProcessNotFound += Manager_OnAgentWithProcessNotFound;
                manager.OnNoResourceCompatibleAgentFound += Manager_OnNoResourceCompatibleAgentFound;
                manager.OnAgentAborted += Manager_OnAgentAborted;

                display.DisplayManager = manager;

                // Manager starts after this event has been fired.
                display.Loaded += Display_Loaded;                
                
                this.ContentWindow.GetAllDisplays()[windowLayoutID - 1].Children.Add(display);
                
                this.displays.Add(display);
            }

            /*foreach (ContentDisplayManager manager in this.displayManagers)
            {
                manager.Start();
            }*/
        }

        private void Display_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ContentDisplay display = (ContentDisplay)sender;
            
            if (!display.DisplayManager.Running)
            {
                display.DisplayManager.SetRenderSize(new Size(display.ActualWidth, display.ActualHeight));
                display.DisplayManager.Start();
            }
        }

        public void Cancel()
        {
            foreach (ContentDisplay display in this.displays)
            {
                display.DisplayManager.Cancel();
            }
        }

        private void Manager_OnAgentAborted(Job job, Agent agent)
        {
            string errorText = string.Empty;

            errorText += "Error while handling job " + job.OrderingNumber + " (" + job.Resource.Name + "): ";
            errorText += "The agent aborted the execution of the given resource.";

            EventsManager.Log(Job_EventType.Error, this.Configuration, errorText);
        }

        private void Manager_OnResourceNotAvailable(Job job)
        {
            //throw new NotImplementedException();
            string errorText = string.Empty;

            errorText += "Error while handling job " + job.OrderingNumber + " (" + job.Resource.Name + "): ";
            errorText += "The resource is not available.";

            EventsManager.Log(Job_EventType.Error, this.Configuration, errorText);
        }

        private void Manager_OnNoResourceCompatibleAgentFound(Job job)
        {
            //throw new NotImplementedException();
            string errorText = string.Empty;

            errorText += "Error while handling job " + job.OrderingNumber + " (" + job.Resource.Name + "): ";
            errorText += "No agent is able to handle the given resource.";

            EventsManager.Log(Job_EventType.Error, this.Configuration, errorText);
        }

        private void Manager_OnAgentWithProcessNotFound(Job job)
        {
            //throw new NotImplementedException();
            string errorText = string.Empty;

            errorText += "Error while handling job " + job.OrderingNumber + " (" + job.Resource.Name + "): ";
            errorText += "There is no agent running the process " + ((ProcessResource)job.Resource).ProcessID + ".";

            EventsManager.Log(Job_EventType.Error, this.Configuration, errorText);
        }

        private void Manager_OnAgentNotReachable(Job job, Agent agent)
        {
            //throw new NotImplementedException();
            string errorText = string.Empty;

            errorText += "Error while handling job " + job.OrderingNumber + " (" + job.Resource.Name + "): ";
            errorText += "The agent " + agent.IP.ToString() + " is offline.";

            EventsManager.Log(Job_EventType.Error, this.Configuration, errorText);
        }
    }
}

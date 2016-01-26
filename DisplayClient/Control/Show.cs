using DisplayClient.Layout;
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
        private List<ContentDisplayManager> displayManagers;

        public Show(Job_Configuration configuration)
        {
            this.Configuration = configuration;

            this.displayManagers = new List<ContentDisplayManager>();

            switch (configuration.Layout)
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
            }
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

        public void Start()
        {
            this.displayManagers.Clear();

            foreach (int windowLayoutID in this.Configuration.JobLists.Keys)
            {
                JobWindowList windowJob = this.Configuration.JobLists[windowLayoutID];
                ContentDisplayManager manager = new ContentDisplayManager(windowJob, this.Configuration.Agents, this.Configuration.JobID);

                ContentDisplay display = new ContentDisplay();

                manager.DisplayDispatcher = display.Dispatcher;

                manager.OnAgentNotReachable += Manager_OnAgentNotReachable;
                manager.OnResourceNotAvailable += Manager_OnResourceNotAvailable;
                manager.OnAgentWithProcessNotFound += Manager_OnAgentWithProcessNotFound;
                manager.OnNoResourceCompatibleAgentFound += Manager_OnNoResourceCompatibleAgentFound;

                display.DisplayManager = manager;

                // Manager starts after this event has been fired.
                display.Loaded += Display_Loaded;                
                
                this.ContentWindow.GetAllDisplays()[windowLayoutID - 1].Children.Add(display);
                
                this.displayManagers.Add(manager);
            }

            /*foreach (ContentDisplayManager manager in this.displayManagers)
            {
                manager.Start();
            }*/
        }

        private void Display_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ContentDisplay display = (ContentDisplay)sender;

            display.DisplayManager.SetRenderSize(new Size(display.ActualWidth, display.ActualHeight));
            display.DisplayManager.Start();
        }

        public void Cancel()
        {
            foreach (ContentDisplayManager manager in this.displayManagers)
            {
                manager.Cancel();
            }
        }

        private void Manager_OnResourceNotAvailable(IResource resource)
        {
            throw new NotImplementedException();
        }

        private void Manager_OnNoResourceCompatibleAgentFound(Job job)
        {
            throw new NotImplementedException();
        }

        private void Manager_OnAgentWithProcessNotFound(Job job)
        {
            throw new NotImplementedException();
        }

        private void Manager_OnAgentNotReachable(Job job, Agent agent)
        {
            //throw new NotImplementedException();
        }
    }
}

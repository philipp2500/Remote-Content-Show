using Remote_Content_Show_Container;
using Remote_Content_Show_Container.Resouces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace DisplayClient
{
    public class ContentDisplayManager
    {
        private JobWindowList jobForWindow;

        private List<WorkingAgent> workingAgents;

        public ContentDisplayManager(JobWindowList jobForWindow)
        {
            this.jobForWindow = jobForWindow;
            this.workingAgents = new List<WorkingAgent>();

            this.jobForWindow = new JobWindowList();
            this.jobForWindow.Jobs.Add(new Job() { Duration = 2, OrderingNumber = 1, Resource = new FileResource() { Path = "http://www.google.at" } });
            this.jobForWindow.Jobs.Add(new Job() { Duration = 2, OrderingNumber = 1, Resource = new FileResource() { Path = "http://img.pr0gramm.com/2016/01/20/341c9285b24bd3f8.jpg" } });
        }

        public delegate void ImageDisplayRequested(BitmapImage image);

        public delegate void WebsiteDisplayRequested(Uri uri);

        public event ImageDisplayRequested OnImageDisplayRequested;

        public event WebsiteDisplayRequested OnWebsiteDisplayRequested;

        public void Start()
        {            
            if (jobForWindow != null)
            {
                Task.Factory.StartNew(() =>
                {
                    List<Job> ordered = jobForWindow.Jobs.OrderBy(x => x.OrderingNumber).ToList();

                    foreach (Job job in ordered)
                    {
                        this.RunJob(job);
                    }
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

                if (fr.Path.EndsWith(".jpg"))
                {
                    if (this.OnImageDisplayRequested != null)
                    {
                        BitmapImage img = new BitmapImage(new Uri(fr.Path));

                        this.OnImageDisplayRequested(img);
                    }
                    else if (this.OnWebsiteDisplayRequested != null)
                    {
                        this.OnWebsiteDisplayRequested(new Uri(fr.Path));
                    }
                }
            }

            Task.Delay(job.Duration / 1000);
        }
    }
}

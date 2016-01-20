using Remote_Content_Show_Container;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace DisplayClient
{
    public class DisplayManager
    {
        private JobWindowList jobForWindow;

        public DisplayManager(JobWindowList jobForWindow)
        {
            this.jobForWindow = jobForWindow;
        }

        public delegate void ImageDisplayRequested(BitmapImage image);

        public event ImageDisplayRequested OnImageDisplayRequested;

        public void Start()
        {
            if (this.OnImageDisplayRequested != null)
            {
                this.OnImageDisplayRequested(new BitmapImage(new Uri(@"C:\Temp\jellyfish1.jpg")));
            }
        }
    }
}

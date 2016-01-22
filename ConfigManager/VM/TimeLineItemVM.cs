using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Remote_Content_Show_Container;
using System.Windows.Media.Imaging;
using ImageHandler;

namespace ConfigManager
{
    public class TimeLineItemVM
    {
        private IResource resource;
        private int duration;

        public TimeLineItemVM(int duration, IResource resource)
        {
            this.duration = duration;
            this.resource = resource;
        }

        public string Duration
        {
            get
            {
                string min = (this.duration / 60).ToString();
                string sek = (this.duration % 60).ToString();
                return string.Format("{0}:{1}", min, sek);
            }
        }

        public string Name
        {
            get
            {
                return this.resource.Name;
            }
        }

        public BitmapImage ShowImag
        {
            get
            {
                if (this.resource is WebResource)
                {
                    return ImageHandler.ImageHandler.BitmapToBitmapImage(ConfigManager.Properties.Resources.smallloadFromweb);
                }
                else if (this.resource is ProcessResource)
                {
                    return ImageHandler.ImageHandler.BitmapToBitmapImage(ConfigManager.Properties.Resources.smallloadFromProcess);
                }
                else
                {
                    return ImageHandler.ImageHandler.BitmapToBitmapImage(ConfigManager.Properties.Resources.snallloadFromFile);
                }
            }
        }
    }
}

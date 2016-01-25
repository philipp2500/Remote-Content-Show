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
        public TimeLineItemVM(int duration, IResource resource)
        {
            this.Duration = duration;
            this.Resource = resource;
        }

        public string DurationS
        {
            get
            {
                string min = (this.Duration / 60).ToString();
                string sek = (this.Duration % 60).ToString();
                return string.Format("{0}:{1}", min, sek);
            }
        }

        public int Duration
        {
            get;
            private set;
        }

        public IResource Resource
        {
            get;
            private set;
        }

        public string Name
        {
            get
            {
                return this.Resource.Name;
            }
        }

        public BitmapImage ShowImag
        {
            get
            {
                if (this.Resource is WebResource)
                {
                    return ImageHandler.ImageHandler.BitmapToBitmapImage(ConfigManager.Properties.Resources.smallloadFromweb);
                }
                else if (this.Resource is ProcessResource)
                {
                    return ImageHandler.ImageHandler.BitmapToBitmapImage(ConfigManager.Properties.Resources.smallloadFromProcess);
                }
                else
                {
                    FileResource fr = this.Resource as FileResource;
                    if (!fr.Loacal)
                    {
                        return ImageHandler.ImageHandler.BitmapToBitmapImage(ConfigManager.Properties.Resources.snallloadFromFile);
                    }
                    else
                    {
                        return ImageHandler.ImageHandler.BitmapToBitmapImage(ConfigManager.Properties.Resources.smalllocalFiles);
                    }                    
                }
            }
        }
    }
}

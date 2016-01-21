using Remote_Content_Show_Container;
using System.IO;
using System.Windows.Media.Imaging;

namespace Agent
{
    public class ProcessVM
    {
        private ProcessDescription process = null;
        private BitmapImage windowPicture = null;

        public ProcessVM(ProcessDescription process)
        {
            this.process = process;
        }

        public int ProcessId
        {
            get
            {
                return this.process.ProcessId;
            }
        }

        public string ProcessName
        {
            get
            {
                return this.process.ProcessName;
            }
        }

        public string ProcessTitle
        {
            get
            {
                return this.process.ProcessTitle;
            }
        }

        public BitmapImage WindowPicture
        {
            get
            {
                if (this.windowPicture == null)
                {
                    this.windowPicture = 
                        ImageHandler.ImageHandler.BytesToImage(this.process.WindowPicture);
                }

                return this.windowPicture;
            }
        }
    }
}

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
                    using (var memory = new MemoryStream(this.process.WindowPicture))
                    {
                        memory.Position = 0;
                        this.windowPicture = new BitmapImage();
                        this.windowPicture.BeginInit();
                        this.windowPicture.StreamSource = memory;
                        this.windowPicture.CacheOption = BitmapCacheOption.OnLoad;
                        this.windowPicture.EndInit();
                    }
                }

                return this.windowPicture;
            }
        }
    }
}

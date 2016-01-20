using Remote_Content_Show_Container;
using System.IO;
using System.Windows.Media.Imaging;

namespace Agent
{
    public class ProcessVM
    {
        private ProcessDescription process = null;

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
                //TODO
                using (var memory = new MemoryStream(this.process.WindowPicture))
                {
                    memory.Position = 0;
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = memory;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();

                    return bitmapImage;
                }
            }
        }
    }
}

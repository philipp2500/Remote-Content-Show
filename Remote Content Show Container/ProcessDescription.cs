using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Remote_Content_Show_Container
{
    public class ProcessDescription
    {

        public ProcessDescription(byte[] windowPicture, int processId, string processName)
        {
            this.WindowPicture = windowPicture;
            this.ProcessId = processId;
            this.ProcessName = processName;
        }
            
        public string ProcessName
        {
            get;
            private set;
        }

        public byte[] WindowPicture
        {
            get;
            private set;
        }

        public int ProcessId
        {
            get;
            private set;
        }
    }
}

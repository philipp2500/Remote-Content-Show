using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Remote_Content_Show_Container;

namespace Remote_Content_Show_Protocol
{
    public class RCS_Job_Cancel : Remote_Content_Show_Message
    {
        public RCS_Job_Cancel(CancelJobReason reason, Guid jobID)
        {
            this.Reason = reason;
            this.JobID = jobID;
        }

        public Guid JobID
        {
            get;
            private set;
        }

        public CancelJobReason Reason
        {
            get;
            private set;
        }
    }
}

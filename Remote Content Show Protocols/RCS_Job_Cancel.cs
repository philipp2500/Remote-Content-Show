using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Remote_Content_Show_Container.Enums;

namespace Remote_Content_Show_Protocol
{
    public class RCS_Job_Cancel : Remote_Content_Show_Message
    {
        public RCS_Job_Cancel(CancelJobReason reason)
        {
            this.Reason = reason;
        }

        public Guid JobID
        {
            get;
            set;
        }

        public CancelJobReason Reason
        {
            get;
            private set;
        }
    }
}

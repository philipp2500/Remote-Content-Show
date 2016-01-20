using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

using Remote_Content_Show_Container.Enums;

namespace Remote_Content_Show_Protocol
{
    public class RCS_Render_Job_Cancel : Remote_Content_Show_Message
    {
        public CancelRenderJobReason Reason
        {
            get;
            set;
        }

        public Guid ConceredRenderJobID
        {
            get;
            set;
        }
    }
}

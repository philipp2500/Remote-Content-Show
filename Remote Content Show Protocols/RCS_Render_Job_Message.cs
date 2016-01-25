using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Remote_Content_Show_Container;

namespace Remote_Content_Show_Protocol
{
    public class RCS_Render_Job_Message : Remote_Content_Show_Message
    {
        public RCS_Render_Job_Message(RenderMessage message, Guid concernedRenderJobID, RemoteType remote) : base(remote)
        {
            this.Message = message;
            this.ConcernedRenderJobID = concernedRenderJobID;
        }

        public RenderMessage Message
        {
            get;
            private set;
        }

        public Guid ConcernedRenderJobID
        {
            get;
            private set;
        }
    }
}

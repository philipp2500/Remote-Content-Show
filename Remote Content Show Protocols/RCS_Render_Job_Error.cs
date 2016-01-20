using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Remote_Content_Show_Container.Enums;

namespace Remote_Content_Show_Protocol
{
    public class RCS_Render_Job_Error : Remote_Content_Show_Message
    {
        public RCS_Render_Job_Error(RenderError errorDescription, Guid concernedRenderJobID)
        {
            this.ErrorDescription = errorDescription;
            this.ConcernedRenderJobID = concernedRenderJobID;
        }

        public RenderError ErrorDescription
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

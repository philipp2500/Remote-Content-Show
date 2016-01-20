using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Remote_Content_Show_Protocol
{
    public class RCS_Render_Job_Result : Remote_Content_Show_Message
    {
        public RCS_Render_Job_Result(Guid concernedRenderJobID, byte[] picture)
        {
            this.ConcernedRenderJobID = concernedRenderJobID;
            this.Picture = picture;
        }

        public Guid ConcernedRenderJobID
        {
            get;
            private set;
        }

        public byte[] Picture
        {
            get;
            private set;
        }
    }
}

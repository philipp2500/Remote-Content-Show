using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

using Remote_Content_Show_Container;

namespace Remote_Content_Show_Protocol
{
    public class RCS_Render_Job : Remote_Content_Show_Message
    {
        public RCS_Render_Job(RenderConfiguration configuration, RemoteType remote) : base(remote)
        {
            this.Configuration = configuration;
        }

        public RenderConfiguration Configuration
        {
            get;
            private set;
        }
    }
}

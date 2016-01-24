using Remote_Content_Show_Container;
using System.Diagnostics;

namespace Agent
{
    public class CaptureThreadArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CaptureThreadArgs"/> class.
        /// </summary>
        public CaptureThreadArgs(Process process, RenderConfiguration configuration)
        {
            this.Process = process;
			this.Configuration = configuration;
            this.Exit = false;
        }

        public Process Process { get; private set; }

		public RenderConfiguration Configuration { get; private set; }
        
        public bool Exit { get; set; }
    }
}

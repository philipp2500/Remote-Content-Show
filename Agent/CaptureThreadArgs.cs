using System;
using System.Diagnostics;

namespace Agent
{
    public class CaptureThreadArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CaptureThreadArgs"/> class.
        /// </summary>
        /// <param name="process">The process whose main window to capture.</param>
        /// <param name="captureDelay">The number of milliseconds between two screen captures.</param>
        /// <param name="duration">The GUID of the associated RenderJob.</param>
        public CaptureThreadArgs(Process process, int captureDelay, int duration, Guid renderJobId)
        {
            this.Process = process;
            this.CaptureDelay = captureDelay;
            this.Duration = duration;
            this.RenderJobId = renderJobId;
            this.Exit = false;
        }

        public bool Exit { get; set; }

        public Process Process { get; private set; }

        public int CaptureDelay { get; private set; }

        public int Duration { get; private set; }

        public Guid RenderJobId { get; private set; }
    }
}

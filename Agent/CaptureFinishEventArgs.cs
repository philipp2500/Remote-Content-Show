using System;

namespace Agent
{
    public class CaptureFinishEventArgs : EventArgs
    {
        public CaptureFinishEventArgs(Guid renderJobId)
        {
            this.RenderJobId = renderJobId;
        }

        public Guid RenderJobId { get; private set; }
    }
}
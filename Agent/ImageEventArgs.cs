using System;
using System.Drawing;

namespace Agent
{
    public class ImageEventArgs : EventArgs
    {
        public ImageEventArgs(Guid concernedJobId, Bitmap image)
        {
            this.ConcernedRenderJobID = concernedJobId;
            this.Image = image;
        }

        public Guid ConcernedRenderJobID { get; private set; }

        public Bitmap Image { get; private set; }
    }
}
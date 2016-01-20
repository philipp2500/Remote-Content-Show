using System;
using System.Drawing;

namespace Agent
{
    public class ImageEventArgs : EventArgs
    {
        public ImageEventArgs(Bitmap image)
        {
            this.Image = image;
        }

        public Bitmap Image { get; private set; }
    }
}
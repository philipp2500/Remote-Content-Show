using System.Diagnostics;

namespace Agent
{
    public class CaptureThreadArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CaptureThreadArgs"/> class.
        /// </summary>
        /// <param name="process">The process whose main window to capture.</param>
        /// <param name="fps">The number of frames to capture per second.</param>
        public CaptureThreadArgs(Process process, int fps)
        {
            this.Process = process;
            this.FPS = fps;
        }

        public Process Process { get; private set; }

        public int FPS { get; private set; }

        public bool Exit { get; set; }
    }
}

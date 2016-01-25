using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Threading;
using System.Diagnostics;
using Remote_Content_Show_Container;

namespace Agent
{
    public class ScreenCapture
    {
        /// <summary>
        /// The event fired when a image was captured.
        /// </summary>
        public event EventHandler<ImageEventArgs> OnImageCaptured;

        /// <summary>
        /// The event fired when the capturing of images is finished.
        /// </summary>
        public event EventHandler<CaptureFinishEventArgs> OnCaptureFinished;

        /// <summary>
        /// The event fired when the process whose window to capture has exited.
        /// </summary>
        public event EventHandler<CaptureFinishEventArgs> OnProcessExited;

        public bool IsRunning { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        private const int PROCESS_STARTUP_WAIT_DURATION = 15000;
        /// <summary>
        /// Indicates that a minimized window should be restored.
        /// </summary>
        private const int SW_RESTORE = 9;
        private const int PW_CLIENTONLY = 1; // 1 => only window content; 0 => window incl. border
        private CaptureThreadArgs captureArgs = null;
		
        /// <summary>
        /// Continuously captures images of the given resource and fires the <see cref="ScreenCapture.OnImageCaptured"/> event.
        /// <see cref="FileResource"/>s are opened with their default program.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the process has no UI.</exception>
        /// <exception cref="ProcessStartupException">Thrown if the process took to long to start up (<seealso cref="ScreenCapture.PROCESS_STARTUP_WAIT_DURATION"/>).</exception>
        /// <exception cref="ArgumentException">
        /// Thrown if config's update interval is less than 0
        /// or the process for a <see cref="FileResource"/> cannot be started
        /// or a <see cref="ProcessResource"/>s PID cannot be found.</exception>
        /// <exception cref="NotSupportedException">Thrown if the job's resource is not of type <see cref="FileResource"/> or <see cref="ProcessResource"/>.</exception>
        public void StartCapture(RenderConfiguration config)
        {
			Process proc = null;
            IResource resource = config.JobToDo.Resource;

            if (config.UpdateInterval < 0)
            {
                throw new ArgumentException("The update interval must not be less than 0.");
            }

            if (resource is FileResource)
			{
                string path = ((FileResource)resource).Path;

                try
                {
                    proc = Process.Start(path);
                }
                catch
                {
                    throw new ArgumentException("Failed to start the process for the FileResource.");
                }
                
                if (!proc.WaitForInputIdle(PROCESS_STARTUP_WAIT_DURATION))
                {
                    throw new ProcessStartupException();
                }
            }
			else if (resource is ProcessResource)
			{
				int pid = ((ProcessResource)resource).ProcessID;
				proc = Process.GetProcessById(pid);
			}
			else
			{
                throw new NotSupportedException("Only capturing of jobs with FileResource and ProcessResource is supported.");
			}
			
            if (proc.MainWindowHandle == IntPtr.Zero)
            {
                throw new InvalidOperationException("Cannot capture images of a window-less process.");
            }

            Thread capturer = new Thread(new ParameterizedThreadStart(this.Capture));
            this.captureArgs = new CaptureThreadArgs(proc, config);
            capturer.IsBackground = true;
            capturer.Start(this.captureArgs);
        }

        /// <summary>
        /// Stops the previously started capturing of a process' window.
        /// </summary>
        public void StopCapture()
        {
            if (this.captureArgs == null)
            {
                return;
            }

            this.captureArgs.Exit = true;
        }

        /// <summary>
        /// Captures an image of the given window.
        /// </summary>
        /// <param name="handle">The window handle of the window to capture.</param>
        /// <returns>A bitmap image of the captured window.</returns>
        public Bitmap CaptureWindow(IntPtr handle)
        {
            var rect = new Rect();
            GetClientRect(handle, ref rect);
            int width = rect.Right - rect.Left;
            int height = rect.Bottom - rect.Top;

            if (width <= 0 || height <= 0)
            {
                // if window has been closed
                return null;
            }

            Bitmap bmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            Graphics gfxBmp = Graphics.FromImage(bmp);
            IntPtr hdcBitmap = gfxBmp.GetHdc();

            PrintWindow(handle, hdcBitmap, PW_CLIENTONLY);
            gfxBmp.ReleaseHdc(hdcBitmap);
            gfxBmp.Dispose();

            return bmp;
        }
        
        private void Capture(object data)
        {
            CaptureThreadArgs args = data as CaptureThreadArgs;

            if (args == null)
            {
                throw new ArgumentException("Parameter must be of type CaptureThreadArgs and must not be null.");
            }
			
			RenderConfiguration config = args.Configuration;
						
			Size imageSize = new Size(config.RenderWidth, config.RenderHeight);
			Process proc = args.Process;
            IntPtr windowHandle = proc.MainWindowHandle;
            DateTime endTime = DateTime.Now.AddSeconds(config.JobToDo.Duration); //TODO sicherstellen, dass duration in sekunden!!!!!!!!!!!!
            Bitmap prevImage = new Bitmap(1, 1);

            if (windowHandle == IntPtr.Zero)
            {
                throw new InvalidOperationException("Cannot capture images of a window-less process.");
            }
            
            while (DateTime.Now <= endTime && !proc.HasExited && !args.Exit)
            {
                if (IsIconic(windowHandle))
                {
                    ShowWindow(windowHandle, SW_RESTORE);
                }

                Thread.Sleep((int)config.UpdateInterval); //TODO warum ist update interval Double???????????
                
                var image = this.CaptureWindow(windowHandle);

                if (image == null)
                {
                    continue;
                }
                
				if (this.OnImageCaptured != null &&
				    (!config.IgnoreEqualImages || !ImageHandler.ImageHandler.AreEqual(image, prevImage)))
                {
					image = (Bitmap)ImageHandler.ImageHandler.Resize(image, imageSize);
                    this.OnImageCaptured(this, new ImageEventArgs(config.JobID, image));
                }

                prevImage.Dispose();
                prevImage = image;                
            }
            
            this.IsRunning = false;

            if (DateTime.Now > endTime && !proc.HasExited && this.OnCaptureFinished != null)
            {
                this.OnCaptureFinished(this, new CaptureFinishEventArgs(config.JobID));
            }

            if (proc.HasExited && this.OnProcessExited != null)
            {
                this.OnProcessExited(this, new CaptureFinishEventArgs(config.JobID));
            }
        }
        
        [StructLayout(LayoutKind.Sequential)]
        private struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetClientRect(IntPtr hWnd, ref Rect rect);

        [DllImport("user32.dll")]
        private static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, int nFlags);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern bool IsIconic(IntPtr hWnd);

    }
}

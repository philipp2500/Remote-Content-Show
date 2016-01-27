using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Threading;
using System.Diagnostics;
using Remote_Content_Show_Container;
using System.Text;

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

        #region Constants

        /// <summary>
        /// The Z order position of the "bottommost" window.
        /// </summary>
        private static readonly IntPtr HWND_BOTTOM = new IntPtr(1);

        /// <summary>
        /// The number of milliseconds between the update of the window handle of the captured process.
        /// (Some applications like PowerPoint show a startup window which is closed later on when the actual window is ready,
        /// and therefore the new window would never be captured.)
        /// </summary>
        private const int PROCESS_WINDOW_UPDATE_INTERVAL = 10000;
        /// <summary>
        /// The number of milliseconds to wait for a process to startup before an exception is thrown.
        /// </summary>
        private const int PROCESS_STARTUP_WAIT_DURATION = 30000;

        /// <summary>
        /// Indicates that the moved window should not be resized.
        /// </summary>
        private const uint SWP_NOSIZE = 0x0001;
        /// <summary>
        /// Indicates that the moved window should not be moved in X, Y direction.
        /// </summary>
        private const uint SWP_NOMOVE = 0x0002;
        /// <summary>
        /// Indicates that the moved window should not be activated.
        /// </summary>
        private const uint SWP_NOACTIVATE = 0x0010;
        /// <summary>
        /// Indicates that a minimized window should be restored.
        /// </summary>
        private const int SW_RESTORE = 9;
        /// <summary>
        /// Indicates that a window should be minimized.
        /// </summary>
        private const int SW_MINIMIZE = 6;
        private const int SW_SHOWNOACTIVATE = 4;
        private const int PW_CLIENTONLY = 1; // 1 => only window content; 0 => window incl. border

        #endregion Constants

        /// <summary>
        /// Indicates whether the captured window's process was started by this instance.
        /// </summary>
        private bool selfStartedProcess = false;
        /// <summary>
        /// The arguments used to control the capturing thread.
        /// </summary>
        private CaptureThreadArgs captureArgs = null;

        /// <summary>
        /// Gets a value indicating whether a capturing thread is running.
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Continuously captures images of the given resource and fires the <see cref="ScreenCapture.OnImageCaptured"/> event.
        /// <see cref="FileResource"/>s are opened with their default program.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if a already capturing a window or if the process has no UI.</exception>
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

            if (this.IsRunning)
            {
                throw new InvalidOperationException("A capturing process is already started.");
            }

            if (config.UpdateInterval < 0)
            {
                throw new ArgumentException("The update interval must not be less than 0.");
            }

            if (resource is FileResource)
			{
                string path = ((FileResource)resource).Path;
                string extension = System.IO.Path.GetExtension(path);

                // TODO start PowerPoint files as fullscreen slideshow
                if (extension == ".ppt" || extension == ".pptx")
                {
                    proc = new Process();
                    proc.StartInfo.FileName = "powerpnt.exe";
                    proc.StartInfo.Arguments = string.Format("/S \"{0}\"", path);
                }
                else
                {
                    proc = new Process();
                    proc.StartInfo.FileName = path;
                }

                try
                {
                    proc.Start();
                    this.selfStartedProcess = true;
                }
                catch
                {
                    throw new ArgumentException("Failed to start the process for the FileResource.");
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
            
            if (!proc.WaitForInputIdle(PROCESS_STARTUP_WAIT_DURATION))
            {
                throw new ProcessStartupException();
            }

            if (proc.MainWindowHandle == IntPtr.Zero)
            {
                throw new InvalidOperationException("Cannot capture images of a window-less process.");
            }

            // bring the window to the "bottommost" position
            SetWindowPos(proc.MainWindowHandle, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
            
            Thread capturer = new Thread(new ParameterizedThreadStart(this.Capture));
            this.captureArgs = new CaptureThreadArgs(proc, config);
            capturer.IsBackground = true;
            capturer.Start(this.captureArgs);
            this.IsRunning = true;
        }

        /// <summary>
        /// Stops the previously started capturing of a process' window and
        /// kills the captured process if it was started by this instance.
        /// Returns immediately if capturing was not started before.
        /// </summary>
        public void StopCapture()
        {
            if (!this.IsRunning)
            {
                return;
            }

            if (this.selfStartedProcess)
            {
                try
                {
                    this.captureArgs.Process.Kill();
                }
                catch
                {
                    // process has already exited
                }
            }

            this.captureArgs.Exit = true;
            this.IsRunning = false;
            this.selfStartedProcess = false;
        }

        /// <summary>
        /// Captures an image of the given window.
        /// If the window is minimized, it is restored, the image is captured and the window is minimized again.
        /// </summary>
        /// <param name="handle">The window handle of the window to capture.</param>
        /// <returns>A bitmap image of the captured window or null if the window has no UI.</returns>
        public Bitmap SnapshotWindow(IntPtr handle)
        {
            bool wasMinimized = false;

            if (IsIconic(handle))
            {
                wasMinimized = true;
                ShowWindow(handle, SW_SHOWNOACTIVATE);
            }

            Bitmap image = this.CaptureWindow(handle);

            if (wasMinimized)
            {
                ShowWindow(handle, SW_MINIMIZE);
            }

            return image;
        }

        /// <summary>
        /// Captures an image of the given window.
        /// </summary>
        /// <param name="handle">The window handle of the window to capture.</param>
        /// <returns>A bitmap image of the captured window or null if the window is not visible (e.g. minimized).</returns>
        private Bitmap CaptureWindow(IntPtr handle)
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
        
        /// <summary>
        /// Continously captures images as configured in the given <see cref="CaptureThreadArgs"/>.
        /// </summary>
        /// <param name="data">The arguments of type <see cref="CaptureThreadArgs"/>.</param>
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
            DateTime windowHandleUpdateTime = DateTime.Now;
            IntPtr windowHandle = proc.MainWindowHandle;
            DateTime endTime = DateTime.Now.AddSeconds(config.JobToDo.Duration);
            Bitmap prevImage = new Bitmap(1, 1);
            double actualImageRatio = 0;
            double configImageRatio = config.RenderWidth / (double)config.RenderHeight;

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

                // update the window handle if needed
                if (DateTime.Now > windowHandleUpdateTime)
                {
                    proc = Process.GetProcessById(proc.Id);
                    windowHandle = proc.MainWindowHandle;
                    windowHandleUpdateTime.AddMilliseconds(PROCESS_WINDOW_UPDATE_INTERVAL);
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
                    actualImageRatio = image.Width / (double)image.Height;
                    configImageRatio = config.RenderWidth / (double)config.RenderHeight;

                    if (actualImageRatio < configImageRatio)
                    {
                        imageSize = new Size((int)(config.RenderHeight * actualImageRatio), config.RenderHeight);
                    }
                    else
                    {
                        imageSize = new Size(config.RenderWidth, (int)(config.RenderWidth * (1.0 / actualImageRatio)));
                    }

                    image = (Bitmap)ImageHandler.ImageHandler.Resize(image, imageSize);
                    this.OnImageCaptured(this, new ImageEventArgs(config.RenderJobID, image));
                }

                prevImage.Dispose();
                prevImage = image;                
            }

            this.StopCapture();

            if (DateTime.Now > endTime && !proc.HasExited && this.OnCaptureFinished != null)
            {
                this.OnCaptureFinished(this, new CaptureFinishEventArgs(config.RenderJobID));
            }

            if (proc.HasExited && this.OnProcessExited != null)
            {
                this.OnProcessExited(this, new CaptureFinishEventArgs(config.RenderJobID));
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
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        
        [DllImport("user32.dll")]
        private static extern bool IsIconic(IntPtr hWnd);

    }
}
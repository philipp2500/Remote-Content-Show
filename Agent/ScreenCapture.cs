using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Threading;
using System.Diagnostics;

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

        private const int SW_RESTORE = 9; // Indicates that a minimized window should be restored
        private const int PW_CLIENTONLY = 1; // 1 => only window content; 0 => window incl. border
        private CaptureThreadArgs captureArgs = null;

        /// <summary>
        /// Continuously captures images of the given process' window and fires the <see cref="ScreenCapture.OnImageCaptured"/> event.
        /// </summary>
        /// <param name="proc">The process whose main window to capture.</param>
        /// <param name="captureDelay">The number of milliseconds between two screen captures.</param>
        /// <param name="duration">The duration in seconds indicating how long to capture.</param>
        /// <param name="renderJobId">The GUID of the associated RenderJob.</param>
        /// <exception cref="InvalidOperationException">Thrown if the process' main window handle is <see cref="IntPtr.Zero"/>.</exception>
        /// <exception cref="ArgumentException">Thrown if captureDelay is less than 0.</exception>
        public void StartCapture(Process proc, int captureDelay, int duration, Guid renderJobId)
        {
            if (proc.MainWindowHandle == IntPtr.Zero)
            {
                throw new InvalidOperationException("Cannot capture images of a window-less process.");
            }

            if (captureDelay < 0)
            {
                throw new ArgumentException("The FPS must not be less than 0.", "fps");
            }

            Thread capturer = new Thread(new ParameterizedThreadStart(this.Capture));
            this.captureArgs = new CaptureThreadArgs(proc, captureDelay, duration, renderJobId);
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

            Process proc = args.Process;
            Bitmap prevImage = new Bitmap(1, 1);
            IntPtr windowHandle = proc.MainWindowHandle;
            DateTime endTime = DateTime.Now.AddSeconds(args.Duration);

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

                Thread.Sleep(args.CaptureDelay);
                
                var image = CaptureWindow(windowHandle);

                if (image == null)
                {
                    continue;
                }
                
                //if (!ImageHandler.ImageHandler.AreEqual(image, prevImage)) //TODO equal images
                if (this.OnImageCaptured != null)
                {
                    this.OnImageCaptured(this, new ImageEventArgs(args.RenderJobId, image));
                }

                prevImage.Dispose();
                prevImage = image;                
            }
            
            this.IsRunning = false;

            if (DateTime.Now > endTime && !proc.HasExited && this.OnCaptureFinished != null)
            {
                this.OnCaptureFinished(this, new CaptureFinishEventArgs(args.RenderJobId));
            }

            if (proc.HasExited && this.OnProcessExited != null)
            {
                this.OnProcessExited(this, new CaptureFinishEventArgs(args.RenderJobId));
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

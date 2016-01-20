using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Threading;
using System.Diagnostics;

namespace Agent
{
    public class ScreenCapture
    {
        public event EventHandler<ImageEventArgs> OnImageCaptured;
        public event EventHandler OnProcessExited;

        private const int SW_RESTORE = 9;
        private const int PW_CLIENTONLY = 1; // 1 => only window content; 0 => window incl. border

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
        
        public void StartCapture(Process proc, int fps)
        {
            if (proc.MainWindowHandle == IntPtr.Zero)
            {
                throw new InvalidOperationException("Cannot capture images of a window-less process.");
            }

            if (fps < 0)
            {
                throw new ArgumentException("The FPS must not be less than 0.", "fps");
            }

            Thread capturer = new Thread(new ParameterizedThreadStart(this.Capture));
            capturer.IsBackground = true;
            capturer.Start(new CaptureThreadArgs(proc, fps));
        }

        private void Capture(object data)
        {
            CaptureThreadArgs args = data as CaptureThreadArgs;

            if (args == null)
            {
                throw new ArgumentException("Parameter must be of type CaptureThreadArgs and must not be null.");
            }

            Process proc = args.Process;
            int captureDelay = 1000 / args.FPS;
            Bitmap prevImage = new Bitmap(1, 1);
            IntPtr windowHandle = proc.MainWindowHandle;

            if (windowHandle == IntPtr.Zero)
            {
                throw new InvalidOperationException("Cannot capture images of a window-less process.");
            }
            
            while (!args.Exit && !proc.HasExited)
            {
                if (IsIconic(windowHandle))
                {
                    ShowWindow(windowHandle, SW_RESTORE);
                }

                Thread.Sleep(captureDelay);
                
                var image = CaptureWindow(windowHandle);

                if (image == null)
                {
                    continue;
                }
                
                //if (!ImageHandler.ImageHandler.AreEqual(image, prevImage))
                if (this.OnImageCaptured != null)
                {
                    this.OnImageCaptured(this, new ImageEventArgs(image));
                }

                prevImage.Dispose();
                prevImage = image;                
            }

            if (proc.HasExited && this.OnProcessExited != null)
            {
                this.OnProcessExited(this, EventArgs.Empty);
            }
        }

        public Bitmap CaptureWindow(IntPtr handle)
        {
            var rect = new Rect();
            GetClientRect(handle, ref rect);
            int width = rect.Right - rect.Left;
            int height = rect.Bottom - rect.Top;

            if (width == 0 && height == 0)
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

    }
}

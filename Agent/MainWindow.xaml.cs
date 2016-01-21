using Agent.Network;
using Remote_Content_Show_Container;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows;

namespace Agent
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Server server = null;
        private System.Drawing.Size pictureSize = new System.Drawing.Size(150, 150);
        private ObservableCollection<ProcessVM> processes = new ObservableCollection<ProcessVM>();

        public MainWindow()
        {
            this.InitializeComponent();

            this.lstProcesses.ItemsSource = this.processes;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ScreenCapture capturer = new ScreenCapture();

            foreach (var proc in Process.GetProcesses())
            {
                if (proc.MainWindowHandle != IntPtr.Zero)
                {
                    Bitmap bmp = capturer.CaptureWindow(proc.MainWindowHandle);
                    if (bmp == null)
                    {
                        // only give access to processes which provide a window
                        continue;
                    }

                    byte[] picture = ImageHandler.ImageHandler.ImageToBytes(ImageHandler.ImageHandler.Resize(bmp, pictureSize));
                    ProcessDescription p = new ProcessDescription(picture, proc.Id, proc.ProcessName, proc.MainWindowTitle);
                    processes.Add(new ProcessVM(p));
                }
            }
        }
        
        private void btnListen_Click(object sender, RoutedEventArgs e)
        {
            if (this.server != null && this.server.IsRunning)
            {
                return;
            }

            this.server = new Server(Remote_Content_Show_Protocol.NetworkConfiguration.Port);
            this.server.Start();
            this.btnListen.Content = "Listening...";
            this.btnListen.IsEnabled = false;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.server != null)
            {
                this.server.Stop();
            }
        }
    }
}

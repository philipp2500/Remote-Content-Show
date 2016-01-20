using Remote_Content_Show_Container;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows;

namespace Agent
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<ProcessVM> processes = new ObservableCollection<ProcessVM>();

        public MainWindow()
        {
            InitializeComponent();

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
                        continue;
                    }

                    byte[] picture = ImageToBytes(bmp);
                    ProcessDescription p = new ProcessDescription(picture, proc.Id, proc.ProcessName, proc.MainWindowTitle);
                    processes.Add(new ProcessVM(p));
                }
            }
        }

        public static byte[] ImageToBytes(Bitmap img)
        {
            byte[] byteArray = new byte[0];
            using (MemoryStream stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                stream.Close();

                byteArray = stream.ToArray();
            }

            return byteArray;
        }
    }
}

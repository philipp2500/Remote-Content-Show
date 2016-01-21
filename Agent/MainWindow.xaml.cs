using Agent.Network;
using Newtonsoft.Json;
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
        private System.Drawing.Size pictureSize = new System.Drawing.Size(100, 100);

        public MainWindow()
        {
            this.InitializeComponent();
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

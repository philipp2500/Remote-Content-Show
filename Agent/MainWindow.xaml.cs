using Agent.Network;
using System;
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

            try
            {
                this.server = new Server(Remote_Content_Show_Protocol.NetworkConfiguration.Port);
                this.server.OnClientConnected += this.Server_OnClientConnected;
                this.server.OnClientDisconnected += this.Server_OnClientDisconnected;
                this.server.Start();
                this.btnListen.Content = "Listening...";
                this.btnListen.IsEnabled = false;
            }
            catch
            {
            }
        }

        private void Server_OnClientDisconnected(object sender, EventArgs e)
        {
            this.lstOutput.Dispatcher.Invoke(() =>
            {
                this.lstOutput.Items.Add("A client has disconnected.");
            });
        }

        private void Server_OnClientConnected(object sender, EventArgs e)
        {
            this.lstOutput.Dispatcher.Invoke(() =>
            {
                this.lstOutput.Items.Add("A client has connected.");
            });
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

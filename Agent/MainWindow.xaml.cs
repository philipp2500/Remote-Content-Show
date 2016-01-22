using Agent.Network;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;

namespace Agent
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string TIME_FORMAT = "dd.MM.yyyy HH:mm:ss";
        private Server server = null;
        private NotifyIcon notifyIcon = null;
        private System.Drawing.Size pictureSize = new System.Drawing.Size(100, 100);

        public MainWindow()
        {
            this.InitializeComponent();

            this.notifyIcon = new NotifyIcon();
            this.notifyIcon.Icon = new Icon("../../Resources/Icon.ico");
            this.notifyIcon.MouseDoubleClick += NotifyIcon_MouseDoubleClick;
            this.notifyIcon.Visible = true;
        }
        
        private void btnListen_Click(object sender, RoutedEventArgs e)
        {
            if (this.server != null && this.server.IsRunning)
            {
                return;
            }

            try
            {
                this.server = new Server(Remote_Content_Show_Protocol.NetworkConfiguration.PortAgent);
                this.server.OnClientConnected += this.Server_OnClientConnected;
                this.server.OnClientDisconnected += this.Server_OnClientDisconnected;
                this.server.Start();

                this.btnListen.Content = "Warte auf Verbindungsanfragen...";
                this.btnListen.IsEnabled = false;
                
                this.lstOutput.Items.Add($"{DateTime.Now.ToString(TIME_FORMAT)} - Agent gestartet.");
                
                this.notifyIcon.BalloonTipTitle = "Agent gestartet";
                this.notifyIcon.BalloonTipText = "Warte auf Verbindungsanfragen...";
                this.notifyIcon.ShowBalloonTip(400);
            }
            catch
            {
            }
        }

        private void Server_OnClientDisconnected(object sender, EventArgs e)
        {
            string time = DateTime.Now.ToString(TIME_FORMAT);

            this.lstOutput.Dispatcher.Invoke(() =>
            {
                this.lstOutput.Items.Add($"{time} - Ein Client hat die Verbindung getrennt.");
            });

            this.notifyIcon.BalloonTipTitle = "Client getrennt";
            this.notifyIcon.BalloonTipText = $"{time} - Ein Client hat die Verbindung getrennt.";
            this.notifyIcon.ShowBalloonTip(400);
        }

        private void Server_OnClientConnected(object sender, EventArgs e)
        {
            string time = DateTime.Now.ToString(TIME_FORMAT);

            this.lstOutput.Dispatcher.Invoke(() =>
            {
                this.lstOutput.Items.Add($"{time} - Ein Client hat eine Verbindung hergestellt.");
            });

            this.notifyIcon.BalloonTipTitle = "Neuer Client";
            this.notifyIcon.BalloonTipText = $"{time} - Ein Client hat eine Verbindung hergestellt.";
            this.notifyIcon.ShowBalloonTip(400);
        }

        private void NotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.WindowState = WindowState.Normal;
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                this.ShowInTaskbar = false;
            }
            else if (this.WindowState == WindowState.Normal)
            {
                this.ShowInTaskbar = true;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.notifyIcon.Visible = false;

            if (this.server != null)
            {
                this.server.Stop();
            }
        }
    }
}

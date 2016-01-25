using Agent.Network;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows;

namespace Agent
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private const string TIME_FORMAT = "dd.MM.yyyy HH:mm:ss";
        private bool notificationsEnabled = false;
        private Server server = null;
        private System.Windows.Forms.NotifyIcon notifyIcon = null;
        private System.Drawing.Size pictureSize = new System.Drawing.Size(100, 100);
        
        public MainWindow()
        {
            this.InitializeComponent();

            this.chbNotification.DataContext = this;
            this.NotificationsEnabled = true;
                        
            this.notifyIcon = new System.Windows.Forms.NotifyIcon();
            this.notifyIcon.Icon = Properties.Resources.Icon1;
            this.notifyIcon.MouseDoubleClick += this.NotifyIcon_MouseDoubleClick;
            this.notifyIcon.Visible = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets a value indicating whether taskbar notifications should be displayed.
        /// </summary>
        public bool NotificationsEnabled
        {
            get
            {
                return this.notificationsEnabled;
            }
            set
            {
                this.notificationsEnabled = value;

                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("NotificationsEnabled"));
                }
            }
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
                
                if (this.NotificationsEnabled)
                {
                    this.notifyIcon.BalloonTipTitle = "Agent gestartet";
                    this.notifyIcon.BalloonTipText = "Warte auf Verbindungsanfragen...";
                    this.notifyIcon.ShowBalloonTip(400);
                }
            }
            catch
            {
                MessageBox.Show(
                    "Konnte Agent nicht starten.\n" +
                    $"Bitte überprüfen Sie, ob Port {Remote_Content_Show_Protocol.NetworkConfiguration.PortAgent} zum Binden bereit ist.",
                    "Start fehlgeschlagen",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        private void Server_OnClientDisconnected(object sender, EventArgs e)
        {
            string time = DateTime.Now.ToString(TIME_FORMAT);

            this.lstOutput.Dispatcher.Invoke(() =>
            {
                this.lstOutput.Items.Add($"{time} - Ein Client hat die Verbindung getrennt.");
            });

            if (this.NotificationsEnabled)
            {
                this.notifyIcon.BalloonTipTitle = "Client getrennt";
                this.notifyIcon.BalloonTipText = $"{time} - Ein Client hat die Verbindung getrennt.";
                this.notifyIcon.ShowBalloonTip(400);
            }
        }

        private void Server_OnClientConnected(object sender, EventArgs e)
        {
            string time = DateTime.Now.ToString(TIME_FORMAT);

            this.lstOutput.Dispatcher.Invoke(() =>
            {
                this.lstOutput.Items.Add($"{time} - Ein Client hat eine Verbindung hergestellt.");
            });

            if (this.NotificationsEnabled)
            {
                this.notifyIcon.BalloonTipTitle = "Neuer Client";
                this.notifyIcon.BalloonTipText = $"{time} - Ein Client hat eine Verbindung hergestellt.";
                this.notifyIcon.ShowBalloonTip(400);
            }
        }

        /// <summary>
        /// Restores the window from the taskbar.
        /// </summary>
        private void NotifyIcon_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
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

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            this.notifyIcon.Visible = false;
            this.notifyIcon.Dispose();

            if (this.server != null)
            {
                this.server.Stop();
            }
        }
    }
}

using Agent.Network;
using System;
using System.ComponentModel;
using System.Net.Sockets;
using System.Windows;

namespace Agent
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private const string TIME_FORMAT = "dd.MM.yyyy HH:mm:ss";
        private const int NOTIFICATION_POPUP_DURATION = 1000;
        private bool notificationsEnabled = false;
        private Server server = null;
        private System.Windows.Forms.NotifyIcon notifyIcon = null;

        /// <summary>
        /// The size of the process preview images.
        /// </summary>
        private System.Drawing.Size pictureSize = new System.Drawing.Size(100, 100);
        
        public MainWindow()
        {
            this.InitializeComponent();
            
            this.chbNotification.DataContext = this;
            this.NotificationsEnabled = true;
            
            this.notifyIcon = new System.Windows.Forms.NotifyIcon();
            this.notifyIcon.Icon = Properties.Resources.Icon1;
            this.notifyIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.notifyIcon.Text = "RCS Agent";
            this.notifyIcon.Visible = true;
            this.notifyIcon.MouseDoubleClick += this.NotifyIcon_MouseDoubleClick;

            this.StartServer();
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

        private void StartServer()
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
                this.server.OnClientKeepAliveOmitted += this.Server_OnClientKeepAliveOmitted;
                this.server.Start();

                this.btnListen.Content = "Warte auf Verbindungsanfragen...";
                this.btnListen.IsEnabled = false;

                this.lstOutput.Items.Add($"{DateTime.Now.ToString(TIME_FORMAT)} - Agent gestartet (Port {Remote_Content_Show_Protocol.NetworkConfiguration.PortAgent}).");

                this.ShowNotification("RCS Agent gestartet", "Warte auf Verbindungsanfragen...");
            }
            catch (SocketException)
            {
                MessageBox.Show(
                    "Konnte Agent nicht starten.\n" +
                    $"Bitte überprüfen Sie, ob Port {Remote_Content_Show_Protocol.NetworkConfiguration.PortAgent} zum Binden bereit ist.",
                    "RCS Agent : Start fehlgeschlagen",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }
        
        /// <summary>
        /// Shows a taskbar notification popup if <see cref="MainWindow.NotificationsEnabled"/> is true.
        /// </summary>
        /// <param name="title">The notification's title.</param>
        /// <param name="text">The notification's text.</param>
        private void ShowNotification(string title, string text)
        {
            if (!this.NotificationsEnabled)
            {
                return;
            }

            this.notifyIcon.BalloonTipTitle = title;
            this.notifyIcon.BalloonTipText = text;
            this.notifyIcon.ShowBalloonTip(NOTIFICATION_POPUP_DURATION);
        }

        private void btnListen_Click(object sender, RoutedEventArgs e)
        {
            this.StartServer();
        }

        private void Server_OnClientConnected(object sender, ConnectionEventArgs e)
        {
            string time = DateTime.Now.ToString(TIME_FORMAT);

            this.lstOutput.Dispatcher.Invoke(() =>
            {
                this.lstOutput.Items.Add($"{time} - Ein Client hat eine Verbindung hergestellt ({e.RemoteEndPoint}).");
            });

            this.ShowNotification("Neuer Client", $"{time} - Ein Client hat eine Verbindung hergestellt.");
        }

        private void Server_OnClientDisconnected(object sender, ConnectionEventArgs e)
        {
            string time = DateTime.Now.ToString(TIME_FORMAT);

            this.lstOutput.Dispatcher.Invoke(() =>
            {
                this.lstOutput.Items.Add($"{time} - Die Verbindung zu einem Client wurde getrennt ({e.RemoteEndPoint}).");
            });

            this.ShowNotification("Client getrennt", $"{time} - Die Verbindung zu einem Client wurde getrennt.");
        }

        private void Server_OnClientKeepAliveOmitted(object sender, ConnectionEventArgs e)
        {
            string time = DateTime.Now.ToString(TIME_FORMAT);

            this.lstOutput.Dispatcher.Invoke(() =>
            {
                this.lstOutput.Items.Add($"{time} - Ein Client ({e.RemoteEndPoint}) hat zu lange keine Keep-Alive-Nachrichten gesendet. Die Verbindung wird abgebrochen.");
            });

            this.ShowNotification("Client getrennt", $"{time} - Ein Client hat zu lange keine Keep-Alives gesendet.");
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

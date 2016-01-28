using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Forms;

namespace ITServicesDataServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Server server;
        private NotifyIcon notifyIcon = null;
        private ObservableCollection<RequestHandler> requests = new ObservableCollection<RequestHandler>();
        
        public MainWindow()
        {
            InitializeComponent();
            this.Requests.ItemsSource = requests;

            this.notifyIcon = new NotifyIcon();
            this.notifyIcon.Icon = Properties.Resources.icon1;
            this.notifyIcon.Text = "IT Services Data Server";
            this.notifyIcon.Visible = true;
            this.notifyIcon.MouseDoubleClick += this.NotifyIcon_MouseDoubleClick;
        }
        
        private void Start_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.server = new Server(1053);
                this.server.OnRequest += Server_OnRequest;
                this.Start.IsEnabled = false;
            }
            catch
            {
                System.Windows.MessageBox.Show("Fehler beim Starten des Servers!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Server_OnRequest(object sender, RequestHandler e)
        {            
            this.Dispatcher.Invoke(() =>
                {
                    this.requests.Add(e);
                }
            );
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            this.Stop.IsEnabled = false;
            if (this.server != null)
            {
                this.server.Stop();
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
            // make the notify icon disappear from taskbar
            this.notifyIcon.Visible = false;
            this.notifyIcon.Icon = null;
            this.notifyIcon.Dispose();

            if (this.server != null)
            {
                this.server.Stop();
            }
        }
    }
}

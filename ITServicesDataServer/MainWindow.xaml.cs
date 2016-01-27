using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace ITServicesDataServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Server server;
        private ObservableCollection<RequestHandler> requests = new ObservableCollection<RequestHandler>();

        public MainWindow()
        {
            InitializeComponent();
            this.Requests.ItemsSource = requests;
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            this.server = new Server(1053);
            this.server.OnRequest += Server_OnRequest;
            this.Start.IsEnabled = false;
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

        }
    }
}

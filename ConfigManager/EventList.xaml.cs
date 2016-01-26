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
using System.Windows.Shapes;
using Remote_Content_Show_Container;
using System.Net;
using Remote_Content_Show_Protocol;

namespace ConfigManager
{
    /// <summary>
    /// Interaction logic for EventList.xaml
    /// </summary>
    public partial class EventList : Window
    {
        private ObservableCollection<EventItemVM> eventItems = new ObservableCollection<EventItemVM>();
        private NetworkManager netmanager = new NetworkManager();

        public EventList()
        {
            InitializeComponent();
            this.ErrorListBox.ItemsSource = eventItems;
            this.netmanager.OnError += Netmanager_OnError;
            this.netmanager.OnMessageReceived += Netmanager_OnMessageReceived;
        }

        private void Netmanager_OnError(object sender, ErrorEventHandler e)
        {
            MessageBox.Show(e.Text, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void Netmanager_OnMessageReceived(object sender, MessageRecivedEventHandler e)
        {
            switch (e.Code)
            {
                case MessageCode.MC_Event_List_Response:
                    RCS_Event_List_Response rcsELR = Remote_Content_Show_MessageGenerator.GetMessageFromByte<RCS_Event_List_Response>(e.MessageData);
                    this.Dispatcher.Invoke(() =>
                        {
                            foreach (Event ev in rcsELR.Event_List.List)
                            {
                                this.eventItems.Add(new EventItemVM(ev));
                            }

                            this.ErrorListBox.ItemsSource = eventItems;
                        }
                    );
                    break;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(this.IpInput.Text))
            {
                try
                {
                    IPAddress[] ips = Dns.GetHostAddresses(this.IpInput.Text);
                    if (ips.Length == 0)
                    {
                        MessageBox.Show("IP/Name konnte nicht aufgelöst werden!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        NetworkConnection netC = this.netmanager.ConnectTo(ips[0], NetworkConfiguration.PortPi);
                        byte[] messageData = Remote_Content_Show_MessageGenerator.GetMessageAsByte(new RCS_Event_List_Request(RemoteType.Configurator));
                        netC.Write(messageData, MessageCode.MC_Event_List_Request);

                        this.ErrorListBox.Items.Clear();
                        this.IpInput.Text = string.Empty;
                        eventItems = new ObservableCollection<EventItemVM>();
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("IP/Name konnte nicht aufgelöst werden!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}

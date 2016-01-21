using System;
using System.Collections.Generic;
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
using System.Net;
using Remote_Content_Show_Protocol;
using Remote_Content_Show_Container;

namespace ConfigManager
{
    /// <summary>
    /// Interaction logic for NewJob.xaml
    /// </summary>
    public partial class NewJob : Window
    {
        private NetworkManager netmanager = new NetworkManager();
        private List<ProcessDescriptionWrapper> allProcess = new List<ProcessDescriptionWrapper>();

        public NewJob()
        {
            InitializeComponent(); 
            this.netmanager.OnError += Netmanager_OnError;
            this.netmanager.OnMessageReceived += Netmanager_OnMessageReceived;
        }

        private void Netmanager_OnMessageReceived(object sender, MessageRecivedEventHandler e)
        {
            switch (e.Code)
            {
                case MessageCode.MC_Process_List_Response:
                    RCS_Process_List_Response rcsPLR = Remote_Content_Show_MessageGenerator.GetMessageFromByte<RCS_Process_List_Response>(e.MessageData);
                    this.Dispatcher.Invoke(() =>
                        {
                            AgentProcessOverview apo = new AgentProcessOverview(rcsPLR, e.Ip);
                            this.AddedAgents.Items.Add(apo);
                        }
                    );
                    // Add the results for later!
                    foreach (ProcessDescription pd in rcsPLR.ProcesseList.Processes)
                    {
                        this.allProcess.Add(new ProcessDescriptionWrapper(pd, e.Ip));
                    }
                    break;
            }
        }

        private void Netmanager_OnError(object sender, ErrorEventHandler e)
        {
            MessageBox.Show(e.Text, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void AddAgent_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(this.AgentConnection.Text))
            {
                try
                {
                    IPAddress[] ips = Dns.GetHostAddresses(this.AgentConnection.Text);
                    if (ips.Length == 0)
                    {
                        MessageBox.Show("IP/Name konnte nicht aufgelöst werden!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        NetworkConnection netC = this.netmanager.ConnectTo(ips[0]);
                        byte[] messageData = Remote_Content_Show_MessageGenerator.GetMessageAsByte(new RCS_Process_List_Request());
                        netC.Write(messageData, MessageCode.MC_Process_List_Request);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("IP/Name konnte nicht aufgelöst werden!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ToContent_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(this.JobName.Text))
            {
                this.Tab1.IsEnabled = false;
                this.Tab2.IsSelected = true;
                this.Tab2.IsEnabled = true;
            }
            else
            {
                MessageBox.Show("Bitte Namen für den Job eingeben!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}

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
using Microsoft.Win32;
using System.IO;

namespace ConfigManager
{
    /// <summary>
    /// Interaction logic for NewJob.xaml
    /// </summary>
    public partial class NewJob : Window
    {
        private NetworkManager netmanager = new NetworkManager();
        private List<ProcessDescriptionWrapper> allProcess = new List<ProcessDescriptionWrapper>();
        private List<TimeLineControl> timelines = new List<TimeLineControl>();
        private Job_Configuration job = new Job_Configuration();
        private IResource currenResource;

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
                        this.allProcess.Add(new ProcessDescriptionWrapper(pd, e.Ip, rcsPLR.ClientName));
                    }

                    this.job.Agents.Add(new Agent() { IP = e.Ip.ToString() });
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
                        NetworkConnection netC = this.netmanager.ConnectToRead(ips[0], NetworkConfiguration.PortAgent);
                        byte[] messageData = Remote_Content_Show_MessageGenerator.GetMessageAsByte(new RCS_Process_List_Request(RemoteType.Configurator));
                        netC.Write(messageData, MessageCode.MC_Process_List_Request);

                        this.AgentConnection.Text = string.Empty;
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("IP/Name konnte nicht aufgelöst werden!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ToContent_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(this.JobName.Text))
            {
                this.netmanager.CloseAllConnection();
                this.Tab1.IsEnabled = false;
                this.Tab2.IsSelected = true;
                this.Tab2.IsEnabled = true;
                // Change Lyout
                this.LayoutImagOverwie.Source = ((Image)((ComboBoxItem)this.JobLayout.SelectedItem).Content).Source;

                this.job.Name = this.JobName.Text;
                this.job.Layout = (WindowLayout)this.JobLayout.SelectedIndex;
                this.ToShowWindowId.Maximum = WindowLayoutHelper.GetWindows(this.job.Layout);

                for (int i = 1; i <= WindowLayoutHelper.GetWindows(this.job.Layout); i++)
                {
                    TimeLineControl tlc = new TimeLineControl(i);
                    this.TimeLineContainer.Children.Add(tlc);
                    this.timelines.Add(tlc);
                }
            }
            else
            {
                MessageBox.Show("Bitte Namen für den Job eingeben!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ToAction_Click(object sender, RoutedEventArgs e)
        {
            if (this.timelines.Sum(x => x.Items.Count) > 0)
            {
                this.Tab2.IsEnabled = false;
                this.Tab3.IsSelected = true;
                this.Tab3.IsEnabled = true;

                foreach (TimeLineControl tlc in this.timelines)
                {
                    JobWindowList jwl = new JobWindowList();
                    jwl.WindowLayoutNumber = tlc.WindowLayoutId;
                    jwl.Looping = tlc.Loop;
                    int i = 1;             
                    foreach (TimeLineItemVM tlivm in tlc.Items)
                    {
                        jwl.Jobs.Add(new Job() { Duration = tlivm.Duration, Resource = tlivm.Resource, OrderingNumber = i });
                        i++;
                    }

                    this.job.JobLists.Add(tlc.WindowLayoutId, jwl);                    
                }
            }
            else
            {
                MessageBox.Show("Der Job enthält keine Elemente!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (((ComboBox)sender).SelectedIndex)
            {
                case 0:
                    // Proses
                    ProcessDialog pd = new ProcessDialog(this.allProcess);
                    if (pd.ShowDialog() == true)
                    {
                        ProcessDescriptionWrapper pdw = this.allProcess.Find(x => x.Id == pd.Result);
                        this.currenResource = new ProcessResource() { ProcessID = pdw.PDescription.ProcessId, ProcessAgent = new Agent() { IP = pdw.Ip.ToString() }, Name = pdw.PDescription.ProcessName };
                        this.SelectedName.Text = this.currenResource.Name;
                    }
                    break;
                case 1:
                    //Web
                    UrlDialog ud = new UrlDialog();
                    if (ud.ShowDialog() == true)
                    {
                        this.currenResource = new WebResource() { Path = ud.Url,  Name = new Uri(ud.Url).Host };
                        this.SelectedName.Text = this.currenResource.Name;
                    }
                    break;
                case 2:
                    // File
                    OpenFileDialog ofd = new OpenFileDialog();
                    ofd.Multiselect = false;
                    if (ofd.ShowDialog() == true)
                    {
                        if (!string.IsNullOrWhiteSpace(ofd.FileName))
                        {
                            this.currenResource = new FileResource() { Path = ofd.FileName, Name = System.IO.Path.GetFileName(ofd.FileName) };
                            this.SelectedName.Text = this.currenResource.Name;
                        }
                    }
                    break;
            }            
        }

        private void AddJobToTimeLine_Click(object sender, RoutedEventArgs e)
        {
            if (this.currenResource != null)
            {
                int duration = ((int)this.ToDurationMinuten.Value) * 60 + (int)this.ToDurationSekunden.Value;
                TimeLineItemVM tlivm = new TimeLineItemVM(duration, this.currenResource);
                ((TimeLineControl)this.TimeLineContainer.Children[(int)this.ToShowWindowId.Value - 1]).Add(tlivm);
                this.SelectIResource.SelectedIndex = -1;
                this.SelectedName.Text = string.Empty;
                this.currenResource = null;
            }
            else
            {
                MessageBox.Show("Bitte eine Resource auswählen!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void CloseThisThing_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SpeicherDenJob_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.AddExtension = true;
            sfd.DefaultExt = "rcs";
            sfd.Filter = "Remote Control Show Job (*.rcs)|*.rcs|All files (*.*)|*.*";

            if (sfd.ShowDialog() == true)
            {
                if (!string.IsNullOrWhiteSpace(sfd.FileName))
                {
                    try
                    {
                        byte[] data = Remote_Content_Show_MessageGenerator.GetMessageAsByte(new RCS_Job(this.job, RemoteType.Configurator));

                        FileStream fs = new FileStream(sfd.FileName, FileMode.Create);                       
                        fs.Write(data, 0, data.Length);
                        fs.Flush();
                        fs.Close();
                    }
                    catch
                    {
                        MessageBox.Show("Fehler beim Speicher!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
        }

        private void SendJob_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(this.IptosendJob.Text))
            {
                try
                {
                    IPAddress[] ips = Dns.GetHostAddresses(this.IptosendJob.Text);
                    if (ips.Length == 0)
                    {
                        MessageBox.Show("IP/Name konnte nicht aufgelöst werden!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        NetworkConnection netC = this.netmanager.ConnectTo(ips[0], NetworkConfiguration.PortPi);
                        byte[] messageData = Remote_Content_Show_MessageGenerator.GetMessageAsByte(new RCS_Job(this.job, RemoteType.Configurator));
                        netC.Write(messageData, MessageCode.MC_Job);

                        this.ToPiSendList.Items.Add(new ListViewItem() { Content = netC.Ip + " " + this.job.Name });
                        this.IptosendJob.Text = string.Empty;
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

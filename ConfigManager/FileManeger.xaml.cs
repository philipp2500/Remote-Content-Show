using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
using Remote_Content_Show_Protocol;
using System.Collections.ObjectModel;
using Microsoft.Win32;
using System.IO;

namespace ConfigManager
{
    /// <summary>
    /// Interaction logic for FileManeger.xaml
    /// </summary>
    public partial class FileManeger : Window
    {
        private NetworkManager netmanager;
        private ObservableCollection<FileItem> items = new ObservableCollection<FileItem>();
        private IPAddress ip;

        public FileManeger()
        {
            InitializeComponent();
            this.Elemets.ItemsSource = items;
            this.netmanager = new NetworkManager();
            this.netmanager.OnError += Netmanager_OnError;
            this.netmanager.OnMessageReceived += Netmanager_OnMessageReceived;
        }

        private void Netmanager_OnMessageReceived(object sender, MessageRecivedEventHandler e)
        {
            switch (e.Code)
            {
                case MessageCode.MC_FileList:
                    RCS_FileList gf = Remote_Content_Show_MessageGenerator.GetMessageFromByte<RCS_FileList>(e.MessageData);
                    foreach (FileItem fi in gf.Items)
                    {
                        this.items.Add(fi);
                    }
                    this.Dispatcher.Invoke(() =>
                        {
                            this.Elemets.ItemsSource = items;
                        }
                    );
                    break;
            }
        }

        private void Netmanager_OnError(object sender, ErrorEventHandler e)
        {
            MessageBox.Show(e.Text, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void LoadDenBild_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == true)
            {
                if (!string.IsNullOrWhiteSpace(ofd.FileName))
                {
                    try
                    {
                        RCS_FileAdd fa = new RCS_FileAdd(RemoteType.Configurator);
                        FileStream fs = new FileStream(ofd.FileName, FileMode.Open);
                        fs.Seek(0, SeekOrigin.Begin);
                        byte[] data = new byte[fs.Length];
                        fs.Read(data, 0, data.Length);
                        fs.Close();

                        fa.Data = data;
                        fa.Name = ofd.SafeFileName;

                        if (this.items.Count(x => x.Name == fa.Name) == 0)
                        {
                            try
                            {
                                NetworkConnection netC = this.netmanager.ConnectTo(ip, NetworkConfiguration.PortPi);
                                byte[] messageData = Remote_Content_Show_MessageGenerator.GetMessageAsByte(fa);
                                netC.Write(messageData, MessageCode.MC_FileAdd);
                                this.items.Add(new FileItem() { Name = fa.Name });
                                this.FileName.Text = fa.Name;
                            }
                            catch
                            {
                            }
                        }
                        else
                        {
                            MessageBox.Show("Der File existiert schon!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                        }                            
                    }
                    catch
                    {
                        MessageBox.Show("Fehler beim Laden!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                    }   
                }
            }
        }

        private void CloseThisThing_Click(object sender, RoutedEventArgs e)
        {
            this.netmanager.CloseAllConnection();
            this.Close();
        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(this.IptoGetFiles.Text))
            {
                try
                {
                    IPAddress[] ips = Dns.GetHostAddresses(this.IptoGetFiles.Text);
                    if (ips.Length == 0)
                    {
                        MessageBox.Show("IP/Name konnte nicht aufgelöst werden!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        NetworkConnection netC = this.netmanager.ConnectToRead(ips[0], NetworkConfiguration.PortPi);
                        this.ip = ips[0];
                        byte[] messageData = Remote_Content_Show_MessageGenerator.GetMessageAsByte(new RCS_GetFiles(RemoteType.Configurator));
                        netC.Write(messageData, MessageCode.MC_GetFiles);
                        this.items = new ObservableCollection<FileItem>();
                        this.FileName.Text = string.Empty;
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("IP/Name konnte nicht aufgelöst werden!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FileItem fi = (FileItem)((Button)sender).Tag;
            try
            {
                NetworkConnection netC = this.netmanager.ConnectTo(ip, NetworkConfiguration.PortPi);
                byte[] messageData = Remote_Content_Show_MessageGenerator.GetMessageAsByte(new RCS_FileDelete(fi.Name, RemoteType.Configurator));
                netC.Write(messageData, MessageCode.MC_FileDelete);
                this.items.Remove(fi);
            }
            catch
            {
                MessageBox.Show("Konnte nicht hinzugefügt werden!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }            
        }
    }
}

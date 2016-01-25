using Microsoft.Win32;
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
using ImageHandler;
using Remote_Content_Show_Container;
using Remote_Content_Show_Protocol;
using System.IO;
using System.Net;

namespace ConfigManager
{
    /// <summary>
    /// Interaction logic for CreateConfriguration.xaml
    /// </summary>
    public partial class CreateConfriguration : Window
    {
        private byte[] image;
        private NetworkManager netmanager = new NetworkManager();
        private string imageName;

        public CreateConfriguration()
        {
            InitializeComponent();
        }

        private void SendJobB_Click(object sender, RoutedEventArgs e)
        {
            if (this.image != null && this.image.Length > 0)
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
                            byte[] messageData = Remote_Content_Show_MessageGenerator.GetMessageAsByte(new RCS_Configuration_Image(this.image, RemoteType.Configurator));
                            netC.Write(messageData, MessageCode.MC_Configuration_Image);

                            this.ToPiSendList.Items.Add(new ListViewItem() { Content = netC.Ip + " " + this.imageName });
                            this.IptosendJob.Text = string.Empty;
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("IP/Name konnte nicht aufgelöst werden!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Sie haben kein Bild geladen!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void LoadDenBild_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            ofd.Filter = "Image Files (*.png, *.jpg)|*.png;*.jpg|All files (*.*)|*.*";
            if (ofd.ShowDialog() == true)
            {
                if (!string.IsNullOrWhiteSpace(ofd.FileName))
                {
                    try
                    {
                        FileStream fs = new FileStream(ofd.FileName, FileMode.Open);
                        fs.Seek(0, SeekOrigin.Begin);
                        byte[] data = new byte[fs.Length];
                        fs.Read(data, 0, data.Length);
                        fs.Close();
                        this.image = data;
                        BitmapImage bi = ImageHandler.ImageHandler.BytesToImage(data);
                        this.SelectedImag.Source = bi;
                        this.ImagName.Text = ofd.SafeFileName;
                        this.imageName = ofd.SafeFileName;
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
    }
}

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
using Remote_Content_Show_Container;
using Remote_Content_Show_Protocol;
using Microsoft.Win32;
using System.IO;

namespace ConfigManager
{
    /// <summary>
    /// Interaction logic for SendJob.xaml
    /// </summary>
    public partial class SendJob : Window
    {
        private RCS_Job currentJob;

        public SendJob()
        {
            InitializeComponent();
        }

        private void CloseThisThing_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SendJobB_Click(object sender, RoutedEventArgs e)
        {
            if (this.currentJob != null)
            {

            }
        }

        private void LoadDenJob_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            ofd.Filter = "Text files (*.rcs)|*.rcs|All files (*.*)|*.*";
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
                        this.currentJob = Remote_Content_Show_MessageGenerator.GetMessageFromByte<RCS_Job>(data);
                        this.JobName.Text = this.currentJob.Configuration.Name;
                    }
                    catch
                    {
                        MessageBox.Show("Fehler beim Laden!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                    }

                }
            }
        }
    }
}

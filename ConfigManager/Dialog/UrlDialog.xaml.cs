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

namespace ConfigManager
{
    /// <summary>
    /// Interaction logic for UrlDialog.xaml
    /// </summary>
    public partial class UrlDialog : Window
    {
        public UrlDialog()
        {
            InitializeComponent();
        }

        public string Url
        {
            get;
            private set;
        }

        private void Cancle_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (this.IsUrl())
            {
                this.DialogResult = true;
                this.Url = this.UrlInput.Text;
                this.Close();
            }
            else
            {
                MessageBox.Show("Keine gültige URL!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool IsUrl()
        {
            try
            {
                string a = new Uri(this.UrlInput.Text).Host;
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

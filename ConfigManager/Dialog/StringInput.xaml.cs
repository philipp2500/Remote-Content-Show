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

namespace ConfigManager
{
    /// <summary>
    /// Interaction logic for StringInput.xaml
    /// </summary>
    public partial class StringInput : Window
    {
        public StringInput()
        {
            InitializeComponent();
        }
        public string Path
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
            if (!string.IsNullOrWhiteSpace(this.StringInputT.Text))
            {
                this.DialogResult = true;
                this.Path = this.StringInputT.Text;
                this.Close();
            }
            else
            {
                MessageBox.Show("Keine Eingabe!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

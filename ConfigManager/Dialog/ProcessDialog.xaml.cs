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

namespace ConfigManager
{
    /// <summary>
    /// Interaction logic for ProcessDialog.xaml
    /// </summary>
    public partial class ProcessDialog : Window
    {
        public Guid Result
        {
            get;
            private set;
        }

        public ProcessDialog(List<ProcessDescriptionWrapper> allProcess)
        {
            InitializeComponent();
            this.Result = Guid.Empty;
            foreach(ProcessDescriptionWrapper pdw in allProcess)
            {
                ComboBoxItem cbi = new ComboBoxItem();
                cbi.Tag = pdw.Id;

                ProcessPickerItem pit = new ProcessPickerItem();
                pit.PrcessName.Content = pdw.PDescription.ProcessName;
                pit.PrcessAgent.Content = pdw.AgentName;
                pit.PrcessTitle.Content = pdw.PDescription.ProcessTitle;
                pit.ProcessImag.Source = ImageHandler.ImageHandler.BytesToImage(pdw.PDescription.WindowPicture);

                cbi.Content = pit;
                this.ProcessList.Items.Add(cbi);
            }
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (this.Result != Guid.Empty)
            {
                this.DialogResult = true;
            }
            else
            {
                this.DialogResult = false;
            }

            this.Close();
        }

        private void Cancle_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void ProcessList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.Result = (Guid)((ComboBoxItem)this.ProcessList.SelectedItem).Tag;
        }
    }
}

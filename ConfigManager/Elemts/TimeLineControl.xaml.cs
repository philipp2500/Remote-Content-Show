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

namespace ConfigManager
{
    /// <summary>
    /// Interaction logic for TimeLineControl.xaml
    /// </summary>
    public partial class TimeLineControl : UserControl
    {
        public ObservableCollection<TimeLineItemVM> Items = new ObservableCollection<TimeLineItemVM>();

        public bool Loop
        {
            get
            {
                return (bool)this.LoopBox.IsChecked;
            }
        }

        public TimeLineControl(int windowId)
        {
            InitializeComponent();
            this.LyoutWindowNumber.Content = windowId;
            this.WindowLayoutId = windowId;
            this.TimeLineItems.ItemsSource = this.Items;
        }

        public void Add(TimeLineItemVM tivm)
        {
            this.Items.Add(tivm);
        }

        public int WindowLayoutId
        {
            get;
            private set;
        }

        private void TimeLineItems_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete && this.TimeLineItems.SelectedIndex >= 0)
            {
                this.Items.RemoveAt(this.TimeLineItems.SelectedIndex);
            }            
        }
    }
}

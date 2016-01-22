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
using GongSolutions.Wpf.DragDrop;

namespace ConfigManager
{
    /// <summary>
    /// Interaction logic for TimeLineControl.xaml
    /// </summary>
    public partial class TimeLineControl : UserControl, IDropTarget
    {
        private ObservableCollection<TimeLineItemVM> items = new ObservableCollection<TimeLineItemVM>();

        public TimeLineControl(int windowId)
        {
            InitializeComponent();
            this.LyoutWindowNumber.Content = windowId;
            this.WindowLayoutId = windowId;
            this.TimeLineItems.ItemsSource = this.items;
        }

        public void Add(TimeLineItemVM tivm)
        {
            this.items.Add(tivm);
        }

        public int WindowLayoutId
        {
            get;
            private set;
        }

        void IDropTarget.DragOver(IDropInfo dropInfo)
        {
            TimeLineItemVM sourceItem = dropInfo.Data as TimeLineItemVM;
            TimeLineItemVM targetItem = dropInfo.TargetItem as TimeLineItemVM;

            if (sourceItem != null && targetItem != null)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                dropInfo.Effects = DragDropEffects.Copy;
            }
        }

        void IDropTarget.Drop(IDropInfo dropInfo)
        {
            TimeLineItemVM sourceItem = dropInfo.Data as TimeLineItemVM;
            TimeLineItemVM targetItem = dropInfo.TargetItem as TimeLineItemVM;
           
        }        
    }
}

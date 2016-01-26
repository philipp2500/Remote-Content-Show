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
using System.Windows.Shapes;
using Remote_Content_Show_Container;

namespace ConfigManager
{
    /// <summary>
    /// Interaction logic for LayoutDesigner.xaml
    /// </summary>
    public partial class LayoutDesigner : Window
    {
        private ObservableCollection<LayoutItemVM> items = new ObservableCollection<LayoutItemVM>();
        private Point startPoint;
        private Point endPoint;
        private int index = 1;

        public LayoutDesigner()
        {
            InitializeComponent();
            this.Layoutwindow.ItemsSource = items;
            this.items.Add(new LayoutItemVM() { Item = new LayoutItem(0, 0, 0, 0, index), HeightParent = this.Layoutwindow.ActualHeight, WidthParent = this.Layoutwindow.ActualWidth });
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ColorPi_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (this.Layoutwindow != null)
            {
                this.Layoutwindow.Background = new SolidColorBrush((Color)this.ColorPi.SelectedColor);
            }
        }
        private void Layoutwindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (LayoutItemVM li in this.items)
            {
                li.WidthParent = this.Layoutwindow.ActualWidth;
                li.HeightParent = this.Layoutwindow.ActualHeight;
            }
        }

        private void Layoutwindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.startPoint = e.GetPosition(this.Layoutwindow);
        }

        private void Layoutwindow_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //Point endPoint = e.GetPosition(this);
            //b.Width = endPoint.X - startPoint.X;
            //b.Height = endPoint.Y - startPoint.Y;##
        }

        private void Layoutwindow_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.endPoint = e.GetPosition(this);
                this.items[0].Item.MarginLeft = Math.Min(this.startPoint.X, this.endPoint.Y);
                this.items[0].Item.MarginTop = Math.Min(this.startPoint.Y, this.endPoint.Y);
                this.items[0].Item.Width = Math.Max(this.startPoint.X, this.endPoint.Y) - Math.Min(this.startPoint.X, this.endPoint.Y);
                this.items[0].Item.Height = Math.Max(this.startPoint.Y, this.endPoint.Y) - Math.Min(this.startPoint.Y, this.endPoint.Y);
            }
        }


    }
}

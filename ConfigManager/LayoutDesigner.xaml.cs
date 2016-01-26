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

namespace ConfigManager
{
    /// <summary>
    /// Interaction logic for LayoutDesigner.xaml
    /// </summary>
    public partial class LayoutDesigner : Window
    {
        private ObservableCollection<TimeLineItemVM> Items = new ObservableCollection<TimeLineItemVM>();
        private Border b = new Border();
        private Point startPoint;

        public LayoutDesigner()
        {
            InitializeComponent();
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

        private void Layoutwindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            startPoint = e.GetPosition(this);
            b.Margin = new Thickness(startPoint.X, startPoint.Y, 0, 0);
            b.BorderThickness = new Thickness(3);
            b.BorderBrush = Brushes.Black;
            this.Layoutwindow.Children.Add(b);
        }

        private void Layoutwindow_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Point endPoint = e.GetPosition(this);
            b.Width = endPoint.X - startPoint.X;
            b.Width = endPoint.Y - startPoint.Y;
        }
    }
}

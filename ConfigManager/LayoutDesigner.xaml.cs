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
using Microsoft.Win32;
using Remote_Content_Show_Protocol;
using System.IO;

namespace ConfigManager
{
    /// <summary>
    /// Interaction logic for LayoutDesigner.xaml
    /// </summary>
    public partial class LayoutDesigner : Window
    {
        private const double percent = 0.1;
        private ObservableCollection<LayoutItemVM> items = new ObservableCollection<LayoutItemVM>();
        private Point startPoint;
        private Point endPoint;
        private int index = 1;

        public LayoutDesigner()
        {
            InitializeComponent();
            this.Layoutwindow.ItemsSource = this.items;
            this.ListofItems.ItemsSource = this.items;            
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(this.NamedesLayouts.Text))
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.AddExtension = true;
                sfd.DefaultExt = "rcs";
                sfd.Filter = "Remote Control Layout (*.rcl)|*.rcl|All files (*.*)|*.*";

                if (sfd.ShowDialog() == true)
                {
                    if (!string.IsNullOrWhiteSpace(sfd.FileName))
                    {
                        try
                        {
                            Color co = (Color)this.ColorPi.SelectedColor;
                            WindowLayout wl = new WindowLayout() { Name = this.NamedesLayouts.Text, Color = new byte[] { co.A, co.R, co.G, co.B } };
                            foreach (LayoutItemVM li in this.items)
                            {
                                wl.Items.Add(li.Item);
                            }

                            byte[] data = Remote_Content_Show_MessageGenerator.GetMessageAsByte(wl);

                            FileStream fs = new FileStream(sfd.FileName, FileMode.Create);
                            fs.Write(data, 0, data.Length);
                            fs.Flush();
                            fs.Close();
                        }
                        catch
                        {
                            MessageBox.Show("Fehler beim Speicher!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Bitte Namen für das Layout eingeben!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
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
            this.items.Add(new LayoutItemVM() { Item = new LayoutItem(0, 0, 0, 0, index), HeightParent = this.Layoutwindow.ActualHeight, WidthParent = this.Layoutwindow.ActualWidth });
            index++;
        }

        private void Layoutwindow_MouseMove(object sender, MouseEventArgs e)
        {
            this.endPoint = e.GetPosition(this.Layoutwindow);
            if (e.LeftButton == MouseButtonState.Pressed)
            {                
                this.items.Last().ItemMarginLeft = Math.Min(this.startPoint.X, this.endPoint.X) / this.Layoutwindow.ActualWidth;
                this.items.Last().ItemMarginTop = Math.Min(this.startPoint.Y, this.endPoint.Y) / this.Layoutwindow.ActualHeight;
                this.items.Last().ItemWidth= (Math.Max(this.startPoint.X, this.endPoint.X) - Math.Min(this.startPoint.X, this.endPoint.X)) / this.Layoutwindow.ActualWidth;
                this.items.Last().ItemHeight = (Math.Max(this.startPoint.Y, this.endPoint.Y) - Math.Min(this.startPoint.Y, this.endPoint.Y)) / this.Layoutwindow.ActualHeight;
            }

            this.CursorPosituin.Content = string.Format("X: {0}  Y: {1}", Math.Truncate((this.endPoint.X / this.Layoutwindow.ActualWidth) * 100), Math.Truncate((this.endPoint.Y / this.Layoutwindow.ActualHeight) * 100));
        }
    }
}

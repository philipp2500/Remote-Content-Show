using Remote_Content_Show_Container;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace DisplayClient.Layout
{
    public class CustomWindow : IContentWindow
    {
        private WindowLayout layout;

        private Grid root;

        public CustomWindow(WindowLayout layout)
        {
            this.layout = layout;
        }

        public static WindowLayout GetTestLayout()
        {
            WindowLayout layout = new WindowLayout();
            layout.Color = new byte[] { 255, 0, 0, 255 };
            layout.Name = "layout1";
            layout.Items = new List<LayoutItem>();
            layout.Items.Add(new LayoutItem(0, 0, 0.5, 1, 1));
            layout.Items.Add(new LayoutItem(0.5, 0, 0.5, 1, 2));
            
            return layout;
        }

        public void Generate()
        {
            this.root = new Grid();
            this.root.Margin = new Thickness(0, 0, 0, 0);
            this.root.HorizontalAlignment = HorizontalAlignment.Left;
            this.root.VerticalAlignment = VerticalAlignment.Top;
            this.root.Background = new SolidColorBrush(Color.FromArgb(this.layout.Color[0],
                                                                      this.layout.Color[1],
                                                                      this.layout.Color[2],
                                                                      this.layout.Color[3]));

            List<LayoutItem> ordered = layout.Items.OrderBy(x => x.Number).ToList();

            foreach (LayoutItem item in ordered)
            {
                Grid grid = new Grid();
                grid.Name = "display" + item.Number;
                grid.HorizontalAlignment = HorizontalAlignment.Left;
                grid.VerticalAlignment = VerticalAlignment.Top;

                this.root.Children.Add(grid);
            }
            
            //this.root.Width = rootSize.Width;
            //this.root.Height = rootSize.Height;
        }

        public void ApplySize(Size size)
        {
            this.root.Width = size.Width;
            this.root.Height = size.Height;

            Grid[] displays = this.GetAllDisplays();

            foreach (Grid display in displays)
            {
                LayoutItem accordingLayout = layout.Items.Where(x => display.Name.Equals("display" + x.Number)).ToList()[0];

                display.Width = LayoutHelper.GetActuelValue(accordingLayout.Width, size.Width);
                display.Height = LayoutHelper.GetActuelValue(accordingLayout.Height, size.Height);
                display.Margin = new Thickness(LayoutHelper.GetActuelValue(accordingLayout.MarginLeft, size.Width),
                                               LayoutHelper.GetActuelValue(accordingLayout.MarginTop, size.Height),
                                               0,
                                               0);
            }
        }

        public Grid[] GetAllDisplays()
        {
            return this.root.Children.Select(x => (Grid)x).ToArray();
        }

        public Grid GetRoot()
        {
            return this.root;
        }
    }
}

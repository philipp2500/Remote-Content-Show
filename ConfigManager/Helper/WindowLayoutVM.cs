using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Remote_Content_Show_Container;
using System.Windows.Media.Imaging;
using ImageHandler;
using System.Drawing;

namespace ConfigManager
{
    public class WindowLayoutVM
    {
        public WindowLayoutVM(WindowLayout layout)
        {
            this.Layout = layout;
        }

        public WindowLayout Layout
        {
            get;
            private set;
        }

        public string Name
        {
            get
            {
                return this.Layout.Name;
            }
        }

        public BitmapImage Image
        {
            get
            {
                double with = 150;
                double height = 120;
                double thicknes = 3;

                Bitmap erg = new Bitmap((int)with, (int)height);
                Graphics b = Graphics.FromImage(erg);
                b.FillRectangle(new SolidBrush(Color.FromArgb(this.Layout.Color[0], this.Layout.Color[1], this.Layout.Color[2], this.Layout.Color[3])), 0, 0, (float)with, (float)height);

                foreach (LayoutItem l in this.Layout.Items)
                {
                    Graphics g = Graphics.FromImage(erg);                    
                    g.DrawRectangle(new Pen(Color.Black, (float)thicknes), (float)LayoutHelper.GetActuelValue(l.MarginLeft, with), (float)LayoutHelper.GetActuelValue(l.MarginTop, height), (float)LayoutHelper.GetActuelValue(l.Width, with), (float)LayoutHelper.GetActuelValue(l.Height, height));
                    g.FillRectangle(Brushes.White, (float)LayoutHelper.GetActuelValue(l.MarginLeft, with) + (float)thicknes, (float)LayoutHelper.GetActuelValue(l.MarginTop, height) + (float)thicknes, (float)LayoutHelper.GetActuelValue(l.Width, with) - (float)thicknes * 2, (float)LayoutHelper.GetActuelValue(l.Height, height) - (float)thicknes * 2);
                    g.DrawString(l.Number.ToString(), new Font("Tahoma", 10), Brushes.Black, (float)LayoutHelper.GetActuelValue(l.MarginLeft, with) + (float)thicknes, (float)LayoutHelper.GetActuelValue(l.MarginTop, height) + (float)thicknes);
                }
                return ImageHandler.ImageHandler.BitmapToBitmapImage(erg);
            }
        }
    }
}

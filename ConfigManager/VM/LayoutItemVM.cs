using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Remote_Content_Show_Container;
using System.Windows.Controls;
using System.Windows;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ConfigManager
{
    public class LayoutItemVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private double heightParent;
        private double widthParent;

        public double HeightParent
        {
            get
            {
                return this.heightParent;
            }
            set
            {
                this.heightParent = value;
                this.OnPropertyChanged("Width");
                this.OnPropertyChanged("Height");
            }
        }

        public double WidthParent
        {
            get
            {
                return this.widthParent;
            }
            set
            {
                this.widthParent = value;
                this.OnPropertyChanged("Width");
                this.OnPropertyChanged("Height");
            }
        }

        public LayoutItem Item
        {
            get;
            set;
        }

        public double ItemHeight
        {
            set
            {
                this.Item.Height = value;
                this.OnPropertyChanged("Height");
                this.OnPropertyChanged();
            }
            get
            {
                return Math.Truncate(this.Item.Height * 100);
            }
        }

        public double ItemWidth
        {
            set
            {
                this.Item.Width = value;
                this.OnPropertyChanged("Width");
                this.OnPropertyChanged();
            }
            get
            {
                return Math.Truncate(this.Item.Width * 100);
            }
        }

        public double ItemMarginLeft
        {
            set
            {
                this.Item.MarginLeft = value;
                this.OnPropertyChanged("DrawBorder");
                this.OnPropertyChanged();
            }
            get
            {
                return Math.Truncate(this.Item.MarginLeft * 100);
            }
        }

        public double ItemMarginTop
        {
            set
            {
                this.Item.MarginTop = value;
                this.OnPropertyChanged("DrawBorder");
                this.OnPropertyChanged();
            }
            get
            {
                return Math.Truncate(this.Item.MarginTop * 100);
            }
        }

        public Thickness DrawBorder
        {            
            get
            {
                return new Thickness(LayoutHelper.GetActuelValue(this.Item.MarginLeft, this.WidthParent), LayoutHelper.GetActuelValue(this.Item.MarginTop, this.HeightParent), 0, 0);
            }
        }

        public double Height
        {
            get
            {
                return LayoutHelper.GetActuelValue(this.Item.Height, this.HeightParent);
            }
        }

        public double Width
        {
            get
            {
                return LayoutHelper.GetActuelValue(this.Item.Width, this.WidthParent);
            }
        }

        public int Number
        {
            get
            {
                return this.Item.Number;
            }
        }        

        public void SetSizeParent(double widthParent, double heightParent)
        {
            this.WidthParent = widthParent;
            this.HeightParent = heightParent;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}

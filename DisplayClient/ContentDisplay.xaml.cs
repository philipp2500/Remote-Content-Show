using Remote_Content_Show_Container;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace DisplayClient
{
    public partial class ContentDisplay : UserControl
    {
        private DisplayManager manager;

        public ContentDisplay()
        {
            this.InitializeComponent();
        }

        public DisplayManager DisplayManager
        {
            get
            {
                return this.manager;
            }

            set
            {
                this.manager = value;
                this.manager.OnImageDisplayRequested += Manager_OnImageDisplayRequested;

                this.manager.Start();
            }
        }

        private void Manager_OnImageDisplayRequested(Windows.UI.Xaml.Media.Imaging.BitmapImage image)
        {
            this.DisplayingImage.Source = image;
        }
    }
}

﻿using Remote_Content_Show_Container;
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

        private FrameworkElement currentDisplayControl;

        public ContentDisplay()
        {
            this.InitializeComponent();

            this.currentDisplayControl = null;
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
                this.manager.OnWebsiteDisplayRequested += Manager_OnWebsiteDisplayRequested;

                this.manager.Start();
            }
        }

        private void Manager_OnWebsiteDisplayRequested(Uri uri)
        {
            if (this.currentDisplayControl != null)
            {
                this.currentDisplayControl.Visibility = Visibility.Collapsed;
            }

            this.currentDisplayControl = this.DisplayingWebView;
            this.currentDisplayControl.Visibility = Visibility.Visible;

            this.DisplayingWebView.Navigate(uri);
        }

        private void Manager_OnImageDisplayRequested(Windows.UI.Xaml.Media.Imaging.BitmapImage image)
        {
            Windows.UI.Core.CoreWindow.GetForCurrentThread().Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (this.currentDisplayControl != null)
                {
                    this.currentDisplayControl.Visibility = Visibility.Collapsed;
                }

                this.currentDisplayControl = this.DisplayingImage;
                this.currentDisplayControl.Visibility = Visibility.Visible;

                this.DisplayingImage.Source = image;
            });
        }
    }
}
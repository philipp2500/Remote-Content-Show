using DisplayClient.Storage;
using Remote_Content_Show_Container;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
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
    public partial class ContentDisplay : UserControl, INotifyPropertyChanged
    {
        private ContentDisplayManager manager;

        private FrameworkElement currentDisplayControl;

        public event PropertyChangedEventHandler PropertyChanged;

        public ContentDisplay()
        {
            this.InitializeComponent();

            this.currentDisplayControl = null;
            this.DataContext = this;
            this.Loaded += ContentDisplay_Loaded;
            this.SizeChanged += ContentDisplay_SizeChanged;
        }

        private void ContentDisplay_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.manager != null)
            {
                this.manager.SetRenderSize(new Size(this.ActualWidth, this.ActualHeight));
            }
        }

        public ContentDisplayManager DisplayManager
        {
            get
            {
                return this.manager;
            }

            set
            {
                this.manager = value;
                this.manager.OnImageDisplayRequested += Manager_OnImageDisplayRequested;
                this.manager.OnVideoDisplayRequested += Manager_OnVideoDisplayRequested;
                this.manager.OnWebsiteDisplayRequested += Manager_OnWebsiteDisplayRequested;
                this.manager.OnDisplayAbortRequested += Manager_OnDisplayAbortRequested;
                this.manager.OnJobResultDisplayRequested += Manager_OnJobResultDisplayRequested;
            }
        }

        public ImageBrush ConfigImage
        {
            get
            {
                ImageBrush brush = new ImageBrush();

                return brush;
            }
        }

        private async void ContentDisplay_Loaded(object sender, RoutedEventArgs e)
        {
            this.rootGrid.Background = new ImageBrush() { ImageSource = await PersistenceManager.GetConfigurationImage() };
        }

        public async void UpdateConfigImage()
        {
            this.rootGrid.Background = new ImageBrush() { ImageSource = await PersistenceManager.GetConfigurationImage() };
        }

        private void Manager_OnJobResultDisplayRequested(Windows.UI.Xaml.Media.Imaging.BitmapImage image)
        {
            if (this.currentDisplayControl != this.DisplayingJobResult)
            {
                if (this.currentDisplayControl != null)
                {
                    this.currentDisplayControl.Visibility = Visibility.Collapsed;
                }

                this.currentDisplayControl = this.DisplayingJobResult;
                this.currentDisplayControl.Visibility = Visibility.Visible;

                Debug.WriteLine("--> Display job result");
            }

            this.DisplayingJobResult.Source = image;
            this.rootGrid.Background = null;
        }

        private void Manager_OnVideoDisplayRequested(Uri videoPath)
        {
            if (this.currentDisplayControl != this.DisplayingVideo)
            {
                if (this.currentDisplayControl != null)
                {
                    this.currentDisplayControl.Visibility = Visibility.Collapsed;
                }

                this.currentDisplayControl = this.DisplayingVideo;
                this.currentDisplayControl.Visibility = Visibility.Visible;

                Debug.WriteLine("--> Displaying video");

                this.rootGrid.Background = null;
            }

            this.DisplayingVideo.Source = videoPath;
            this.DisplayingVideo.Play();
        }

        private async void Manager_OnDisplayAbortRequested()
        {
            if (this.currentDisplayControl != null)
            {
                if (this.currentDisplayControl is Image)
                {
                    ((Image)this.currentDisplayControl).Source = null;
                }
                else if (this.currentDisplayControl is MediaElement)
                {
                    ((MediaElement)this.currentDisplayControl).Stop();
                    ((MediaElement)this.currentDisplayControl).AutoPlay = true;
                }
                else if (this.currentDisplayControl is WebView)
                {

                }

                this.currentDisplayControl.Visibility = Visibility.Collapsed;
                this.currentDisplayControl = null;
                this.rootGrid.Background = new ImageBrush() { ImageSource = await PersistenceManager.GetConfigurationImage() };
            }

            Debug.WriteLine("--> Aborting display");
        }

        private void Manager_OnWebsiteDisplayRequested(Uri uri)
        {
            if (this.currentDisplayControl != this.DisplayingWebView)
            {
                if (this.currentDisplayControl != null)
                {
                    this.currentDisplayControl.Visibility = Visibility.Collapsed;
                }

                this.currentDisplayControl = this.DisplayingWebView;
                this.currentDisplayControl.Visibility = Visibility.Visible;

                Debug.WriteLine("--> Displaying web view");
                this.rootGrid.Background = null;
            }

            this.DisplayingWebView.Navigate(uri);
        }

        private void Manager_OnImageDisplayRequested(Windows.UI.Xaml.Media.Imaging.BitmapImage image)
        {
            //Windows.UI.Core.CoreWindow.GetForCurrentThread().Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            //{
            if (this.currentDisplayControl != this.DisplayingImage)
            {
                if (this.currentDisplayControl != null)
                {
                    this.currentDisplayControl.Visibility = Visibility.Collapsed;
                }

                this.currentDisplayControl = this.DisplayingImage;
                this.currentDisplayControl.Visibility = Visibility.Visible;

                Debug.WriteLine("--> Displaying image");
                this.rootGrid.Background = null;
            }

            this.DisplayingImage.Source = image;

            //});
        }
    }
}

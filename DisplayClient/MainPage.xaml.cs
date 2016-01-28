using DisplayClient.Layout;
using DisplayClient.Log;
using DisplayClient.Storage;
using Remote_Content_Show_Container;
using Remote_Content_Show_Protocol;
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
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace DisplayClient
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Server adminListener;

        private Show currentShow;

        public MainPage()
        {
            this.InitializeComponent();

            string path = PersistenceManager.GetWriteablePath();

            Job_Configuration config = this.GetTestConfiguration();

            PersistenceManager.SaveJobConfiguration(config);

            /*this.currentShow = new Show(config, new Size(200, 200));

            this.LayoutContainer.Children.Add(this.currentShow.ContentWindow.GetRoot());

            this.currentShow.Start();*/

            //

            //EventsManager.Log(Job_EventType.Error, null, @"Resource C:\Temp\jellyfish1.jpg not found.");
            //EventsManager.Log(Job_EventType.Aborted, null, @"Displaying the resource C:\\Temp\\presi.pptx has been aborted by agent 192.168.2.101");

            //List<LoggedEvent> events = EventsManager.GetLoggedEvents();

            // 

            this.DataContext = this;

            this.Loaded += MainPage_Loaded;
            this.SizeChanged += MainPage_SizeChanged;
        }

        private Job_Configuration GetTestConfiguration()
        {
            Job_Configuration config = new Job_Configuration();
            config.Name = "testconfig1";
            config.Layout = CustomWindow.GetTestLayout(); // WindowLayout.DoubleWindowVertikalSplitted;
            config.Agents.Add(new Agent() { IP = "10.101.150.11" });
            config.Agents.Add(new Agent() { IP = "10.101.150.19" });

            List<Job> jobs1 = new List<Job>();
            jobs1.Add(new Job() { Duration = 15, OrderingNumber = 1, Resource = new FileResource() { Path = "http://www.w3schools.com/html/mov_bbb.mp4" } });
            jobs1.Add(new Job() { Duration = 15, OrderingNumber = 3, Resource = new FileResource() { Name = "test.pptx", Path = @"C:\Temp\test.pptx" } });
            jobs1.Add(new Job() { Duration = 15, OrderingNumber = 2, Resource = new WebResource() { Path = "http://www.google.at" } });

            List<Job> jobs2 = new List<Job>();
            jobs2.Add(new Job() { Duration = 15, OrderingNumber = 2, Resource = new WebResource() { Path = "http://www.fhwn.ac.at" } });
            //jobs2.Add(new Job() { Duration = 15, OrderingNumber = 1, Resource = new FileResource() { Name = "excel.xlsx", Path = @"C:\Temp\excel.xlsx" } });
            jobs2.Add(new Job() { Duration = 15, OrderingNumber = 3, Resource = new FileResource() { Path = "http://img.pr0gramm.com/2016/01/22/ef07ff94fd3236d1.jpg" } });
            jobs2.Add(new Job() { Duration = 15, OrderingNumber = 1, Resource = new FileResource() { Local = true, Path = "jellyfish2d.jpg" } });

            config.JobLists.Add(1, new JobWindowList() { Looping = true, WindowLayoutNumber = 1, Jobs = jobs1 });
            config.JobLists.Add(2, new JobWindowList() { Looping = true, WindowLayoutNumber = 2, Jobs = jobs2 });

            return config;
        }

        private void MainPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.currentShow != null)
            {
                this.currentShow.UpdateSize(new Size(this.rootGrid.ActualWidth, this.rootGrid.ActualHeight));
            }
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            // check image
            BitmapImage img = await PersistenceManager.GetConfigurationImage();

            if (img == null)
            {
                string noconfPath = Path.Combine(await PersistenceManager.GetAssetsPath(), "NoConf.PNG");

                img = new BitmapImage(new Uri(noconfPath));

                PersistenceManager.SaveConfigurationImage(PersistenceManager.GetFile(noconfPath));
            }

            this.rootGrid.Background = new ImageBrush() { ImageSource = img };

            // start listener
            this.adminListener = new Server(NetworkConfiguration.PortPi);
            this.adminListener.OnConnectionReceived += AdminListener_OnConnectionReceived;

            this.adminListener.Start();

            // check saved configuration
            Job_Configuration configuration = PersistenceManager.GetJobConfiguration();

            if (configuration != null)
            {
                if (this.currentShow != null)
                {
                    this.currentShow.Cancel();
                    this.currentShow = null;

                    this.LayoutContainer.Children.Clear();
                }

                this.currentShow = new Show(configuration, new Size(rootGrid.ActualWidth, rootGrid.ActualHeight));

                this.LayoutContainer.Children.Add(this.currentShow.ContentWindow.GetRoot());

                this.currentShow.Start();
            }
        }

        public  ImageBrush ConfigImage
        {
            get
            {
                ImageBrush brush = new ImageBrush();
                return brush;
            }
        }

        private void AdminListener_OnConnectionReceived(Windows.Networking.Sockets.StreamSocket socket)
        {
            ExternalAdmin admin = new ExternalAdmin(socket);

            admin.OnConfigurationImageReceived += Admin_OnConfigurationImageReceived;
            admin.OnJobConfigurationReceived += Admin_OnJobConfigurationReceived;
            admin.OnCancelRequestReceived += Admin_OnCancelRequestReceived;
            admin.OnEventRequestReceived += Admin_OnEventRequestReceived;

            admin.OnLocalFileListRequestReceived += Admin_OnLocalFileListRequestReceived;
            admin.OnLocalFileAddRequestReceived += Admin_OnLocalFileAddRequestReceived;
            admin.OnLocalFileRemoveRequestReceived += Admin_OnLocalFileRemoveRequestReceived;
        }

        private void Admin_OnLocalFileAddRequestReceived(byte[] content, string filename)
        {
            PersistenceManager.SaveFile(content, filename);
        }

        private void Admin_OnLocalFileRemoveRequestReceived(string filename)
        {
            PersistenceManager.DeleteFile(filename);
        }

        private void Admin_OnLocalFileListRequestReceived(ExternalAdmin sender)
        {
            List<FileItem> items = PersistenceManager.GetLocalFilesList().Select(x => new FileItem() { Name = x.Name }).ToList();
            RCS_FileList list = new RCS_FileList(items, RemoteType.Client);

            sender.SendLocalFilesList(list);
        }

        private async void Admin_OnConfigurationImageReceived(byte[] image)
        {
            PersistenceManager.SaveConfigurationImage(image);

            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                this.rootGrid.Background = new ImageBrush() { ImageSource = await PersistenceManager.GetConfigurationImage() };
                
                if (this.currentShow != null)
                {
                    this.currentShow.UpdateConfigImage();
                }
            });
        }

        private async void Admin_OnJobConfigurationReceived(Job_Configuration configuration)
        {
            PersistenceManager.SaveJobConfiguration(configuration);

            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (this.currentShow != null)
                {
                    this.currentShow.Cancel();
                    this.currentShow = null;

                    this.LayoutContainer.Children.Clear();                    
                }

                this.currentShow = new Show(configuration, new Size(rootGrid.ActualWidth, rootGrid.ActualHeight));

                this.LayoutContainer.Children.Add(this.currentShow.ContentWindow.GetRoot());

                this.currentShow.Start();
            });
        }

        private async void Admin_OnCancelRequestReceived(Guid jobID, CancelJobReason reason)
        {
            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (this.currentShow != null)
                {
                    this.currentShow.Cancel();
                    this.currentShow = null;

                    this.LayoutContainer.Children.Clear();

                    EventsManager.Log(Job_EventType.Aborted, null, "Job " + jobID.ToString() + " has been canceled. Reason: " + reason.ToString());
                }
            });
        }

        private async void Admin_OnEventRequestReceived(ExternalAdmin sender)
        {
            List<LoggedEvent> events = await EventsManager.GetLoggedEvents();

            List<Event> convEvents = events.Select(x => new Event() { Type = x.Type, Description = x.Description, NameOfConcernedJob = x.ConcernedJob.Name, Time = x.Time }).ToList();

            sender.SendEventsList(new Event_List() { List = convEvents });
        }

        private void StartConfigListenerButton_Click(object sender, RoutedEventArgs e)
        {
            this.adminListener.Start();
        }
    }
}

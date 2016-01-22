using DisplayClient.Layout;
using DisplayClient.Log;
using DisplayClient.Storage;
using Remote_Content_Show_Container;
using Remote_Content_Show_Protocol;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        private Server adminListener;

        public event PropertyChangedEventHandler PropertyChanged;

        public MainPage()
        {
            this.InitializeComponent();

            string path = PersistenceManager.GetWriteablePath();

            ContentDisplay display1 = new ContentDisplay();
            //display1.Width = 800;
            //display1.Height = 800;

            display1.DisplayManager = new ContentDisplayManager(new JobWindowList(), new List<Agent>()) { DisplayDispatcher = display1.Dispatcher };

            this.LayoutContainer.Children.Add(display1);

            EventsManager.ClearLog();

            EventsManager.Log(Job_EventType.Error, null, @"Resource C:\Temp\jellyfish1.jpg not found.");
            EventsManager.Log(Job_EventType.Aborted, null, @"Displaying the resource C:\\Temp\\presi.pptx has been aborted by agent 192.168.2.101");

            List<LoggedEvent> events = EventsManager.GetLoggedEvents();

            // 
            this.adminListener = new Server(NetworkConfiguration.PortPi);
            this.adminListener.OnConnectionReceived += AdminListener_OnConnectionReceived;

            this.DataContext = this;
        }

        public ImageBrush ConfigImage
        {
            get
            {
                ImageBrush brush = new ImageBrush();
                brush.ImageSource = PersistenceManager.GetConfigurationImage();

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
        }

        private void Admin_OnConfigurationImageReceived(byte[] image)
        {
            PersistenceManager.SaveConfigurationImage(image);

            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs("ConfigImage"));
            }
        }

        private void Admin_OnJobConfigurationReceived(Job_Configuration configuration)
        {

        }

        private void Admin_OnCancelRequestReceived(Guid jobID, CancelJobReason reason)
        {
        }

        private void Admin_OnEventRequestReceived(ExternalAdmin sender)
        {
            List<LoggedEvent> events = EventsManager.GetLoggedEvents();

            List<Event> convEvents = events.Select(x => new Event() { Type = x.Type, Description = x.Description, NameOfConcernedJob = "philip", Time = x.Time }).ToList();

            sender.SendEventsList(new Event_List() { List = convEvents });
        }

        private void StartConfigListenerButton_Click(object sender, RoutedEventArgs e)
        {
            this.adminListener.Start();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

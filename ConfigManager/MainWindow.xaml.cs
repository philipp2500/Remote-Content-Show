using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ConfigManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void NewJob_Click(object sender, RoutedEventArgs e)
        {
            NewJob newJobWindow = new NewJob();
            newJobWindow.ShowDialog();
        }

        private void SendJob_Click(object sender, RoutedEventArgs e)
        {
            SendJob sj = new SendJob();
            sj.ShowDialog();
        }

        private void ChangePicture_Click(object sender, RoutedEventArgs e)
        {
            CreateConfriguration cc = new CreateConfriguration();
            cc.ShowDialog();
        }

        private void GetEventList_Click(object sender, RoutedEventArgs e)
        {
            EventList ev = new EventList();
            ev.ShowDialog();
        }

        private void FileManager_Click(object sender, RoutedEventArgs e)
        {
            FileManeger fm = new FileManeger();
            fm.ShowDialog();
        }

        private void CreateLyout_Click(object sender, RoutedEventArgs e)
        {
            LayoutDesigner ld = new LayoutDesigner();
            ld.ShowDialog();
        }
    }
}

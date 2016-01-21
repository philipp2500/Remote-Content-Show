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
using Remote_Content_Show_Container;
using System.Net;
using Remote_Content_Show_Protocol;
using ImageHandler;

namespace ConfigManager
{
    /// <summary>
    /// Interaction logic for AgentProcessOverview.xaml
    /// </summary>
    public partial class AgentProcessOverview : UserControl
    {
        public AgentProcessOverview(RCS_Process_List_Response processResponce, IPAddress ip)
        {
            InitializeComponent();

            this.AgentProcessOverviewName.Content = processResponce.ClientName;
            this.AgentProcessOverviewIP.Content = ip.ToString();

            foreach (ProcessDescription pd in processResponce.ProcesseList.Processes)
            {
                Image i = new Image();
                i.Source = ImageHandler.ImageHandler.BytesToImage(pd.WindowPicture);
                i.Height = 100;
                i.Width = 100;
                i.Margin = new Thickness(2);
                this.AgentProcessOverviewStackPanel.Children.Add(i);
            }
        }
    }
}

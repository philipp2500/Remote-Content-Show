using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace DisplayClient.Layout
{
    public interface IContentWindow
    {
        Grid GetRoot();

        Grid[] GetAllDisplays();
    }
}

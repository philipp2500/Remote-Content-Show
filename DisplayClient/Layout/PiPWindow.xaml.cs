﻿using System;
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

namespace DisplayClient.Layout
{
    public sealed partial class PiPWindow : UserControl, IContentWindow
    {
        public PiPWindow()
        {
            this.InitializeComponent();
        }

        public Grid[] GetAllDisplays()
        {
            List<Grid> array = new List<Grid>();

            array.Add(this.Display1);
            array.Add(this.Display2);
            array.Add(this.Display3);
            array.Add(this.Display4);
            array.Add(this.Display5);

            return array.ToArray();
        }

        public Grid GetRoot()
        {
            return this.mainroot;
        }
    }
}

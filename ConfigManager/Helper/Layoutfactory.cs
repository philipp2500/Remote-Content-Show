﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Remote_Content_Show_Container;

namespace ConfigManager
{
    public static class Layoutfactory
    {
        public static ObservableCollection<WindowLayoutVM> LoadBasicLayouts()
        {
            ObservableCollection<WindowLayoutVM> erg = new ObservableCollection<WindowLayoutVM>();

            WindowLayout wl1 = new WindowLayout();
            wl1.Name = "Single Window";
            wl1.Items.Add(new LayoutItem(0, 0, 1, 1, 1));
            wl1.Color = new byte[] { 255, 0, 0, 0 };
            erg.Add(new WindowLayoutVM(wl1));

            WindowLayout wl2 = new WindowLayout();
            wl2.Name = "Double Window vertical Split";
            wl2.Items.Add(new LayoutItem(0, 0, 0.5, 1, 1));
            wl2.Items.Add(new LayoutItem(0.5, 0, 0.5, 1, 2));
            wl2.Color = new byte[] { 255, 0, 0, 0 };
            erg.Add(new WindowLayoutVM(wl2));

            WindowLayout wl3 = new WindowLayout();
            wl3.Name = "Double Window horizontal  Split";
            wl3.Items.Add(new LayoutItem(0, 0, 1, 0.5, 1));
            wl3.Items.Add(new LayoutItem(0, 0.5, 1, 0.5, 2));
            wl3.Color = new byte[] { 255, 0, 0, 0 };
            erg.Add(new WindowLayoutVM(wl3));

            WindowLayout wl4 = new WindowLayout();
            wl4.Name = "Window in the Middel";
            wl4.Items.Add(new LayoutItem(0.25, 0.25, 0.5, 0.5, 1));
            wl4.Color = new byte[] { 255, 0, 0, 0 };
            erg.Add(new WindowLayoutVM(wl4));

            WindowLayout wl5 = new WindowLayout();
            wl5.Name = "Four Fields";
            wl5.Items.Add(new LayoutItem(0, 0, 0.5, 0.5, 1));
            wl5.Items.Add(new LayoutItem(0.5, 0, 0.5, 0.5, 2));
            wl5.Items.Add(new LayoutItem(0, 0.5, 0.5, 0.5, 3));
            wl5.Items.Add(new LayoutItem(0.5, 0.5, 0.5, 0.5, 4));
            wl5.Color = new byte[] { 255, 0, 0, 0 };
            erg.Add(new WindowLayoutVM(wl5));

            WindowLayout wl6 = new WindowLayout();
            wl6.Name = "Complex Layout";
            wl6.Items.Add(new LayoutItem(0.1, 0.1, 0.2, 0.35, 1));
            wl6.Items.Add(new LayoutItem(0.1, 0.55, 0.2, 0.35, 2));
            wl6.Items.Add(new LayoutItem(0.4, 0.1, 0.2, 0.8, 3));
            wl6.Items.Add(new LayoutItem(0.7, 0.1, 0.2, 0.8, 4));
            wl6.Color = new byte[] { 255, 0, 0, 0 };
            erg.Add(new WindowLayoutVM(wl6));


            return erg;
        }
    }
}

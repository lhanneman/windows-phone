using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Reflection;

namespace BeerTrends.Pages.Settings
{
    public partial class About : PhoneApplicationPage
    {
        public About()
        {
            InitializeComponent();
            ContentPanel.Loaded += ContentPanel_Loaded;
        }

        void ContentPanel_Loaded(object sender, RoutedEventArgs e)
        {
            lblVersion.Text = "Version: " + GetVersionNumber();
        }

        private string GetVersionNumber()
        {
            var asm = Assembly.GetExecutingAssembly();
            var parts = asm.FullName.Split(',');
            return parts[1].Split('=')[1];
        }
    }
}
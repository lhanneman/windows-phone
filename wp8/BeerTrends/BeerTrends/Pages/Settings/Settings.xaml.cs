using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using BeerTrends.Helpers;

namespace BeerTrends.Pages.Settings
{
    public partial class Settings : PhoneApplicationPage
    {

        public Settings()
        {
            InitializeComponent();
            btnAbout.Tap += btnAbout_Tap;
            ContentPanel.Loaded += ContentPanel_Loaded;
        }

        void ContentPanel_Loaded(object sender, RoutedEventArgs e)
        {

        }

        void btnAbout_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/Settings/About.xaml", UriKind.Relative));
        }

        // for settings:
        //var settings = IsolatedStorageSettings.ApplicationSettings;
        //settings["CheckBox1Checked"] = true;
        //settings.Save();
        //http://stackoverflow.com/questions/21318037/how-to-save-the-value-of-the-checkbox-in-a-windows-phone-application
    }
}
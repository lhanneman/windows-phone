using System;
using System.IO.IsolatedStorage;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using MapMyElevation.Helpers;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Windows.Devices.Geolocation;
using Windows.System;
using System.Linq;

namespace MapMyElevation
{
    public partial class MainPage : PhoneApplicationPage
    {

        public MainPage()
        {
            InitializeComponent();
            btnQuickCheck.Tap += btnQuickCheck_Tap;
            btnOptions.Tap += btnOptions_Tap;
            btnMapRoute.Tap += btnMapRoute_Tap;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            // make sure user has agreed to let this app use their location, and that
            // their location settings are turned on!
            bool locationSettingsVerified = UserSettings.CheckLocationConsentAndSettings();

            // CHECK FOR PREVIOUS EXCEPTION:
            ErrorReporting.CheckForPreviousException();

            btnMapRoute.IsEnabled = locationSettingsVerified;
            btnQuickCheck.IsEnabled = locationSettingsVerified;

            // reset:
            App.Geolocator = null;

            
        }

        /// <summary>
        /// WHEN THE BACK BUTTON IS PRESSED, EXIT GAME
        /// </summary>
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            while (NavigationService.BackStack.Any())
                NavigationService.RemoveBackEntry();
        }

        void btnMapRoute_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            // start = true will tell the page to start navigation immediately:
            NavigationService.Navigate(new Uri("/Pages/MapARoute.xaml?start=true", UriKind.Relative));
        }

        void btnOptions_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/Options.xaml", UriKind.Relative));
        }

        void btnQuickCheck_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/QuickCheck.xaml", UriKind.Relative));
        }

        private void MenuAd_ErrorOccurred(object sender, Microsoft.Advertising.AdErrorEventArgs e)
        {
            string err = e.Error.ToString();
        }


    }
}
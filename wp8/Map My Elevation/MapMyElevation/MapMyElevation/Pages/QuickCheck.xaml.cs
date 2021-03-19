using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using MapMyElevation.Resources;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Windows.Devices.Geolocation;
using MapMyElevation.Helpers;
using System.Device.Location;
using System.Windows.Threading;

namespace MapMyElevation.Pages
{
    public partial class QuickCheck : PhoneApplicationPage
    {

        int tryCount = 0;
        DispatcherTimer timer = new DispatcherTimer();
        
        public QuickCheck()
        {
            InitializeComponent();
            btnRefresh.Tap += btnRefresh_Tap;
        }

        void btnRefresh_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            HideInfo();
            ShowLoading();
            InitializeGeolocator();
            
        }

        protected override void OnRemovedFromJournal(System.Windows.Navigation.JournalEntryRemovedEventArgs e)
        {
            App.Geolocator = null;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {

            base.OnNavigatedTo(e);

            //if (NavigationContext.QueryString.Keys.Contains("fromToast"))
            //{
            //    if (NavigationContext.QueryString["fromToast"] == "true")
            //    {
            //        // came from toast, don't do anything!
            //        return;
            //    }
            //}

            if (App.Geolocator == null)
            {
                App.Geolocator = new Geolocator();
                App.Geolocator.DesiredAccuracy = PositionAccuracy.High;
                App.Geolocator.MovementThreshold = 10; // The units are meters.
                App.Geolocator.PositionChanged += PositionChanged;
            }

            if (App.Geolocator.LocationStatus == PositionStatus.Disabled)
            {
                // Location is turned off
                if (MessageBox.Show(AppResources.AppRequiresLocation, AppResources.LocationRequired, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    UserSettings.LaunchLocationSettingsApp();
                else
                    return;
            }

            // start the timer:
            RestartTimer();
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (timer != null)
            {
                timer.Stop();
                timer = null;
            }
        }

        void InitializeGeolocator()
        {
            if (App.Geolocator == null)
            {
                App.Geolocator = new Geolocator();
                App.Geolocator.DesiredAccuracy = PositionAccuracy.High;
                App.Geolocator.MovementThreshold = 10; // The units are meters.
                App.Geolocator.PositionChanged += PositionChanged;
            }
        }

        void PositionChanged(Geolocator sender, PositionChangedEventArgs e)
        {

            if (!App.RunningInBackground)
            {
                Dispatcher.BeginInvoke(() =>
                {
                    // try getting altitude 3 times before telling them 0:
                    if (e.Position.Coordinate.Altitude == 0 && tryCount < 3)
                    {
                        tryCount += 1;
                        return;
                    }

                    App.Geolocator.PositionChanged -= PositionChanged;
                    App.Geolocator = null;

                    UserSettings settings = UserSettings.GetUserSettings();

                    lblLatitude.Text = e.Position.Coordinate.Latitude.ToString("0.00");
                    lblLongitude.Text = e.Position.Coordinate.Longitude.ToString("0.00");
                    lblElevation.Text = ValueFormatter.GetElevationFormatted(e.Position.Coordinate.Altitude, settings.Options.ElevationUnit);

                    ShowInfo();
                    HideLoading();
                });
            }
            //else
            //{
            //    Microsoft.Phone.Shell.ShellToast toast = new Microsoft.Phone.Shell.ShellToast();
            //    toast.Content = "We'll continue to run in the background!";
            //    toast.Title = AppResources.ApplicationTitle;
            //    toast.NavigationUri = new Uri("/Pages/QuickCheck.xaml?fromToast=true", UriKind.Relative);
            //    toast.Show();
            //}
        }

        void RestartTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 20);
            timer.Tick += timer_Tick;
            timer.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            if (MessageBox.Show("It seems to be taking a little longer than normal to look up your location. Tap OK to try again.", "Sorry", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                App.Geolocator = null;
                InitializeGeolocator();
            }

        }

        #region show/hide functionality

        void ShowInfo()
        {
            lblLatitude.Visibility = System.Windows.Visibility.Visible;
            lblLatitudeLabel.Visibility = System.Windows.Visibility.Visible;
            lblLongitude.Visibility = System.Windows.Visibility.Visible;
            lblLongitudeLabel.Visibility = System.Windows.Visibility.Visible;
            lblElevation.Visibility = System.Windows.Visibility.Visible;
            lblElevationLabel.Visibility = System.Windows.Visibility.Visible;
            btnRefresh.Visibility = System.Windows.Visibility.Visible;
        }

        void HideInfo()
        {
            lblLatitude.Visibility = System.Windows.Visibility.Collapsed;
            lblLatitudeLabel.Visibility = System.Windows.Visibility.Collapsed;
            lblLongitude.Visibility = System.Windows.Visibility.Collapsed;
            lblLongitudeLabel.Visibility = System.Windows.Visibility.Collapsed;
            lblElevation.Visibility = System.Windows.Visibility.Collapsed;
            lblElevationLabel.Visibility = System.Windows.Visibility.Collapsed;
            btnRefresh.Visibility = System.Windows.Visibility.Collapsed;
        }

        void HideLoading()
        {
            lblLoading.Visibility = System.Windows.Visibility.Collapsed;
            pbLoading.Visibility = System.Windows.Visibility.Collapsed;
        }

        void ShowLoading()
        {
            lblLoading.Visibility = System.Windows.Visibility.Visible;
            pbLoading.Visibility = System.Windows.Visibility.Visible;
        }

        #endregion

    }
}
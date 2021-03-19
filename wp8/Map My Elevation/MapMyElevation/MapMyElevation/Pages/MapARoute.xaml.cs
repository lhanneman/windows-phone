using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Windows.Devices.Geolocation;
using System.Device.Location;
using Microsoft.Phone.Maps.Toolkit;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Maps.Services;
using System.Windows.Threading;
using System.Windows.Shapes;
using System.Windows.Media;
using MapMyElevation.Helpers;
using MapMyElevation.Resources;

namespace MapMyElevation.Pages
{
    public partial class MapARoute : PhoneApplicationPage
    {
        // globals:
        double totalDistanceInMeters;
        DispatcherTimer timer;
        TimeSpan totalTime;
        int tryCount = 0;
     //   int backgroundPositionChangeCount = 0;
        
      //  bool showedToast = false;

        // pushpin stuff:
        Pushpin pushpin;
        MapOverlay ppOverlay;
        MapLayer ppLayer;
        UserSettings settings = null;
        
        public MapARoute()
        {
            InitializeComponent();

            // handlers:
            btnStartStop.Tap += btnStartStop_Tap;
        }

        protected override void OnRemovedFromJournal(System.Windows.Navigation.JournalEntryRemovedEventArgs e)
        {
            App.Geolocator = null;
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/Menu.xaml", UriKind.Relative));
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {

            base.OnNavigatedTo(e);
            if (e.NavigationMode == NavigationMode.Reset) return;

            if (App.Geolocator == null)
            {
                App.Geolocator = new Geolocator();
                App.Geolocator.DesiredAccuracy = PositionAccuracy.High;
                App.Geolocator.MovementThreshold = 10; // The units are meters
            }

            if (App.Geolocator.LocationStatus == PositionStatus.Disabled)
            {
                if (MessageBox.Show(AppResources.AppRequiresLocation, AppResources.LocationRequired, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    UserSettings.LaunchLocationSettingsApp();
                else
                    return;
            }

            bool bStartImmediately = false;

           
            if (NavigationContext.QueryString.Keys.Contains("start"))
            {
                if (NavigationContext.QueryString["start"] == "true")
                {
                    bStartImmediately = true;
                    App.listChartData = new List<ChartData>();
                    NavigationContext.QueryString.Remove("start");
                }
            }
            else if (NavigationContext.QueryString.Keys.Contains("summary"))
            {
                btnStartStop.Content = AppResources.StartNavigation;
                btnStartStop.Visibility = System.Windows.Visibility.Visible;
                chkKeepCentered.Visibility = System.Windows.Visibility.Visible;
                NavigationContext.QueryString.Remove("start");
                return;
            }
            else
            {
                // prob came from from pressing start:
                return;
            }

            if (bStartImmediately)
            {
                lblStatus.Text = AppResources.Initializing;
                StartNavigation();
            }
        }

        void AddPushpin(GeoCoordinate location)
        {
            // add the pushpin:
            pushpin = new Pushpin();
            pushpin.Content = AppResources.CurrentPosition;

            ppOverlay = new MapOverlay();
            ppOverlay.Content = pushpin;
            ppOverlay.GeoCoordinate = location;
            ppOverlay.PositionOrigin = new Point(0, 1);

            ppLayer = new MapLayer();
            ppLayer.Add(ppOverlay);

            MainMap.Layers.Add(ppLayer);

        }

        void RemovePushpin()
        {
            MainMap.Layers.Remove(ppLayer);
            pushpin = null;
            ppOverlay = null;
        }

        void btnStartStop_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Button thisButton = (Button)sender;
            if (thisButton.Content.ToString() == AppResources.StartNavigation)
            {
                // pressed start:

                // toggle text for "stop":
                thisButton.Content = AppResources.StopNavigation;
                chkKeepCentered.Visibility = System.Windows.Visibility.Visible;

                StartNavigation();
            }
            else
            {
                // pressed stop:
                StopNavigation();
            }
        }

        private void StartNavigation()
        {

            lblStatus.Text = AppResources.Initializing;

            // clear any map elements that may be here from before:
            MainMap.MapElements.Clear();
            MainMap.Layers.Clear();
            
            // look up the user's settings:
            settings = UserSettings.GetUserSettings();

            // show the loading progressbar:
            pbLoading.Visibility = Visibility.Visible;

            // clear out our summary in state:
            if (PhoneApplicationService.Current.State.Keys.Contains("summary")) PhoneApplicationService.Current.State["summary"] = null;

            // handle when the position changes:
            App.Geolocator.PositionChanged += PositionChanged;
        }

        private void StopNavigation()
        {

            App.Geolocator.PositionChanged -= PositionChanged;
            App.Geolocator = null;

            Helpers.SummaryData summary = new Helpers.SummaryData(totalDistanceInMeters, totalTime, App.listChartData);

            // save into phone state:
            PhoneApplicationService.Current.State["summary"] = summary;

            //btnSummary.Visibility = System.Windows.Visibility.Visible;
            NavigationService.Navigate(new Uri("/Pages/Summary.xaml", UriKind.Relative));

            timer = null;
            App.listChartData = null;
        }

        void btnSummary_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/Summary.xaml", UriKind.Relative));
        }

        void PositionChanged(Geolocator sender, PositionChangedEventArgs e)
        {

            // try getting altitude 3 times before telling them 0:
            if (e.Position.Coordinate.Altitude == 0 && tryCount < 3)
            {
                tryCount += 1;
                return;
            }

            if (!App.RunningInBackground)
            {
                Dispatcher.BeginInvoke(() =>
                {

                    // hide loading if needed:
                    if (pbLoading.Visibility == System.Windows.Visibility.Visible) pbLoading.Visibility = System.Windows.Visibility.Collapsed;

                    GeoCoordinate location = new GeoCoordinate(e.Position.Coordinate.Latitude, e.Position.Coordinate.Longitude);
                    if (chkKeepCentered.IsChecked == true) MainMap.Center = location;
                    MainMap.ZoomLevel = 15;

                    if (App.listChartData == null || App.listChartData.Count() == 0)
                    {

                        btnStartStop.Visibility = System.Windows.Visibility.Visible;
                        chkKeepCentered.Visibility = System.Windows.Visibility.Visible;

                        totalDistanceInMeters = 0; // initialize
                        timer = new DispatcherTimer();
                        timer.Interval = TimeSpan.FromSeconds(1);
                        timer.Tick += timer_Tick;
                        totalTime = new TimeSpan();
                        timer.Start();

                        // update status:
                        lblStatus.Text = AppResources.FoundPosition;

                        // center the map on our current location:



                        // add a pushpin with the current location:
                        AddPushpin(location);

                        // first time through:
                        App.listChartData = new List<ChartData>();
                        App.listChartData.Add(new ChartData(e.Position.Coordinate, 0, settings.Options.DistanceUnit));
                        App.LastCoordinate = e.Position.Coordinate;

                        return;

                    }
                    else
                    {

                        GeoCoordinate g1 = new GeoCoordinate(App.LastCoordinate.Latitude, App.LastCoordinate.Longitude);
                        GeoCoordinate g2 = new GeoCoordinate(e.Position.Coordinate.Latitude, e.Position.Coordinate.Longitude);

                        double distanceToNextCoordinate = (g1.GetDistanceTo(g2));

                        g1 = null;
                        g2 = null;

                        if (distanceToNextCoordinate > 1)
                        {
                            totalDistanceInMeters += distanceToNextCoordinate;
                            App.listChartData.Add(new ChartData(e.Position.Coordinate, totalDistanceInMeters, settings.Options.DistanceUnit));
                            DrawLine(e.Position.Coordinate);
                            App.LastCoordinate = e.Position.Coordinate;

                        }
                    }

                    // update status with current elevation:
                    lblStatus.Text = AppResources.CurrentElevationColon + ValueFormatter.GetElevationFormatted(e.Position.Coordinate.Altitude, settings.Options.ElevationUnit);
                });
            }
            else
            {

                Dispatcher.BeginInvoke(() =>
                {

                    // initialize temp data if we need to:
                    if (App.tempChartData == null) App.tempChartData = new List<ChartData>();


                    // initialize timer if we need to:
                    if (timer == null)
                    {
                        timer = new DispatcherTimer();
                        timer.Interval = TimeSpan.FromSeconds(1);
                        timer.Tick += timer_Tick;
                        totalTime = new TimeSpan();
                        timer.Start();
                    }

                    // if we don't have a previous coordinate, set it and continue:
                    if (App.LastCoordinate != null)
                    {
                        App.LastCoordinate = e.Position.Coordinate;
                        App.tempChartData.Add(new ChartData(e.Position.Coordinate, totalDistanceInMeters, settings.Options.DistanceUnit));
                    }
                    else
                    {
                        // if we have
                        GeoCoordinate g1 = new GeoCoordinate(App.LastCoordinate.Latitude, App.LastCoordinate.Longitude);
                        GeoCoordinate g2 = new GeoCoordinate(e.Position.Coordinate.Latitude, e.Position.Coordinate.Longitude);

                        double distanceToNextCoordinate = (g1.GetDistanceTo(g2));

                        g1 = null;
                        g2 = null;

                        if (distanceToNextCoordinate > 1)
                        {
                            totalDistanceInMeters += distanceToNextCoordinate;
                            App.tempChartData.Add(new ChartData(e.Position.Coordinate, totalDistanceInMeters, settings.Options.DistanceUnit));
                            App.LastCoordinate = e.Position.Coordinate;
                        }
                    }

                });
            }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            totalTime = totalTime.Add(TimeSpan.FromSeconds(1));
        }
     
        private void DrawLine(Geocoordinate pos)
        {
            
            // first check for any lines we need to draw from coordinates gathered while app
            // was running in the background:
            if (App.tempChartData != null && App.tempChartData.Count() > 0)
            {
                GeoCoordinate tempLast = new GeoCoordinate(App.tempChartData[0].Coordinate.Latitude, App.tempChartData[0].Coordinate.Longitude);
                for(int i = 1; i < App.tempChartData.Count(); i++)
                {
                    GeoCoordinate thisCoord = new GeoCoordinate(App.tempChartData[i].Coordinate.Latitude, App.tempChartData[i].Coordinate.Longitude);

                    MapPolyline tempLine = new MapPolyline();
                    tempLine.StrokeColor = Colors.Blue;
                    tempLine.StrokeThickness = 15;
                    tempLine.Path.Add(tempLast);
                    tempLine.Path.Add(thisCoord);
                    MainMap.MapElements.Add(tempLine);
                   
                    tempLast = thisCoord;
                }

                // reset:
                App.tempChartData = null;

            }
            
            MapPolyline line = new MapPolyline();
            line.StrokeColor = Colors.Blue;
            line.StrokeThickness = 15;
            line.Path.Add(new GeoCoordinate(App.LastCoordinate.Latitude, App.LastCoordinate.Longitude));
            line.Path.Add(new GeoCoordinate(pos.Latitude, pos.Longitude));
            MainMap.MapElements.Add(line);
        }

    }
}
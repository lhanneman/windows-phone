using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using MapMyElevation.Helpers;
using System.Collections.ObjectModel;
using System.Device.Location;
using MapMyElevation.Resources;
using System.Windows.Media;

namespace MapMyElevation.Pages
{
    public partial class Summary : PhoneApplicationPage
    {
        public Summary()
        {
            InitializeComponent();
        }

        public ObservableCollection<ChartData> Data { get; set; }
        public SolidColorBrush ChartBrush { get; set; }
        public String LineGraphTitle { get; set; }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            ChartBrush = new SolidColorBrush((Color)Application.Current.Resources["PhoneAccentColor"]);
            //LineGraphTitle = new String("Title");

            // pull the summary object out of phone application state:
            if (PhoneApplicationService.Current.State.Keys.Contains("summary"))
            {
                Helpers.SummaryData summary = (Helpers.SummaryData)PhoneApplicationService.Current.State["summary"];

                UserSettings settings = UserSettings.GetUserSettings();

                // FIRST PIVOT  (SUMMARY):
                lblTotalDistance.Text = ValueFormatter.GetDistanceFormatted(summary.TotalDistanceInMeters, settings.Options.DistanceUnit, false);
                lblTotalTime.Text = GetTimeString(summary.TotalTime);
                lblHighestElevation.Text = ValueFormatter.GetElevationFormatted(summary.HighestElevation, settings.Options.ElevationUnit);
                lblLowestElevation.Text = ValueFormatter.GetElevationFormatted(summary.LowestElevation, settings.Options.ElevationUnit);
                lblAverageElevation.Text = ValueFormatter.GetElevationFormatted(summary.AverageElevation, settings.Options.ElevationUnit);

                


                //// APPLY THE TITLE BASED ON THE ELEVATION UNIT:
                //switch (settings.Options.ElevationUnit)
                //{
                //    case (int)Helpers.Enumerations.ElevationUnit.Kilometers:
                //        ElevationLineGraph.Title = AppResources.ElevationInKilos;
                //        break;
                //    case (int)Helpers.Enumerations.ElevationUnit.Meters:
                //        ElevationLineGraph.Title = AppResources.ElevationInMeters;
                //        break;
                //    case (int)Helpers.Enumerations.ElevationUnit.Miles:
                //        ElevationLineGraph.Title = AppResources.ElevationInMiles;
                //        break;
                //    default:
                //        break;
                //}

                // SECOND PIVOT (ELEVATION LINE GRAPH):
                Data = new ObservableCollection<ChartData>();
                foreach (ChartData cd in summary.ChartData)
                {
                    Data.Add(cd);
                }
            }
        }

        private string GetTimeString(TimeSpan ts)
        {
            string result = "";
            if (ts != null)
            {
                //if (ts.Days > 0) result += ts.Days + " Days, ";
                //if (ts.Hours > 0) result += ts.Hours + " Hours, ";
                //if (ts.Minutes > 0) result += ts.Minutes + " Minutes, ";
                //if (ts.Seconds > 0) result += ts.Seconds + " Seconds";
                if (ts.Hours > 0)
                {
                    result = ts.Hours.ToString("00") + ":";
                }
                result += ts.Minutes.ToString("00") + ":" + ts.Seconds.ToString("00");
            }
            return result;
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/MapARoute.xaml?summary=true", UriKind.Relative));
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = this;
        }

        
    }
}
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
using MapMyElevation.Resources;

namespace MapMyElevation.View
{
    public partial class OptionsView : UserControl
    {
        public OptionsView()
        {
            InitializeComponent();

            //lpElevationUnit.ItemsSource = Enumerations.GetNames<Enumerations.ElevationUnit>();
            lpElevationUnit.ItemsSource = Enumerations.GetElevationUnits();
            lpDistanceUnit.ItemsSource = Enumerations.GetDistanceUnits();
            btnAbout.Tap += btnAbout_Tap;

        }

        void btnAbout_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/Pages/About.xaml", UriKind.Relative));
        }
    }
}

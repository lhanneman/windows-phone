using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using MapMyElevation.Helpers;
using MapMyElevation.Resources;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

namespace MapMyElevation.Pages
{
    public partial class About : PhoneApplicationPage
    {
        public About()
        {
            InitializeComponent();
            btnContact.Tap += btnContact_Tap;
            btnRate.Tap += btnRate_Tap;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            // get version info:
            lblVersionNumber.Text = AppResources.Version + ": " + VersionNumber();
        }


        void btnRate_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            MarketplaceReviewTask mrt = new MarketplaceReviewTask();
            mrt.Show();
        }

        void btnContact_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ErrorReporting.SendFeedback();
        }

        private string VersionNumber()
        {
            var info = new AssemblyName(Assembly.GetExecutingAssembly().FullName);
            string version = info.Version.Major + "." + info.Version.MajorRevision + "." + info.Version.Minor + "." + info.Version.MinorRevision;
            return version;
        }
    }
}
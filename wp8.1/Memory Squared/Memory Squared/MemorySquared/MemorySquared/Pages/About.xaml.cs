using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using MemorySquared.Resources;

namespace MemorySquared
{
    public partial class About : PhoneApplicationPage
    {

        public About()
        {
            InitializeComponent();

            // IF WE AREN'T DISPLAYING ADS, DON'T SHOW THE AD ROW:
            if (!Globals.bDisplayAds) AdRow.Height = new GridLength(0);
    
            // SET UP HANDLERS:
            AboutGrid.Loaded += LoadAbout;
            btnContact.Tap += SendFeedback;
            btnRate.Tap += RateApp;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // RESET DEACTIVATED SWITCH UPON NAVIGATING HERE (IF NEEDED):
            if (Globals.bDeactivated) Globals.bDeactivated = false;
        }

        void LoadAbout(Object sender, EventArgs e)
        {

            // SETUP ADVERTISEMENT:
            if (Globals.bDisplayAds) Advertising.GetAdForGrid(LayoutRoot, 2, 0, 0);

            // GET VERSION:
            lblVersionNumber.Text = AppResources.Version + ": " + Globals.VersionNumber().ToString();
            
        }

        void SendFeedback(Object sender, EventArgs e)
        {
           // if (MessageBox.Show("Send feedback?", AppResources.QuestionsOrComments, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
           ErrorReporting.SendFeedback();
        }

        void RateApp(Object sender, EventArgs e)
        {
            MarketplaceReviewTask mrt = new MarketplaceReviewTask();
            mrt.Show();
        }
    }
}
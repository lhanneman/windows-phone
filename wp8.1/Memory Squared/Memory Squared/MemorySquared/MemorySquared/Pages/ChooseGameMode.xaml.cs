using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace MemorySquared.Pages
{
    public partial class ChooseGameMode : PhoneApplicationPage
    {
        
        public ChooseGameMode()
        {
            InitializeComponent();
            
            // IF WE AREN'T DISPLAYING ADS, DON'T SHOW THE AD ROW:
            if (!Globals.bDisplayAds) AdRow.Height = new GridLength(0);

            // SET UP HANDLERS:
            ChooseModeGrid.Loaded += LoadChooseModePage;
            btnEasy.Tap += btnEasy_Tap;
            btnHard.Tap += btnHard_Tap;
        }

        void LoadChooseModePage(Object sender, EventArgs e)
        {
            // SETUP ADVERTISEMENT:
            if (Globals.bDisplayAds) Advertising.GetAdForGrid(LayoutRoot, 2, 0, 0);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // RESET DEACTIVATED SWITCH UPON NAVIGATING HERE (IF NEEDED):
            if (Globals.bDeactivated) Globals.bDeactivated = false;
        }

        void btnEasy_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Globals.Data.GameMode = Enumerations.GameMode.Easy;
            NavigationService.Navigate(new Uri("/Pages/Game.xaml", UriKind.Relative));
        }

        void btnHard_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Globals.Data.GameMode = Enumerations.GameMode.Hard;
            NavigationService.Navigate(new Uri("/Pages/Game.xaml", UriKind.Relative));
        }
        
    }
}
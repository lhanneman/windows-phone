using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using MemorySquared.Helpers;
using MemorySquared.Resources;

namespace MemorySquared
{
    public partial class Incorrect : PhoneApplicationPage
    {
        
        public Incorrect()
        {
            InitializeComponent();

            // IF WE AREN'T DISPLAYING ADS, DON'T SHOW THE AD ROW:
            if (!Globals.bDisplayAds) AdRow.Height = new GridLength(0);

            // ADD OUR HANDLERS:
            IncorrectGrid.Loaded += LoadIncorrect;
            btnTryAgain.Tap += btnTryAgain_Tap;
            btnMainMenu.Tap += btnMainMenu_Tap;
            btnAddScore.Tap += btnAddScore_Tap;

            // reset values:
            Globals.bIncorrect = false;
            Globals.bGameIsStarted = false;
            Globals.nSequenceCounter = 0;
        }

        void LoadIncorrect(object sender, EventArgs e)
        {

            // SETUP ADVERTISEMENT:
            if (Globals.bDisplayAds) Advertising.GetAdForGrid(LayoutRoot, 2, 0, 0);
            
            int nNewScore = 0, nOldScore = 0;

            // GET THEIR CURRENT SCORE FROM THIS ROUND:
            if (PhoneApplicationService.Current.State.ContainsKey("MemorySquared_NewScore"))
                nNewScore = int.Parse(PhoneApplicationService.Current.State["MemorySquared_NewScore"].ToString());

            // GET THEIR CURRENTLY SAVED HIGH SCORE:
            if (PhoneApplicationService.Current.State.ContainsKey("MemorySquared_OldScore"))
                nOldScore = int.Parse(PhoneApplicationService.Current.State["MemorySquared_OldScore"].ToString());

            // NEW HIGH SCORE?
            if (nNewScore > nOldScore)
            {
                // SHOW THE USER "NEW HIGH SCORE!":
                lblNewHighScore.Visibility = Visibility.Visible;

                // SAVE:
                Globals.Data.SaveGameData();
            }
           
            // If the new score is greater than the tenth (or last) score on the
            // leaderboard, and also greater than 0, we'll let them add this score to the leaderboard:
            if (nNewScore > 0 && LeaderboardHelper.CanAddToLeaderboard(Globals.Data, nNewScore) == true)
            {
                btnAddScore.Visibility = Visibility.Visible;
            }
            
            // SHOW THEM THE RESULTS:
            lblYourScore.Text = AppResources.YourScore + ": " + nNewScore.ToString();
            lblPreviousHighScore.Text = AppResources.PreviousHighScore + ": " + nOldScore.ToString();
        }

        /// <summary>
        /// TAKE THE USER BACK TO THE MAIN MENU WHEN THEY HIT BACK SO THEY CAN START A NEW GAME.
        /// </summary>
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/Menu.xaml", UriKind.Relative));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // RESET DEACTIVATED SWITCH UPON NAVIGATING HERE (IF NEEDED):
            if (Globals.bDeactivated) Globals.bDeactivated = false;
        }

        void btnTryAgain_Tap(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/Game.xaml", UriKind.Relative));
        }

        void btnMainMenu_Tap(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/Menu.xaml", UriKind.Relative));
        }

        void btnAddScore_Tap(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/AddName.xaml", UriKind.Relative));
        }
    }
}
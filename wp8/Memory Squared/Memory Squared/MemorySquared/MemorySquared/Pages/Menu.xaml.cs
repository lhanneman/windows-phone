using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;


#if DEBUG
using MockIAPLib;
using Store = MockIAPLib;
using MemorySquared.Resources;
using System.Globalization;
using System.Threading;
#else
using Windows.ApplicationModel.Store;
using Store = Windows.ApplicationModel.Store;
using MemorySquared.Resources;
#endif

namespace MemorySquared
{
    public partial class Menu : PhoneApplicationPage
    {
        public Menu()
        {
            InitializeComponent();

            // IF WE AREN'T DISPLAYING ADS, DON'T SHOW THE AD ROW:
            if (!Globals.bDisplayAds) AdRow.Height = new GridLength(0);

            // ADD OUR HANDLERS:
            MenuGrid.Loaded += LoadMenuPage;
            btnGame.Tap += btnGame_Tap;
            btnInstructions.Tap += btnInstructions_Tap;
            btnOptions.Tap += btnOptions_Tap;
            btnLeaderboard.Tap += btnLeaderboard_Tap;

#if DEBUG
            SeedForDebug();
#endif
                                   
        }

        /// <summary>
        /// WHEN THE BACK BUTTON IS PRESSED, EXIT GAME
        /// </summary>
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            while(NavigationService.BackStack.Any())
                NavigationService.RemoveBackEntry();
        }

        /// <summary>
        /// EXECUTES WHEN WE LOAD THE MENU PAGE
        /// </summary>
        void LoadMenuPage(object sender, EventArgs e)
        {

            // CHECK FOR PREVIOUS EXCEPTION:
            ErrorReporting.CheckForPreviousException();

            // SETUP ADVERTISEMENT:
            if (Globals.bDisplayAds) Advertising.GetAdForGrid(LayoutRoot, 2, 0, 0);
            
            // DISPLAY CURRENT HIGH SCORES:
            lblHighScoreEasy.Text = AppResources.HighScoreEasy + ": " + Globals.Data.HighScoreEasy;
            lblHighScoreHard.Text = AppResources.HighScoreHard + ": " + Globals.Data.HighScoreHard;

            if (Globals.bGameIsPaused == true)
                btnGame.Content = AppResources.ResumeGame;
            else
                btnGame.Content = AppResources.StartGame;

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // RESET DEACTIVATED SWITCH UPON NAVIGATING HERE (IF NEEDED):
            if (Globals.bDeactivated) Globals.bDeactivated = false;
        }

        void btnGame_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (Globals.bGameIsPaused == false)
            {
                // NEW GAME, LET THE USER CHOOSE THE MODE:
                NavigationService.Navigate(new Uri("/Pages/ChooseGameMode.xaml", UriKind.Relative));
            }
            else
            {
                // GAME WAS PAUSED, SO TAKE THEM RIGHT BACK:
                NavigationService.Navigate(new Uri("/Pages/Game.xaml", UriKind.Relative));
            }
        }

        void btnInstructions_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/Instructions.xaml", UriKind.Relative));
        }

        void btnOptions_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/Options.xaml", UriKind.Relative));
        }

        void btnLeaderboard_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/Leaderboard.xaml", UriKind.Relative));
        }

        private void SeedForDebug()
        {
            // POPULATE LEADERBOARD:
            Globals.Data.oLeaderboard.Leaders_Easy = new List<GameData.Leaderboard.Leader>();
            for (var i = 1; i < 31; i++)
            {
                GameData.Leaderboard.Leader leader = new GameData.Leaderboard.Leader();
                leader.Name = "User " + i.ToString();
                leader.Score = i;
                Globals.Data.oLeaderboard.Leaders_Easy.Add(leader);
            }

            Globals.Data.SaveGameData();
            
        }

    }
}
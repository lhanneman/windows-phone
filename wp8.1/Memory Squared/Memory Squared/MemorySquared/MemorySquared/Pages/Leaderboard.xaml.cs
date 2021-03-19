using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;

namespace MemorySquared.Pages
{
    public partial class Leaderboard : PhoneApplicationPage
    {
        public Leaderboard()
        {
            InitializeComponent();
            LayoutRoot.Loaded += LayoutRoot_Loaded;

            // IF WE AREN'T DISPLAYING ADS, DON'T SHOW THE AD ROW:
            if (!Globals.bDisplayAds) AdRow.Height = new GridLength(0);
        }

        void LayoutRoot_Loaded(object sender, EventArgs e)
        {

            LoadCurrentLeaderboard();

            // show the "hard" pivot if that's the mode they were playing:
            if (Globals.Data.GameMode == Enumerations.GameMode.Hard) LeaderboardPivot.SelectedIndex = 1;
           
        }

        void LoadCurrentLeaderboard()
        {
            List<LeaderboardDisplayObject> easyDisplayList = new List<LeaderboardDisplayObject>();
            List<LeaderboardDisplayObject> hardDisplayList = new List<LeaderboardDisplayObject>();

            List<GameData.Leaderboard.Leader> easyList = Globals.Data.oLeaderboard.Leaders_Easy.Take(30).OrderByDescending(l => l.Score).ToList();
            List<GameData.Leaderboard.Leader> hardList = Globals.Data.oLeaderboard.Leaders_Hard.Take(30).OrderByDescending(l => l.Score).ToList();

            int rank = 1;

            for(int i = 0; i < easyList.Count(); i++)
            {
                LeaderboardDisplayObject displayObj = new LeaderboardDisplayObject();
                displayObj.Leader = easyList[i];
                displayObj.DisplayText = String.Format("{0}. {1}  ({2})", rank, easyList[i].Name, easyList[i].Score);

                displayObj.BackgroundColor = GetBackgroundColor(i, Globals.Data.Theme);
                displayObj.ForegroundColor = GetForegroundColor(i, Globals.Data.Theme);

                easyDisplayList.Add(displayObj);

                rank += 1;
            }

            // reset rank for next list:
            rank = 1;

            for (int i = 0; i < hardList.Count(); i++)
            {
                LeaderboardDisplayObject displayObj = new LeaderboardDisplayObject();
                displayObj.Leader = hardList[i];
                displayObj.DisplayText = String.Format("{0}. {1}  ({2})", rank, hardList[i].Name, hardList[i].Score);

                displayObj.BackgroundColor = GetBackgroundColor(i, Globals.Data.Theme);
                displayObj.ForegroundColor = GetForegroundColor(i, Globals.Data.Theme);
                
                hardDisplayList.Add(displayObj);

                rank += 1;
            }

            EasyLeaderboard.ItemsSource = easyDisplayList;
            HardLeaderboard.ItemsSource = hardDisplayList;
            
        }

        private SolidColorBrush GetBackgroundColor(int index, Enumerations.Theme theme)
        {
            if (theme == Enumerations.Theme.Dark)
            {
                if (index % 2 == 0)
                    return new SolidColorBrush(Colors.Black);
                else
                    return new SolidColorBrush(Color.FromArgb(255, 30, 30, 30));
            }
            else
            {
                if (index % 2 == 0)
                    return new SolidColorBrush(Colors.White);
                else
                    return new SolidColorBrush(Color.FromArgb(255, 230, 230, 230));
            }
        }

        private SolidColorBrush GetForegroundColor(int index, Enumerations.Theme theme)
        {
            if (theme == Enumerations.Theme.Dark)
            {
                return new SolidColorBrush(Colors.White);
            }
            else
            {
                return new SolidColorBrush(Colors.Black);
            }
        }

        public class LeaderboardDisplayObject
        {
            public GameData.Leaderboard.Leader Leader { get; set; }
            public SolidColorBrush BackgroundColor { get; set; }
            public SolidColorBrush ForegroundColor { get; set; }
            public string DisplayText { get; set; }
        }

        /// <summary>
        /// WHEN THE BACK BUTTON IS PRESSED, WE WANT NAVIGATE BACK TO THE "INCORRECT" PAGE
        /// </summary>
        /// <param name="e"></param>
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/Menu.xaml", UriKind.Relative));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // RESET DEACTIVATED SWITCH UPON NAVIGATING HERE (IF NEEDED):
            if (Globals.bDeactivated) Globals.bDeactivated = false;
        }
    }
}
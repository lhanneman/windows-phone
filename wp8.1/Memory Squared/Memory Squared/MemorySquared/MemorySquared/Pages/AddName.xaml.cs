using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using MemorySquared.Helpers;

namespace MemorySquared.Pages
{
    public partial class AddName : PhoneApplicationPage
    {
        public AddName()
        {
            InitializeComponent();

            // SET UP HANDLERS:
            AddNameGrid.Loaded += AddNameGrid_Loaded;
            btnSave.Tap += btnSave_Tap;
        }

        void AddNameGrid_Loaded(object sender, EventArgs e)
        {
            txtName.Focus();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // RESET DEACTIVATED SWITCH UPON NAVIGATING HERE (IF NEEDED):
            if (Globals.bDeactivated) Globals.bDeactivated = false;
        }

        // SAVE THE USER'S NAME:
        void btnSave_Tap(object sender, EventArgs e)
        { 
            // GET NEW SCORE:
            int score = int.Parse(PhoneApplicationService.Current.State["MemorySquared_NewScore"].ToString());

            // ADD THIS NEW RECORD TO THE LEADERBOARD:
            LeaderboardHelper.AddToLeaderboard(Globals.Data, txtName.Text, score);

            // RESET OUR VARIABLES, AND TAKE THE USER TO VIEW THE LEADERBOARD:
            Globals.bIncorrect = false;
            Globals.bGameIsStarted = false;
            Globals.nSequenceCounter = 0;
            NavigationService.Navigate(new Uri("/Pages/Leaderboard.xaml", UriKind.Relative));

        }
    }
}
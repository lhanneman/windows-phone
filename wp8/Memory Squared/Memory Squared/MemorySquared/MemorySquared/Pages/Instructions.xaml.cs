using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace MemorySquared
{
    public partial class Instructions : PhoneApplicationPage
    {
               
        public Instructions()
        {
            InitializeComponent();

            // IF WE AREN'T DISPLAYING ADS, DON'T SHOW THE AD ROW:
            if (!Globals.bDisplayAds) AdRow.Height = new GridLength(0);

            // SET UP HANDLERS:
            InstructionsGrid.Loaded += LoadInstructions;

        }

        /// <summary>
        /// WHEN THE BACK BUTTON IS PRESSED, WE WANT TO NAVIGATE BACK TO THE MENU PAGE:
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

        /// <summary>
        /// EXECUTES WHEN WE LOAD THE INSTRUCTIONS PIVOT FOR THE FIRST TIME
        /// (ALSO EXECUTES WHEN COMING BACK TO THE MAINPIVOT, FOR EXAMPLE COMING BACK FROM
        /// THE OPTIONS PAGE).
        /// </summary>
        void LoadInstructions(object sender, EventArgs e)
        {
            // SETUP ADVERTISEMENT:
            if (Globals.bDisplayAds) Advertising.GetAdForGrid(LayoutRoot, 2, 0, 0);
        }

        void btnBack_Tap(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/Menu.xaml", UriKind.Relative));
        }      
    }
}
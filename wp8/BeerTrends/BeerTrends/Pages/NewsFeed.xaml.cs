using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using BeerTrends.Helpers;

namespace BeerTrends.Pages
{
    public partial class NewsFeed : PhoneApplicationPage
    {
        public NewsFeed()
        {
            InitializeComponent();
            InitializeAppBar();
            ContentPanel.Loaded += ContentPanel_Loaded;
        }

        void ContentPanel_Loaded(object sender, RoutedEventArgs e)
        {
            // CHECK FOR PREVIOUS EXCEPTION:
            ErrorReporting.CheckForPreviousException();
        }

        private void InitializeAppBar()
        {
            ApplicationBar = new ApplicationBar();
            ApplicationBar.Mode = ApplicationBarMode.Default;
            ApplicationBar.Opacity = 1.0;
            ApplicationBar.IsVisible = true;
            ApplicationBar.IsMenuEnabled = true;

            // add the app bar icons:
            ApplicationBarIconButton manualEntry = new ApplicationBarIconButton();
            manualEntry.IconUri = new Uri("/Assets/Images/add." + (Globals.IsDarkTheme ? "dark" : "light") + ".png", UriKind.Relative);
            manualEntry.Text = "log beer";
            ApplicationBar.Buttons.Add(manualEntry);
            manualEntry.Click += manualEntry_Click;

            ApplicationBarIconButton scanBeer = new ApplicationBarIconButton();
            scanBeer.IconUri = new Uri("/Assets/Images/camera." + (Globals.IsDarkTheme ? "dark" : "light") + ".png", UriKind.Relative);
            scanBeer.Text = "scan beer";
            ApplicationBar.Buttons.Add(scanBeer);
            scanBeer.Click += ScanBarcode_Click;

            ApplicationBarMenuItem profile = new ApplicationBarMenuItem();
            profile.Text = "my profile";
            ApplicationBar.MenuItems.Add(profile);
            profile.Click += MyProfile_Click;

            ApplicationBarMenuItem settings = new ApplicationBarMenuItem();
            settings.Text = "settings";
            ApplicationBar.MenuItems.Add(settings);
            settings.Click += Settings_Click;

            ApplicationBarMenuItem logout = new ApplicationBarMenuItem();
            logout.Text = "log out";
            ApplicationBar.MenuItems.Add(logout);
            logout.Click += Logout_Click;
        }

        

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            if (this.NavigationContext.QueryString.ContainsKey("FromLogin"))
            {
                bool fromLogin = false;
                if (bool.TryParse(this.NavigationContext.QueryString["FromLogin"], out fromLogin) == true)
                {
                    if (fromLogin)
                    {
                        // clear the backstack so we can't go Back to the login page:
                        while (NavigationService.BackStack.Any()) NavigationService.RemoveBackEntry();
                    }
                }
            }
        }

        /// <summary>
        /// WHEN THE BACK BUTTON IS PRESSED, EXIT APP
        /// </summary>
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {

            if (NavigationService.BackStack.Count() == 0)
            {
                // prompt user before exit:
                e.Cancel = MessageBoxResult.Cancel == MessageBox.Show("", "Are you sure you want to exit?", MessageBoxButton.OKCancel);

            }

            base.OnBackKeyPress(e);
        }

        

        void manualEntry_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/Beer/Search.xaml", UriKind.Relative));
        }

        void ScanBarcode_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/Barcode/Scan.xaml", UriKind.Relative));
        }

        void Settings_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/Settings/Settings.xaml", UriKind.Relative));
        }

        private void Logout_Click(object sender, EventArgs e)
        {
            ParseHelper.Logout();
            NavigationService.Navigate(new Uri("/Pages/Login.xaml", UriKind.RelativeOrAbsolute));
        }

        private void MyProfile_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/User/Profile.xaml", UriKind.Relative));
        }

    }
}
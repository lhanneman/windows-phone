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
using Parse;

namespace BeerTrends
{
    public partial class Login : PhoneApplicationPage
    {

        public Login()
        {
            InitializeComponent();
            btnNew.Tap += btnNew_Tap;
            btnLogin.Tap += btnLogin_Tap;
            btnFacebook.Tap += btnFacebook_Tap;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            
            // if there isn't a parse user in session, or we have a user saved in isolated storage, go straight to news feed
            if (ParseUser.CurrentUser != null)
            {
                NavigationService.Navigate(new Uri("/Pages/NewsFeed.xaml?FromLogin=true", UriKind.Relative));
            }

            // CHECK FOR PREVIOUS EXCEPTION:
            ErrorReporting.CheckForPreviousException();
        }

        void btnLogin_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

            if (txtEmail.Text == "")
            {
                MessageBox.Show("Please enter your email");
                return;
            }

            if (txtPassword.Password == "")
            {
                MessageBox.Show("Please enter your password");
                return;
            }

            
            ParseHelper.Login(txtEmail.Text, txtPassword.Password);
        }

        void btnNew_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/NewAccount.xaml", UriKind.Relative));
        }

        void btnFacebook_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/LoginFacebook.xaml", UriKind.Relative));
        }

        private void PasswordLostFocus(object sender, RoutedEventArgs e)
        {
            var passwordEmpty = string.IsNullOrEmpty(txtPassword.Password);
            PasswordWatermark.Visibility = passwordEmpty ? Visibility.Visible : Visibility.Collapsed;
            txtPassword.Visibility = passwordEmpty ? Visibility.Collapsed : Visibility.Visible;
        }

        private void PasswordGotFocus(object sender, RoutedEventArgs e)
        {
            PasswordWatermark.Visibility = Visibility.Collapsed;
            txtPassword.Visibility = Visibility.Visible;
            txtPassword.Focus();
        }
    }
}
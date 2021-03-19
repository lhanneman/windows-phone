using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Parse;
using Facebook;
using BeerTrends.Helpers;
using BeerTrends.Models;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Threading;

namespace BeerTrends.Pages
{
    public partial class LoginFacebook : PhoneApplicationPage
    {
        Task<ParseUser> task;
        CancellationTokenSource source;

        public LoginFacebook()
        {
            InitializeComponent();
            ContentPanel.Loaded += ContentPanel_Loaded;
        }

        async void ContentPanel_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                source = new CancellationTokenSource();
                fbBrowser.Visibility = Visibility.Visible;
                task = ParseFacebookUtils.LogInAsync(fbBrowser, new[] { "basic_info", "email", "user_location", "user_birthday" }, source.Token);
                await task;
                fbBrowser.Visibility = Visibility.Collapsed;

                // check to see if any fields need to be updated:
                FacebookHelper.CheckForUpdates();
                NavigationService.Navigate(new Uri("/Pages/NewsFeed.xaml?FromLogin=true", UriKind.Relative));
            }
            catch (OperationCanceledException)
            {
                task = null;
            }
            catch (Exception)
            {
                // other error:
                MessageBox.Show("There was an error trying to log in. Please try again.");
                return;
            }        
        }

        // cancel the current task if there is one when the user presses back:
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (task != null) source.Cancel(false);
            base.OnBackKeyPress(e);
        }
    }
}
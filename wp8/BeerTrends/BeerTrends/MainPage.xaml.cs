using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using BeerTrends.Resources;
using Facebook.Client;
using BeerTrends.Helpers;

namespace BeerTrends
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            btnFacebookLogin.SessionStateChanged += btnFacebookLogin_SessionStateChanged;
        }

        void UserInfoChanged(object sender, Facebook.Client.Controls.UserInfoChangedEventArgs e)
        {
            Facebook.Client.GraphUser user = e.User;
        }

        void btnFacebookLogin_SessionStateChanged(object sender, Facebook.Client.Controls.SessionStateChangedEventArgs e)
        {
            if (e.SessionState == Facebook.Client.Controls.FacebookSessionState.Opened)
            {
                
            }
        }
     
    }
}
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
using System.Windows.Media.Imaging;
using System.Windows.Media;
using BeerTrends.Helpers;
using System.Windows.Documents;
using Microsoft.Phone.Tasks;

namespace BeerTrends.Pages.User
{
    public partial class Profile : PhoneApplicationPage
    {
        public Profile()
        {
            InitializeComponent();
            ContentPanel.Loaded += ContentPanel_Loaded;
            lblDiscoveredCount.Tap += lblDiscoveredCount_Tap;
            lblTotalBeersDrank.Tap += lblTotalBeersDrank_Tap;
            lblFavorites.Tap += lblFavorites_Tap;
            lblFollowers.Tap += lblFollowers_Tap;
            lblFollowing.Tap += lblFollowing_Tap;
        }

        void ContentPanel_Loaded(object sender, RoutedEventArgs e)
        {

            lblName.Text = ParseUser.CurrentUser["fullName"].ToString();
           // lblDisplayName.Text = ParseUser.CurrentUser["displayName"].ToString();

            // if we have a city and state, display location:
            if (ParseUser.CurrentUser["city"] != null && ParseUser.CurrentUser["state"] != null)
            {
                lblDisplayName.Text += " (" + ParseUser.CurrentUser["city"].ToString() + ", " + ParseUser.CurrentUser["state"].ToString() + ")";
            }

            lblDiscoveredCount.Text = "16"; // TODO: look up discovered count
            lblDiscoveredCount.Inlines.Add(new Run()
            {
                // add a STAR
                Text = "\uE0B4",
                FontFamily = new FontFamily("Segoe UI Symbol")
            });

            // get their profile pic:
            LoadProfilePic();
            
            lblBio.Text = ParseUser.CurrentUser["bio"].ToString();

            lblFollowing.Text = "Following: 1008";
            lblFollowers.Text = "Followers: 16";
            lblFavorites.Text = "Favorites: 88";

        }

        private async void LoadProfilePic()
        {
            // make sure there is a thumbnail:
            if (ParseUser.CurrentUser["thumbnail"] != null && (string)ParseUser.CurrentUser["thumbnail"] != "")
            {
                string t = ParseUser.CurrentUser["thumbnail"].ToString();
                if (t.ToLower().StartsWith("http"))
                {
                    // url:
                    imgThumbnail.Source = new BitmapImage(new Uri((string)ParseUser.CurrentUser["thumbnail"]));
                }
                else
                {
                    // parse object (saved image in parse):
                    ParseQuery<ParseObject> query = ParseObject.GetQuery("Thumbnails");
                    ParseObject thumbnail = await query.GetAsync(t);
                    var thumbnailFile = thumbnail.Get<ParseFile>("thumbnail");
                    imgThumbnail.Source = new BitmapImage(thumbnailFile.Url);
                }

            }
        }

        private void ChangePicture(object sender, RoutedEventArgs e)
        {
            PhotoChooserTask pct = new PhotoChooserTask();
            pct.Completed += pct_Completed;
            pct.ShowCamera = true;
            pct.Show();
        }

        async void pct_Completed(object sender, PhotoResult e)
        {

            await ParseHelper.UpdateUserProfilePic(e.ChosenPhoto);

            // update profile image on page:
            BitmapImage bi = new BitmapImage();
            bi.SetSource(e.ChosenPhoto);
            imgThumbnail.Source = bi;

        }

        void lblFollowing_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/User/List.xaml?List=following", UriKind.Relative));
        }

        void lblFollowers_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/User/List.xaml?List=followers", UriKind.Relative));
        }

        void lblDiscoveredCount_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/Beer/List.xaml?List=discovered", UriKind.Relative));
        }

        void lblFavorites_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/Beer/List.xaml?List=favorites", UriKind.Relative));
        }

        void lblTotalBeersDrank_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/Beer/List.xaml?List=total", UriKind.Relative));
        }


    }
}
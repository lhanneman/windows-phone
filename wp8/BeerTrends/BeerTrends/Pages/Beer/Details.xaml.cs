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
using BeerTrends.Models;
using BeerTrends.ViewModel;
using System.Threading.Tasks;
using Parse;
using System.IO.IsolatedStorage;

namespace BeerTrends.Pages.Beer
{
    public partial class Details : PhoneApplicationPage
    {
        private BeerViewModel vm;

        public Details()
        {
            InitializeComponent();
            vm = new BeerViewModel();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (this.NavigationContext.QueryString.ContainsKey("UPC"))
            {
                string upc = this.NavigationContext.QueryString["UPC"];
                //lblUPC.Text = upc;

                // TODO: LOOK UP BEER BY UPC 

            }
            else if (PhoneApplicationService.Current.State.Keys.Contains(Constants.SelectedBeer))
            {
                Models.Beer selectedBeer = (Models.Beer)PhoneApplicationService.Current.State[Constants.SelectedBeer];

                // remove key here?
                vm.Beer = selectedBeer;
                lblBeerName.Text = selectedBeer.name;
                BuildAppBar(selectedBeer.id);
                TheBeerView.DataContext = vm.Beer;
            }
            
        }

        private void BuildAppBar(string beerId)
        {
            ApplicationBar = new ApplicationBar();

            if (ISOStorageHelper.BeerIsFavorited(beerId))
                AddUnfavoriteButton();
            else
                AddFavoriteButton();

            AddTermsButton();
        }

        private void UpdateAppBar(bool isFavorited)
        {
            if (isFavorited)
                AddUnfavoriteButton();
            else
                AddFavoriteButton();
        }

        private void AddFavoriteButton()
        {
            ApplicationBarIconButton favButton = new ApplicationBarIconButton();
            favButton.IconUri = new Uri("/Assets/Images/favorite." + (Globals.IsDarkTheme ? "dark" : "light") + ".png", UriKind.Relative);
            favButton.Text = "Add Favorite";
            ApplicationBar.Buttons.Insert(0, favButton);
            favButton.Click += favButton_Click;
        }

        private void favButton_Click(object sender, EventArgs e)
        {
            ParseHelper.OnUpdateFavorite += ParseHelper_OnAddFavorite;
            pbLoading.Visibility = Visibility.Visible;
            ParseHelper.FavoriteBeer(vm.Beer.id);
            ApplicationBarIconButton btn = (ApplicationBarIconButton)sender;
            ApplicationBar.Buttons.Remove(btn);
        }

        private void AddUnfavoriteButton()
        {
            ApplicationBarIconButton favButton = new ApplicationBarIconButton();
            favButton.IconUri = new Uri("/Assets/Images/unfavorite." + (Globals.IsDarkTheme ? "dark" : "light") + ".png", UriKind.Relative);
            favButton.Text = "Unfavorite";
            ApplicationBar.Buttons.Insert(0, favButton);
            favButton.Click += unfavButton_Click;
        }

        private void unfavButton_Click(object sender, EventArgs e)
        {
            ParseHelper.OnUpdateFavorite += ParseHelper_OnRemoveFavorite;
            pbLoading.Visibility = Visibility.Visible;
            ParseHelper.UnfavoriteBeer(vm.Beer.id);
            ApplicationBarIconButton btn = (ApplicationBarIconButton)sender;
            ApplicationBar.Buttons.Remove(btn);
        }

        void ParseHelper_OnAddFavorite(bool success, string result)
        {
            ParseHelper.OnUpdateFavorite -= ParseHelper_OnAddFavorite;
            pbLoading.Visibility = Visibility.Collapsed;
            if (success)
            {
                UpdateAppBar(true);
                ISOStorageHelper.AddFavorite(vm.Beer.id);
            }
            MessageBox.Show(result);
        }

        void ParseHelper_OnRemoveFavorite(bool success, string result)
        {
            ParseHelper.OnUpdateFavorite -= ParseHelper_OnRemoveFavorite;
            pbLoading.Visibility = Visibility.Collapsed;
            if (success)
            {
                UpdateAppBar(false);
                ISOStorageHelper.RemoveFavorite(vm.Beer.id);
            }
            MessageBox.Show(result);
        }

        private void AddTermsButton()
        {
            ApplicationBarIconButton termsButton = new ApplicationBarIconButton();
            termsButton.IconUri = new Uri("/Assets/Images/questionmark." + (Globals.IsDarkTheme ? "dark" : "light") + ".png", UriKind.Relative);
            termsButton.Text = "terms";
            ApplicationBar.Buttons.Add(termsButton);
            termsButton.Click += termButton_Click;
        }

        private void termButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/Beer/Terms.xaml", UriKind.Relative));
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using BeerTrends.ViewModel;
using BeerTrends.Helpers;
using BeerTrends.Models;

namespace BeerTrends.Pages.Beer
{
    public partial class List : PhoneApplicationPage
    {
        private BeerListViewModel vm;

        public List()
        {
            InitializeComponent();
            vm = new BeerListViewModel();
            vm.OnRequestCompleted += RequestCompleted;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            if (this.NavigationContext.QueryString.ContainsKey("List"))
            {
                string list = this.NavigationContext.QueryString["List"];
                if (list == null || list == "") return;
                switch (list.ToLower())
                {
                    case "favorites":
                        lblTitle.Text = "Favorites";
                        vm.GetFavorites();
                        break;
                    case "discovered":
                        lblTitle.Text = "Discovered Beers";
                        break;
                    case "total":
                        lblTitle.Text = "Total Beer Drank";
                        break;
                    default:
                        return;
                }

                BeerListView.DataContext = vm.Beers;

            }
        }

        void RequestCompleted(BreweryDBResult result)
        {
            ToggleLoading(false);
            //PhoneApplicationService.Current.State[Constants.Favorites] = result;
        }

        void ContentPanel_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void ToggleLoading(bool showLoading)
        {
            if (showLoading)
            {
                spLoading.Visibility = Visibility.Visible;
                BeerListView.Visibility = Visibility.Collapsed;
            }
            else
            {
                spLoading.Visibility = Visibility.Collapsed;
                BeerListView.Visibility = Visibility.Visible;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Input;
using BeerTrends.Helpers;
using BeerTrends.ViewModel;
using System.Collections.ObjectModel;
using BeerTrends.Models;

namespace BeerTrends.Pages.Beer
{
    public partial class Search : PhoneApplicationPage
    {
        private BeerListViewModel vm;

        // constructor
        public Search()
        {
            InitializeComponent();
            txtSearch.KeyDown += txtSearch_KeyDown;
            vm = new BeerListViewModel();
            vm.OnRequestCompleted += SearchCompleted;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // check for previous items:
            if (PhoneApplicationService.Current.State.Keys.Contains(Constants.PreviousBeerSearch))
            {
                BreweryDBResult lastSearch = (BreweryDBResult)PhoneApplicationService.Current.State[Constants.PreviousBeerSearch];
                for (var i = 0; i < lastSearch.data.Count(); i++)
                {
                    vm.Beers.Add(lastSearch.data[i]);
                }
            }

            BeerListView.DataContext = vm.Beers;
        }

        void txtSearch_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ToggleLoading(true);
                vm.Search(txtSearch.Text);
                BeerListView.Focus();
            }
        }

        void SearchCompleted(BreweryDBResult result)
        {
            ToggleLoading(false);
            PhoneApplicationService.Current.State[Constants.PreviousBeerSearch] = result;
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
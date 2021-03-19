using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using BeerTrends.Models;
using BeerTrends.Helpers;

namespace BeerTrends.View
{
    public partial class BeerListView : UserControl
    {
        public BeerListView()
        {
            InitializeComponent();
            
        }

        private void lb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox lb = (ListBox)sender;
            Beer selectedBeer = (Beer)lb.SelectedItem;
            PhoneApplicationService.Current.State[Constants.SelectedBeer] = selectedBeer;
            (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/Pages/Beer/Details.xaml", UriKind.Relative));
        }

 

      
    }
}

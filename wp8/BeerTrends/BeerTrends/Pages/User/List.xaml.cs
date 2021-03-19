using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace BeerTrends.Pages.User
{
    public partial class List : PhoneApplicationPage
    {
        public List()
        {
            InitializeComponent();
            ContentPanel.Loaded += ContentPanel_Loaded;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            if (this.NavigationContext.QueryString.ContainsKey("List"))
            {
                string list = this.NavigationContext.QueryString["List"];
                if(list == null || list == "") return;
                switch (list.ToLower())
                {
                    case "followers":
                        lblTitle.Text = "Followers";
                        break;
                    case "following":
                        lblTitle.Text = "Following";
                        break;
                    default:
                        MessageBox.Show("Error list not recognized: " + list);
                        break;
                }
               
            }
        }

        void ContentPanel_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using MapMyElevation.ViewModel;

namespace MapMyElevation.Pages
{
    public partial class Options : PhoneApplicationPage
    {
        private OptionsViewModel viewModel;

        public Options()
        {
            InitializeComponent();
            viewModel = new OptionsViewModel();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (!StateUtilities.IsLaunching && this.State.ContainsKey("Options"))
                viewModel = (OptionsViewModel)this.State["Options"];
            else
                viewModel.GetOptions();   

            OptionsView.DataContext = viewModel.Options;
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            if (this.State.ContainsKey("Options"))
            {
                this.State["Options"] = viewModel;
            }
            else
            {
                this.State.Add("Options", viewModel);
            }

            StateUtilities.IsLaunching = false;
        }

    }
}
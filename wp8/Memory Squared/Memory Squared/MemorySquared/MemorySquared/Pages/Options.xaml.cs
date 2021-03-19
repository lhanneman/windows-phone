using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Linq;
using System.Windows.Media;
using System.Windows.Navigation;
using MemorySquared.Resources;

#if DEBUG
using MockIAPLib;
using Store = MockIAPLib;
using System.Collections.Generic;
using System.Collections.ObjectModel;
#else
using Windows.ApplicationModel.Store;
using Store = Windows.ApplicationModel.Store;
#endif


namespace MemorySquared
{
    public partial class Options : PhoneApplicationPage
    {
        public Options()
        {
            InitializeComponent();

            // IF WE AREN'T DISPLAYING ADS, DON'T SHOW THE AD ROW:
            if (!Globals.bDisplayAds) AdRow.Height = new GridLength(0);
            
            // LOAD THE OPTIONS CANVAS ONCE THE PAGE HAS FULLY LOADED:
            OptionsGrid.Loaded += LoadOptions;
            btnGameColor.Tap += btnGameColor_Tap;
            btnUserColor.Tap += btnUserColor_Tap;
            tsSequencePrompt.Checked += tsSequencePrompt_Checked;
            tsSequencePrompt.Unchecked += tsSequencePrompt_Checked;
            tsLightTheme.Checked += tsLightTheme_Checked;
            tsLightTheme.Unchecked += tsLightTheme_Checked;
            btnAbout.Tap += btnAbout_Tap;
            btnRemoveAds.Tap += btnRemoveAds_Tap;

        }

        /// <summary>
        /// WHEN THE BACK BUTTON IS PRESSED, WE WANT TO NAVIGATE BACK TO THE MENU PAGE:
        /// </summary>
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/Menu.xaml", UriKind.Relative));
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            // reset our deactivated switch if we come back to this page
            // after hitting Start:
            if (Globals.bDeactivated) Globals.bDeactivated = false;

            // if we came from the color picker, then we need to update either the user or game color:
            JournalEntry lastPage = NavigationService.BackStack.FirstOrDefault();
            if (lastPage.Source.ToString() == "/Pages/ColorPicker.xaml")
            {
                // which one are we updating?
                if (PhoneApplicationService.Current.State.Keys.Contains("MemorySquared_SetColor"))
                {
                    switch (PhoneApplicationService.Current.State["MemorySquared_SetColor"].ToString())
                    {
                        case "user":
                            Globals.Data.UserColor = (Application.Current as App).CurrentColorItem.Color;
                            Globals.Data.SaveGameData();
                            break;
                        case "game":
                            Globals.Data.GameColor = (Application.Current as App).CurrentColorItem.Color;
                            Globals.Data.SaveGameData();
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// LOADS UP EVERYTHING ONTO THE OPTIONS CANVAS
        /// </summary>
        void LoadOptions(object sender, EventArgs e)
        {
            // SETUP ADVERTISEMENT:
            if (Globals.bDisplayAds) Advertising.GetAdForGrid(LayoutRoot, 2, 0, 0);

            rectGameColor.Fill = new SolidColorBrush(Globals.Data.GameColor);
            rectUserColor.Fill = new SolidColorBrush(Globals.Data.UserColor);

            tsSequencePrompt.IsChecked = Globals.Data.YourTurnHelper;
            tsLightTheme.IsChecked = Globals.Data.Theme == Enumerations.Theme.Light ? true : false;

            //// GET THE SELECTED VALUE:
            //lpGameColor.SelectedIndex = Globals.Data.GameColor;
            //lpGameColor.SelectionChanged += UpdateSelectedColor;
            //lpGameColor.SetValue(Canvas.ZIndexProperty, 9999);

            //// GET THE SELECTED GAME COLOR VALUE:
            //lpUserColor.SelectedIndex = Globals.Data.UserColor;
            //lpUserColor.SelectionChanged += UpdateSelectedColor;
            //lpUserColor.SetValue(Canvas.ZIndexProperty, 9998);

            //// GET THE SELECTED VALUE:
            //cbYourTurn.IsChecked = Globals.Data.YourTurnHelper;
            //cbYourTurn.Checked += YourTurnCheckBox_Checked;
            //cbYourTurn.Unchecked += YourTurnCheckBox_Checked;

            ////// WHICH THEME DOES THE USER HAVE SAVED?
            //if (Globals.Data.Theme == Enumerations.Theme.Light)
            //   lpTheme.SelectedIndex = 0;
            //else // DISPLAY DARK IF NO THEME CHOSEN:
            //    lpTheme.SelectedIndex = 1;

            //lpTheme.SelectionChanged += UpdateTheme;

            //// DO WE NEED TO DISPLAY ADS?
            //if (Globals.bDisplayAds == true) btnRemoveAds.Visibility = Visibility.Visible;
            
        }

        void btnGameColor_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            PhoneApplicationService.Current.State["MemorySquared_SetColor"] = "game";
            NavigationService.Navigate(new Uri("/Pages/ColorPicker.xaml", UriKind.Relative));
        }

        void btnUserColor_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            PhoneApplicationService.Current.State["MemorySquared_SetColor"] = "user";
            NavigationService.Navigate(new Uri("/Pages/ColorPicker.xaml", UriKind.Relative));
        }


        void tsLightTheme_Checked(object sender, RoutedEventArgs e)
        {
            ToggleSwitch ts = (ToggleSwitch)sender;
            if (ts.IsChecked == true)
            {
                Globals.Data.Theme = Enumerations.Theme.Light;
                ThemeManager.ToLightTheme();
            }
            else
            {
                Globals.Data.Theme = Enumerations.Theme.Dark;
                ThemeManager.ToDarkTheme();
            }

            Globals.Data.SaveGameData();
        }

        void tsSequencePrompt_Checked(object sender, RoutedEventArgs e)
        {
            ToggleSwitch ts = (ToggleSwitch)sender;
            Globals.Data.YourTurnHelper = ts.IsChecked;
            Globals.Data.SaveGameData();
           
        }

        /// <summary>
        /// NAVIGATE TO THE ABOUT PAGE WHEN THE USER TAPS THE ABOUT BUTTON
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnAbout_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/About.xaml", UriKind.Relative));
        }

        async void btnRemoveAds_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

            try
            {
                string productID = "RemoveAds";

                ListingInformation li = await Store.CurrentApp.LoadListingInformationAsync();
                string pID = li.ProductListings[productID].ProductId;

                string receipt = await Store.CurrentApp.RequestProductPurchaseAsync(pID, false);

                // should be active now if purchase was successful:
                if (Store.CurrentApp.LicenseInformation.ProductLicenses[productID].IsActive)
                {
                    if (MessageBox.Show(AppResources.PurchaseSuccessful, AppResources.Success, MessageBoxButton.OK) == MessageBoxResult.OK)
                    {
                        Globals.bDisplayAds = false; // purchase complete, don't display ads

                        NavigationService.Navigate(new Uri("/Pages/Menu.xaml", UriKind.Relative));
                    }
                }
            }
            catch (Exception ex)
            {
                if (MessageBox.Show(AppResources.ErrorProcessingRequest, AppResources.ErrorRequestMessage, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    string sMessage = "Exception: " + ex.ToString();
                    if(ex.Message != "")
                        sMessage += "; Exception Message: " + ex.Message.ToString();

                    if (ex.InnerException != null)
                        sMessage += "; Inner Exception: " + ex.InnerException.ToString();

                    ErrorReporting.SendErrorDetails(sMessage);
                }
            }
        }
    }
}
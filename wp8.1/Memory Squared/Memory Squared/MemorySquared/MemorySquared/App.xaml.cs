﻿using System;
using System.Diagnostics;
using System.Resources;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using MemorySquared.Resources;
using MemorySquared.Helpers;

#if DEBUG
using MockIAPLib;
using Store = MockIAPLib;
using System.Windows.Media;
#else
using Windows.ApplicationModel.Store;
using Store = Windows.ApplicationModel.Store;
#endif

namespace MemorySquared
{
    public partial class App : Application
    {
        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public static PhoneApplicationFrame RootFrame { get; private set; }

        public ColorItem CurrentColorItem { set; get; }

        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {
            // Global handler for uncaught exceptions.
            UnhandledException += Application_UnhandledException;
            
            // Standard XAML initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();

            // load game data:
            if (Globals.Data == null) Globals.Data = GameData.GetGameData();

            if (Globals.Data.Theme == Enumerations.Theme.Light)
                ThemeManager.ToLightTheme();
            else
                ThemeManager.ToDarkTheme();

            // Language display initialization
            InitializeLanguage();

            // Show graphics profiling information while debugging.
            if (Debugger.IsAttached)
            {
                // Display the current frame rate counters.
                Application.Current.Host.Settings.EnableFrameRateCounter = false;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode,
                // which shows areas of a page that are handed off to GPU with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                // Prevent the screen from turning off while under the debugger by disabling
                // the application's idle detection.
                // Caution:- Use this under debug mode only. Application that disables user idle detection will continue to run
                // and consume battery power when the user is not using the phone.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }

            App.RootFrame.Obscured += App_Obscured;

            SetupMockIAP();

        }

        private void SetupMockIAP()
        {
#if DEBUG
            MockIAP.Init();
            MockIAP.RunInMockMode(true);
            MockIAP.SetListingInformation(1, "en-us", "Remove Ads", "1", "MemorySquared");

            // ADD ONE ITEM TO PURCHASE WHICH WILL BE THE VERSION
            // OF THE GAME WITHOUT ADS:

            ProductListing p = new ProductListing
            {
                Name = "RemoveAds",
                //ImageUri = new Uri("/Res/Image/1.png", UriKind.Relative),
                ProductId = "RemoveAds",
                ProductType = Windows.ApplicationModel.Store.ProductType.Durable,
                Keywords = new string[] { "Remove", "Ads" },
                Description = "This will remove ads from the app",
                FormattedPrice = "1.0",
                Tag = string.Empty
            };

            MockIAP.AddProductListing("RemoveAds", p);
#endif
        }

        /// <summary>
        /// GETS CALLED WHEN PHONE IS INTERRUPTED (I.E. NOTIFICATIONS, PHONE CALLS, LOCK SCREEN)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void App_Obscured(object sender, ObscuredEventArgs e)
        {
            // THIS WILL LET US KNOW LATER ON, IF RE-ACTIVATED
            // THAT WE NEED TO SHOW THE PAUSE GRID:
            Globals.bDeactivated = true;
            //Globals.bCancel = true;

            if (Globals.bGameIsStarted)
            {
                GameHelper.PauseGame(true);
            }
        }


        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
            // DO WE NEED TO LOAD ADS OR NOT?
            StoreManager.CheckForAds();
        }
        
        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {

            
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
            // THIS WILL LET US KNOW LATER ON, IF RE-ACTIVATED
            // THAT WE NEED TO SHOW THE PAUSE GRID:
            Globals.bDeactivated = true;
            //Globals.bCancel = true;

            if (Globals.bGameIsStarted)
            {
                GameHelper.PauseGame(true);
            }

        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
        }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            string sMessage = "";

            // CHECK FOR MESSAGE FROM CODE:
            if (PhoneApplicationService.Current.State.ContainsKey("MemorySquared_ErrorMessage"))
            {
                sMessage = PhoneApplicationService.Current.State["MemorySquared_ErrorMessage"].ToString();
            }

            // REPORT THE ERROR:
            ErrorReporting.ReportException(e.Exception, sMessage);

            if (Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                Debugger.Break();
            }
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {

            string sMessage = "";

            // CHECK FOR MESSAGE FROM CODE:
            if (PhoneApplicationService.Current.State.ContainsKey("MemorySquared_ErrorMessage"))
            {
                sMessage = PhoneApplicationService.Current.State["MemorySquared_ErrorMessage"].ToString();
            }
            
            // REPORT THE ERROR:
            ErrorReporting.ReportException(e.ExceptionObject, sMessage);
            
            if (Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                Debugger.Break();
            }
        }

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new PhoneApplicationFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Handle reset requests for clearing the backstack
            RootFrame.Navigated += CheckForResetNavigation;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        private void CheckForResetNavigation(object sender, NavigationEventArgs e)
        {
            // If the app has received a 'reset' navigation, then we need to check
            // on the next navigation to see if the page stack should be reset
            if (e.NavigationMode == NavigationMode.Reset)
                RootFrame.Navigated += ClearBackStackAfterReset;
        }

        private void ClearBackStackAfterReset(object sender, NavigationEventArgs e)
        {
            // Unregister the event so it doesn't get called again
            RootFrame.Navigated -= ClearBackStackAfterReset;

            // Only clear the stack for 'new' (forward) and 'refresh' navigations
            if (e.NavigationMode != NavigationMode.New && e.NavigationMode != NavigationMode.Refresh)
                return;

            // For UI consistency, clear the entire page stack
            while (RootFrame.RemoveBackEntry() != null)
            {
                ; // do nothing
            }
        }

        #endregion

        private void InitializeLanguage()
        {
            try
            {
                // Set the font to match the display language defined by the
                // ResourceLanguage resource string for each supported language.
                //
                // Fall back to the font of the neutral language if the Display
                // language of the phone is not supported.
                //
                // If a compiler error is hit then ResourceLanguage is missing from
                // the resource file.
                RootFrame.Language = XmlLanguage.GetLanguage(AppResources.ResourceLanguage);

                // Set the FlowDirection of all elements under the root frame based
                // on the ResourceFlowDirection resource string for each
                // supported language.
                //
                // If a compiler error is hit then ResourceFlowDirection is missing from
                // the resource file.
                FlowDirection flow = (FlowDirection)Enum.Parse(typeof(FlowDirection), AppResources.ResourceFlowDirection);
                RootFrame.FlowDirection = flow;
            }
            catch
            {
                // If an exception is caught here it is most likely due to either
                // ResourceLangauge not being correctly set to a supported language
                // code or ResourceFlowDirection is set to a value other than LeftToRight
                // or RightToLeft.

                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }

                throw;
            }
        }

    }
}
using MapMyElevation.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Windows.System;
using System.Windows;
using Windows.Devices.Geolocation;


namespace MapMyElevation
{
    public class UserSettings
    {

        private const string sFile = @"MapMyElevation\MapMyElevationUserSettings.xml";

        public OptionsModel Options { get; set; }
        //public bool? FirstTime { get; set; }
        //public bool LocationConsent { get; set; }

        public static void SaveUserSettings(UserSettings settings)
        {

            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {

                if (isf.DirectoryExists("MapMyElevation") == false) isf.CreateDirectory("MapMyElevation");

                XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
                xmlWriterSettings.Indent = true;

                using (IsolatedStorageFileStream stream = isf.OpenFile(sFile, FileMode.Create))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(UserSettings));
                    using (XmlWriter xmlWriter = XmlWriter.Create(stream, xmlWriterSettings))
                    {
                        serializer.Serialize(xmlWriter, settings);
                    }
                }

            }

        }

        public static UserSettings GetUserSettings()
        {
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {

                if (isf.FileExists(sFile) == false) return CreateDefaultSettings();

                UserSettings settings = new UserSettings();

                using (IsolatedStorageFileStream stream = isf.OpenFile(sFile, FileMode.Open))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(UserSettings));
                    settings = (UserSettings)serializer.Deserialize(stream);
                    stream.Close();
                }

                return settings;

            }

        }

        public static void SaveOptions(OptionsModel model)
        {
            // GET THE CURRENT SETTINGS, UPDATE THE OPTIONS, SAVE:
            UserSettings settings = GetUserSettings();
            settings.Options = model;
            SaveUserSettings(settings);
        }

        private static UserSettings CreateDefaultSettings()
        {
            UserSettings settings = new UserSettings();

            settings.Options = GetDefaultOptions();

            UserSettings.SaveUserSettings(settings);

            return settings;

        }

        private static OptionsModel GetDefaultOptions()
        {
            OptionsModel model = new OptionsModel();
            model.ElevationUnit = (int)Helpers.Enumerations.ElevationUnit.Meters;
            model.DistanceUnit = (int)Helpers.Enumerations.DistanceUnit.Miles;
            return model;
        }

        // Function open the Location Settings App using URI Scheme in Windows Phone 8 using C#.
        public static async void LaunchLocationSettingsApp()
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings-location:"));
        }

        public static bool CheckLocationConsentAndSettings()
        {
            // load the user's settings to check if first time or location settings are off:
            //UserSettings settings = UserSettings.GetUserSettings();
            if (!IsolatedStorageSettings.ApplicationSettings.Contains("LocationConsent"))
            {
                if (MessageBox.Show("This app accesses your phone's location. Is that ok?", "Location", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    IsolatedStorageSettings.ApplicationSettings["LocationConsent"] = true;
                else
                    IsolatedStorageSettings.ApplicationSettings["LocationConsent"] = false;

                IsolatedStorageSettings.ApplicationSettings.Save();
            }
            else if ((bool)IsolatedStorageSettings.ApplicationSettings["LocationConsent"] == false)
            {
                // there is a key, but user said no. Check again:
                if (MessageBox.Show("This app accesses your phone's location. Is that ok?", "Location", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    IsolatedStorageSettings.ApplicationSettings["LocationConsent"] = true;
                else
                    IsolatedStorageSettings.ApplicationSettings["LocationConsent"] = false;

                IsolatedStorageSettings.ApplicationSettings.Save();
            }

            if ((bool)IsolatedStorageSettings.ApplicationSettings["LocationConsent"] == true)
            {

                // the user gave permission to use location, now make sure their location settings are turned on:
                Geolocator geo = new Geolocator();

                if (geo.LocationStatus == PositionStatus.Disabled)
                {
                    // Location is turned off
                    //btnMapRoute.IsEnabled = false;
                    //btnQuickCheck.IsEnabled = false;

                    if (MessageBox.Show("This app requires your location settings to be turned on. Would you like to go to your Location settings now?", "Location Required", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        UserSettings.LaunchLocationSettingsApp();
                    }
                }
                else
                {
                    // location enabled:
                   // btnMapRoute.IsEnabled = true;
                   // btnQuickCheck.IsEnabled = true;
                    return true;
                }

            }
            else
            {
                // user did not give location consent:
                //btnMapRoute.IsEnabled = false;
                //btnQuickCheck.IsEnabled = false;
            }

            return false;
        }

        
    }
}

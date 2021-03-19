using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using BeerTrends.Models;
using Parse;

namespace BeerTrends.Helpers
{
    public static class LocationHelper
    {
        public static void UpdateUserLocationFromZip(string zip)
        {

            try
            {
                // make sure we have an object to update once we get the response:
                if (ParseUser.CurrentUser != null)
                {
                    string url = "https://maps.googleapis.com/maps/api/geocode/json?address=" + zip + "&sensor=true";

                    WebClient request = new WebClient();
                    request.DownloadStringCompleted += GotLocationFromZip;
                    Uri uri = new Uri(url);
                    request.DownloadStringAsync(uri);

                }

            }
            catch
            {
                // do nothing, the city/state/country just won't be set
            }
        }

        static void GotLocationFromZip(object sender, DownloadStringCompletedEventArgs e)
        {

            try
            {
                // get a location object from the JSON returned:
                UserLocation location = JsonConvert.DeserializeObject<UserLocation>(e.Result);

                if (location.results.Length > 0)
                {
                    ParseUser.CurrentUser["city"] = location.results[0].address_components[1].long_name;
                    ParseUser.CurrentUser["state"] = location.results[0].address_components[3].long_name;
                    ParseUser.CurrentUser["country"] = location.results[0].address_components[4].long_name;

                    ParseUser.CurrentUser.SaveAsync();

                }
            }
            catch
            {
                // do nothing, the city/state/country just won't be set
            }

        }

        // pass in as comma separated
        public static void UpdateUserLocationFromCityState(string cityState)
        {

            try
            {
                // make sure we have an object to update once we get the response:
                if (ParseUser.CurrentUser != null)
                {
                    string url = "https://maps.googleapis.com/maps/api/geocode/json?address=" + cityState + "&sensor=true";

                    WebClient request = new WebClient();
                    request.DownloadStringCompleted += GotLocationFromCityState;
                    Uri uri = new Uri(url);
                    request.DownloadStringAsync(uri);

                }

            }
            catch
            {
                // do nothing, the city/state/country just won't be set
            }
        }

        static void GotLocationFromCityState(object sender, DownloadStringCompletedEventArgs e)
        {

            try
            {
                // get a location object from the JSON returned:
                UserLocation location = JsonConvert.DeserializeObject<UserLocation>(e.Result);

                if (location.results.Length > 0)
                {
                    ParseUser.CurrentUser["country"] = location.results[0].address_components[3].long_name;
                    ParseUser.CurrentUser.SaveAsync();

                }
            }
            catch
            {
                // do nothing, the city/state/country just won't be set
            }

        }

    }
}

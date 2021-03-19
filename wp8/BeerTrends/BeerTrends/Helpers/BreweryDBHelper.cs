using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeerTrends.Models;
using BeerTrends.Helpers;
using System.Net;

namespace BeerTrends.Helpers
{
    public class BreweryDBHelper
    {
        // each request will start with this:
        private static string _base = "http://api.brewerydb.com/v2/";
        private static string _key = "?key=" + Constants.BreweryDBAPIKey + "&format=json";

        public delegate void RequestCompletedHandler(object sender, DownloadStringCompletedEventArgs e);
        public static event RequestCompletedHandler OnRequestCompleted;

        private static void RequestCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            OnRequestCompleted(sender, e);
        }

        public static void GetBeerList()
        {
            WebClient request = new WebClient();
            request.DownloadStringCompleted += RequestCompleted;
            Uri uri = new Uri(_base + "beers" + _key + "&p=1");
            request.DownloadStringAsync(uri);
        }

        public static void Search(string query)
        {
            WebClient request = new WebClient();
            request.DownloadStringCompleted += RequestCompleted;
            Uri uri = new Uri(_base + "search" + _key + "&type=beer&q=" + query);
            request.DownloadStringAsync(uri);
        }

        public static void GetBeerByIds(List<string> ids)
        {
            string idList = "";
            for (var i = 0; i < ids.Count(); i++)
            {
                idList += ids[i] + ",";
            }

            if (idList.EndsWith(",")) idList = idList.Substring(0, idList.Length - 1);

            WebClient request = new WebClient();
            request.DownloadStringCompleted += RequestCompleted;
            Uri uri = new Uri(_base + "beers" + _key + "&ids=" + idList);
            request.DownloadStringAsync(uri);
        }


    }
}

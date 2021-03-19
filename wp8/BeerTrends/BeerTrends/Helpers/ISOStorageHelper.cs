using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeerTrends.Helpers
{
    public class ISOStorageHelper
    {
        public static bool BeerIsFavorited(string beerId)
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;
            if (settings.Contains(Constants.Favorites) && settings[Constants.Favorites] != null)
            {
                List<string> favList = (List<string>)settings[Constants.Favorites];
                var thisBeer = favList.Where(b => b == beerId).FirstOrDefault();
                if (thisBeer != null)
                {
                    // beer already favorited:
                    return true;
                }
            }

            return false;
        }

        public static void AddFavorite(string beerId)
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;
            if (settings.Contains(Constants.Favorites) && settings[Constants.Favorites] != null)
            {
                List<string> favList = (List<string>)settings[Constants.Favorites];
                var thisBeer = favList.Where(b => b == beerId).FirstOrDefault();
                if (thisBeer == null)
                {
                    // beer wasn't in list, add it:
                    favList.Add(beerId);
                    settings.Save();
                }
            }
            else
            {
                // list didn't exist in settings, adding it:
                List<string> favList = new List<string>();
                favList.Add(beerId);
                settings[Constants.Favorites] = favList;
                settings.Save();
            }
        }

        public static void RemoveFavorite(string beerId)
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;
            if (settings.Contains(Constants.Favorites) && settings[Constants.Favorites] != null)
            {
                List<string> favList = (List<string>)settings[Constants.Favorites];
                var thisBeer = favList.Where(b => b == beerId).FirstOrDefault();
                if (thisBeer != null)
                {
                    favList.Remove(beerId);
                }
            }
           
            settings.Save();
        }
    }
}

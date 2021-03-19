using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Parse;
using Facebook;
using BeerTrends.Models;
using System.Windows;

namespace BeerTrends.Helpers
{
    public static class FacebookHelper
    {
        

        public static async void CheckForUpdates()
        {
            

            // if this user hasn't been updated for at least a week, update their data:
            if (DateTime.Now.AddDays(-7) > (DateTime)ParseUser.CurrentUser.UpdatedAt)
            {

                // Retrieve User's Facebook Info:
                var fb = new FacebookClient();
                fb.AccessToken = ParseFacebookUtils.AccessToken;
                var me = await fb.GetTaskAsync("me");

                // Convert JSON to our FB User object:
                FacebookUser fbUser = JsonConvert.DeserializeObject<FacebookUser>(me.ToString());

                // Populate the Parse object with the user's FB data:
                ParseUser.CurrentUser.Email = fbUser.email;
                ParseUser.CurrentUser.Username = fbUser.email;
                ParseUser.CurrentUser["firstName"] = fbUser.first_name;
                ParseUser.CurrentUser["lastName"] = fbUser.last_name;
                ParseUser.CurrentUser["dob"] = Convert.ToDateTime(fbUser.birthday);
                ParseUser.CurrentUser["displayName"] = fbUser.first_name;
                ParseUser.CurrentUser["gender"] = fbUser.gender;
                ParseUser.CurrentUser["platform"] = "Windows Phone 8";
                ParseUser.CurrentUser["zipCode"] = "";
                ParseUser.CurrentUser["city"] = fbUser.location.name.Split(',')[0];
                ParseUser.CurrentUser["state"] = fbUser.location.name.Split(',')[1];
                ParseUser.CurrentUser["country"] = "";
                ParseUser.CurrentUser["bio"] = "";
                ParseUser.CurrentUser["thumbnail"] = "";

                FacebookHelper.FinishFacebookSetup(fbUser.id, fb.AccessToken);

                // save the parse user:
                // await ParseUser.CurrentUser.SaveAsync();

                // look up the country:

                LocationHelper.UpdateUserLocationFromCityState(fbUser.location.name);

                // successfully signed up, save into Isolated Storage:
                //IsolatedStorageHelper.SaveUser(User.FromParseUser(ParseUser.CurrentUser));

            }

            
        }

        public static void FinishFacebookSetup(string userId, string token)
        {
            try
            {
                string url = "https://graph.facebook.com/" + userId + "/picture?type=normal&redirect=false&token=" + token;
                WebClient wc = new WebClient();
                wc.DownloadStringCompleted += wc_DownloadStringCompleted;
                wc.DownloadStringAsync(new Uri(url));
            }
            catch (Exception ex)
            {
                ParseUser.CurrentUser.SaveAsync();
            }
        }

        static void wc_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            string fbPicData = e.Result;
            FBPictureData fb = JsonConvert.DeserializeObject<FBPictureData>(fbPicData);
            ParseUser.CurrentUser["thumbnail"] = fb.data.url;
            ParseUser.CurrentUser.SaveAsync();
        }

    }

    public class FBPictureData
    {
        public Data data { get; set; }
    }

    public class Data
    {
        public string url { get; set; }
        public bool is_silhouette { get; set; }
    }

}

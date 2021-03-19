using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeerTrends.Models;
using Parse;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using System.Windows.Threading;
using System.IO;
using Newtonsoft.Json;

namespace BeerTrends.Helpers
{
    public class ParseHelper
    {
        // create an event so we can tell the page when the search request has completed:
        public delegate void GetFavoritesHandler(ParseObject result);
        public static event GetFavoritesHandler OnRetrievedFavorites;

        public delegate void UpdateFavoritesHandler(bool success, string result);
        public static event UpdateFavoritesHandler OnUpdateFavorite;

        public delegate void GotFavoritesHandler(bool success, List<string> result);
        public static event GotFavoritesHandler OnGotFavorites;

        private static void RetrievedFavorites(ParseObject result)
        {
            OnRetrievedFavorites(result);
        }

        private static void UpdateFavorite(bool success, string result)
        {
            OnUpdateFavorite(success, result);
        }

        private static void GotFavorites(bool success, List<string> result)
        {
            OnGotFavorites(success, result);
        }

        public static async void CreateNewAccount(string email, string password, string fullName, DateTime dob, string gender, string zip)
        {
            var parseUser = new ParseUser()
            {
                Username = email,
                Password = password,
                Email = email
            };

            // other fields:
            parseUser["fullName"] = fullName;
            //parseUser["lastName"] = lastName;
            parseUser["dob"] = dob;
            //parseUser["displayName"] = displayName;
            parseUser["gender"] = gender;
            parseUser["platform"] = "Windows Phone 8";
            parseUser["zipCode"] = zip;
            parseUser["city"] = ""; // will be updatd if zip was provided
            parseUser["state"] = ""; // will be updatd if zip was provided
            parseUser["thumbnail"] = "";

            try
            {
                // sign up:
                await parseUser.SignUpAsync();

                // look up their location based on the zip if it is provided:
                if (zip != "") LocationHelper.UpdateUserLocationFromZip(zip);

                // take them to the home page:
                (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/Pages/NewsFeed.xaml?FromLogin=true", UriKind.Relative));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.ToString());
            }
        }

        public static async void Login(string email, string password)
        {
            try
            {
                await ParseUser.LogInAsync(email, password);
                (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/Pages/NewsFeed.xaml?FromLogin=true", UriKind.Relative));
            }
            catch (Exception ex)
            {
                // The login failed. Check the error to see why.
                MessageBox.Show("Error logging in. Please double check your email and password or try again later.");
            }
        }

        public static void Logout()
        {
            if (ParseUser.CurrentUser != null) ParseUser.LogOut();
        }

        public static async Task<bool> UpdateUserProfilePic(Stream photo)
        {
            // create the parse file to hold the image:
            ParseFile file = new ParseFile("thumbnail.jpg", photo);

            // save the new file:
            await file.SaveAsync();

            // create a new object:
            var thumbnail = new ParseObject("Thumbnails");
            thumbnail["thumbnail"] = file;
            await thumbnail.SaveAsync();

            // did the user already have a parse file that we need to delete?
            var prevThumbnail = ParseUser.CurrentUser["thumbnail"].ToString();
            if (prevThumbnail != "")
            {
                if (!prevThumbnail.ToLower().StartsWith("http"))
                {
                    // parse thumbnail id
                    // we have an id, look up this parse object/file and delete:
                    ParseQuery<ParseObject> query = ParseObject.GetQuery("Thumbnails");
                    ParseObject prevObject = await query.GetAsync(prevThumbnail);
                    await prevObject.DeleteAsync();
                }

            }

            // link it to the current user:
            ParseUser.CurrentUser["thumbnail"] = thumbnail.ObjectId;

            // save the user:
            await ParseUser.CurrentUser.SaveAsync();

            return true;

        }

        public static async void BeerIsFavorited(string beerId)
        {
            try
            {
                var favs = ParseUser.CurrentUser.GetRelation<ParseObject>("Favorites");

                var query = from fav in favs.Query
                            where fav["beerId"] == beerId
                            select fav;

                ParseObject poFav = await query.FirstAsync();
                RetrievedFavorites(poFav);
            }
            catch
            {
                RetrievedFavorites(null);
            }

           
        }

        public static async void FavoriteBeer(string beerId)
        {

            try
            {
                var newFav = new ParseObject("Favorites")
                {
                    { "beerId", beerId },
                    { "userId", ParseUser.CurrentUser.ObjectId }
                };

                await newFav.SaveAsync();
                
                var relation = ParseUser.CurrentUser.GetRelation<ParseObject>("Favorites");
                relation.Add(newFav);
                await ParseUser.CurrentUser.SaveAsync();
                UpdateFavorite(true, "Successfully added to Favorites!");
            }
            catch(Exception ex)
            {
                UpdateFavorite(false, "Error adding favorite - please try again later");
            }
        }

        public static async void UnfavoriteBeer(string beerId)
        {
            try
            {
                var query = from fav in ParseObject.GetQuery("Favorites")
                            where fav.Get<string>("userId").Equals(ParseUser.CurrentUser.ObjectId) && fav.Get<string>("beerId").Equals(beerId)
                            select fav;

                var thisFav = await query.FirstAsync();

                await thisFav.DeleteAsync();
                UpdateFavorite(true, "Successfully removed from Favorites");
            }
            catch(Exception ex)
            {
                UpdateFavorite(false, "Error removing favorite - please try again later");
            }
        }

        public static async void GetFavorites()
        {
            try
            {

                var favQuery = from fav in ParseObject.GetQuery("Favorites")
                            where fav.Get<string>("userId").Equals(ParseUser.CurrentUser.ObjectId)
                            select fav;

                IEnumerable<ParseObject> favs = await favQuery.FindAsync();

                List<string> ids = new List<string>();

                for (var i = 0; i < favs.Count(); i++)
                {
                    ids.Add(favs.ElementAt(i).Get<string>("beerId"));
                }

                OnGotFavorites(true, ids);
            }
            catch (Exception ex)
            {
                OnGotFavorites(false, null);
            }
        }

      
    }
}

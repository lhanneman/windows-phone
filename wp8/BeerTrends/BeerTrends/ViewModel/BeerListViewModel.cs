using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeerTrends.Models;
using System.ComponentModel;
using System.Collections.ObjectModel;
using BeerTrends.Helpers;
using Newtonsoft.Json;

namespace BeerTrends.ViewModel
{
    public class BeerListViewModel
    {
        public ObservableCollection<Beer> Beers { get; set; }

        // create an event so we can tell the page when the search request has completed:
        public delegate void RequestCompleteHandler(BreweryDBResult result);
        public event RequestCompleteHandler OnRequestCompleted;

        public BeerListViewModel()
        {
            Beers = new ObservableCollection<Beer>();
        }

        private void RequestCompleted(BreweryDBResult result)
        {
            OnRequestCompleted(result);
        }

        public void Search(string query)
        {
            BreweryDBHelper.OnRequestCompleted += GotResults;
            BreweryDBHelper.Search(query);
        }

        public void GetFavorites()
        {
            ParseHelper.OnGotFavorites += ParseHelper_OnGotFavorites;
            ParseHelper.GetFavorites();
        }

        void ParseHelper_OnGotFavorites(bool success, List<string> result)
        {
            ParseHelper.OnGotFavorites -= ParseHelper_OnGotFavorites;
            BreweryDBHelper.OnRequestCompleted += GotResults;
            BreweryDBHelper.GetBeerByIds(result);
        }

        void GotResults(object sender, System.Net.DownloadStringCompletedEventArgs e)
        {
            BreweryDBHelper.OnRequestCompleted -= GotResults;
            Beers.Clear();
            BreweryDBResult result = JsonConvert.DeserializeObject<BreweryDBResult>(e.Result);

            for (var i = 0; i < result.data.Count(); i++)
            {
                Beers.Add(result.data[i]);
            }


            RequestCompleted(result);
        }


    }
}

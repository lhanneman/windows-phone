using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeerTrends.Models;

namespace BeerTrends.ViewModel
{
    public class BeerViewModel
    {
         public Beer Beer { get; set; }

        // create an event so we can tell the page when the search request has completed:
        //public delegate void DoneSearchingHandler(BreweryDBResult result);
        //public event DoneSearchingHandler OnSearchCompleted;

        public BeerViewModel()
        {
            //Beers = new ObservableCollection<Beer>();
        }

        //private void SearchCompleted(BreweryDBResult result)
        //{
        //    OnSearchCompleted(result);
        //}

        //public void Search(string query)
        //{
        //    BreweryDBHelper.OnRequestCompleted += GotSearchResults;
        //    BreweryDBHelper.Search(query);
        //}

        //void GotSearchResults(object sender, System.Net.DownloadStringCompletedEventArgs e)
        //{
        //    BreweryDBHelper.OnRequestCompleted -= GotSearchResults;
        //    Beers.Clear();
        //    BreweryDBResult result = JsonConvert.DeserializeObject<BreweryDBResult>(e.Result);

        //    for (var i = 0; i < result.data.Count() ; i++)
        //    {
        //        Beers.Add(result.data[i]);
        //    }
        //    SearchCompleted(result);
        //}
    }
}

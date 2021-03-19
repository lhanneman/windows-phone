using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeerTrends.Models
{


    public class BreweryDBResult
    {
        public int currentPage { get; set; }
        public int numberOfPages { get; set; }
        public int totalResults { get; set; }
        public List<Beer> data { get; set; }
    }

    public class Beer
    {
        public Beer()
        {
            available = new Available();
            style = new Style();
            labels = new Labels();
            glass = new Glass();
        }

        // check "possible return fields" here: http://www.brewerydb.com/developers/docs-endpoint/beer_index 

        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string foodPairings { get; set; }
        public string originalGravity { get; set; }
        public string glasswareId { get; set; }
        public Glass glass { get; set; }
        public string servingTemperature { get; set; }
        public string servingTemperatureDisplay { get; set; }
        public string abv { get; set; }
        public string ibu { get; set; }
        public int availableId { get; set; }
        public int styleId { get; set; }
        public string isOrganic { get; set; }
        public string status { get; set; }
        public string statusDisplay { get; set; }
        public string createDate { get; set; }
        public string updateDate { get; set; }
        public Available available { get; set; }
        public Style style { get; set; }
        public Labels labels { get; set; }
        public string beerVariationId { get; set; }
        public BeerVariation beerVariation { get; set; }
        public string year { get; set; }
    }

    public class Glass
    {
        public string id { get; set; }
        public string name { get; set; }
        public string createDate { get; set; }
    }

    public class Labels
    {
        public Labels()
        {
            icon = "/Assets/Images/Beer/Icon.png";
            medium = "/Assets/Images/Beer/Medium.png";
            large = "/Assets/Images/Beer/Large.png";
        }
        public string icon { get; set; }
        public string medium { get; set; }
        public string large { get; set; }
    }

    public class Available
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
    }

    public class Style
    {
        public int id { get; set; }
        public int categoryId { get; set; }
        public Category category { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string ibuMin { get; set; }
        public string ibuMax { get; set; }
        public string abvMin { get; set; }
        public string abvMax { get; set; }
        public string srmMin { get; set; }
        public string srmMax { get; set; }
        public string ogMin { get; set; }
        public string fgMin { get; set; }
        public string fgMax { get; set; }
        public string createDate { get; set; }
    }

    public class Category
    {
        public int id { get; set; }
        public string name { get; set; }
        public string createDate { get; set; }
    }

    public class BeerVariation
    {

    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeerTrends.Models
{

    public class UserLocation
    {
        public Result[] results { get; set; }
        public string status { get; set; }
    }

    public class Result
    {
        public Address_Components[] address_components { get; set; }
        public string formatted_address { get; set; }
        public Geometry geometry { get; set; }
        public string[] types { get; set; }
    }

    public class Geometry
    {
        public Bounds bounds { get; set; }
        public Location location { get; set; }
        public string location_type { get; set; }
        public Viewport viewport { get; set; }
    }

    public class Bounds
    {
        public Bounds_Northeast northeast { get; set; }
        public Bounds_Southwest southwest { get; set; }
    }

    public class Bounds_Northeast
    {
        public float lat { get; set; }
        public float lng { get; set; }
    }

    public class Bounds_Southwest
    {
        public float lat { get; set; }
        public float lng { get; set; }
    }

    public class Location_LatitudeLongitude
    {
        public float lat { get; set; }
        public float lng { get; set; }
    }

    public class Viewport
    {
        public Viewport_Northeast northeast { get; set; }
        public Viewport_Southwest southwest { get; set; }
    }

    public class Viewport_Northeast
    {
        public float lat { get; set; }
        public float lng { get; set; }
    }

    public class Viewport_Southwest
    {
        public float lat { get; set; }
        public float lng { get; set; }
    }

    public class Address_Components
    {
        public string long_name { get; set; }
        public string short_name { get; set; }
        public string[] types { get; set; }
    }


}

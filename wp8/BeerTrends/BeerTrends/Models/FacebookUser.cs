using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeerTrends.Models
{

    public class FacebookUser
    {
        public string id { get; set; }
        public string name { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string link { get; set; }
        public string username { get; set; }
        public string birthday { get; set; }
        public Location location { get; set; }
        public string quotes { get; set; }
        public Work[] work { get; set; }
        public string gender { get; set; }
        public string email { get; set; }
        public int timezone { get; set; }
        public string locale { get; set; }
        public bool verified { get; set; }
        public DateTime updated_time { get; set; }
    }

    public class Location
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class Work
    {
        public Employer employer { get; set; }
        public WorkLocation location { get; set; }
        public Position position { get; set; }
        public string description { get; set; }
        public string start_date { get; set; }
    }

    public class Employer
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class WorkLocation
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class Position
    {
        public string id { get; set; }
        public string name { get; set; }
    }


}

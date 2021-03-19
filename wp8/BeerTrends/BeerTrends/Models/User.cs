using Parse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeerTrends.Models
{
    // this is a PARSE user
    public class User
    {
        // readonlies:
        //private string _objectId { get; set; }
        //private bool? _emailVerified { get; set; }
        //private DateTime? _createdAt { get; set; }
        //private DateTime? _updatedAt { get; set; }

        // public:
        public string objectId { get; set; }
        public string username { get; set; }
        public bool? emailVerified { get; set; }
        public string displayName { get; set; }
        public DateTime dob { get; set; }
        public string email { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string gender { get; set; }
        public string zipCode { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public string bio { get; set; }
        public string thumbnail { get; set; } 
        public string platform { get; set; }
        public DateTime? createdAt { get; set; }
        public DateTime? updatedAt { get; set; }

        //public authData authData { get; set; }// needed for FB and Twitter auth

        //// translates a ParseUser into a BeerTrends User object:
        //public static User FromParseUser(ParseUser pu)
        //{
        //    // TODO: update this to use reflection and loop through all automatically?

        //    User user = new User();
        //    user.objectId = pu.ObjectId;
        //    user.username = pu.Username;
        //    user.email = pu.Email;
        //    user.createdAt = pu.CreatedAt;
        //    user.updatedAt = pu.UpdatedAt;

        //    // custom fields:
        //    user.emailVerified = (pu.ContainsKey("emailVerified") ? (bool?)pu["emailVerified"] : null);
        //    user.displayName = (string)pu["displayName"];
        //    user.dob = (DateTime)pu["dob"];
        //    user.firstName = (string)pu["firstName"];
        //    user.lastName = (string)pu["lastName"];
        //    user.gender = (string)pu["gender"];
        //    user.platform = (string)pu["platform"];
        //    user.zipCode = (string)pu["zipCode"];
        //    user.city = (string)pu["city"];
        //    user.state = (string)pu["state"];
        //    user.country = (string)pu["country"];
        //    user.bio = (string)pu["bio"];
        //    user.thumbnail = (string)pu["thumbnail"];
            
            
        //    return user;

        //}

    }

    public class authData { }
}

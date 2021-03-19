using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeerTrends.Models
{
    public class Settings
    {
        public bool darkTheme { get; set; }
    }

    // default values for first time use:
    public class DefaultSettings : Settings
    {
        public DefaultSettings() {
            darkTheme = true;
        }
    }
}

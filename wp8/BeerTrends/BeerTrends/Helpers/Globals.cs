using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BeerTrends.Helpers
{
    class Globals
    {
        public static bool IsDarkTheme
        {
            get 
            {
                Visibility darkBackgroundVisibility = (Visibility)Application.Current.Resources["PhoneDarkThemeVisibility"];
                return darkBackgroundVisibility == Visibility.Visible;
            }
        }
    }
}

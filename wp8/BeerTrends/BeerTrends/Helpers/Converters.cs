using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace BeerTrends.Converters
{
    public class VisibilityConverter : IValueConverter
    {

        // found here: http://dotnetbyexample.blogspot.com/2010/11/converter-for-showinghiding-silverlight.html

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null) return Visibility.Visible;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BeerPropertyConverter : IValueConverter
    {

        // found here: http://dotnetbyexample.blogspot.com/2010/11/converter-for-showinghiding-silverlight.html

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {


            if (value == null || parameter == null) return "";
            
            switch (parameter.ToString().ToLower())
            {
                
                case "abv":
                    return value + "%";
                case "isorganic":
                    if (value.ToString().ToLower() == "n")
                        return "No";
                    else
                        return "Yes";
                default:
                    return value;
            }


        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

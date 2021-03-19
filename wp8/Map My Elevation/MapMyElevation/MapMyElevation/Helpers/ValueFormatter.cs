using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapMyElevation.Resources;
using MapMyElevation.Helpers;

namespace MapMyElevation.Helpers
{
    public static class ValueFormatter
    {
        public static string GetElevationFormatted(double? altitude, int elevationUnit)
        {

            if (altitude != null)
            {
                double altitudeInMeters = (double)altitude;
                
                switch (elevationUnit)
                {
                    case (int)Enumerations.ElevationUnit.Miles:
                        double miles = (0.000621371 * altitudeInMeters);
                        return miles.ToString("0.00") + " " + AppResources.Miles.ToLower();
                    case (int)Enumerations.ElevationUnit.Kilometers:
                        double kilos = (0.001 * altitudeInMeters);
                        return kilos.ToString("0.00") + " " + AppResources.KilometersAbbreviation;
                    case (int)Enumerations.ElevationUnit.Meters:
                        // value is already in meters:
                        return altitudeInMeters.ToString("0") + " " + AppResources.Meters.ToLower();
                    default:
                        break;

                }

            }

            return AppResources.NotAvailable;
        }

        public static string GetDistanceFormatted(double distanceInMeters, int distanceUnit, bool forChart)
        {
            switch (distanceUnit)
            {
                case (int)Enumerations.DistanceUnit.Miles:
                    double miles = (0.000621371 * distanceInMeters);
                    if (forChart) return miles.ToString("0.0") + AppResources.M;
                    // not for chart:
                    return miles.ToString("0.00") + " " + AppResources.Miles.ToLower();
                case (int)Enumerations.DistanceUnit.Kilometers:
                    double kilos = (0.001 * distanceInMeters);
                    if (forChart) return kilos.ToString("0.0") + AppResources.KM;
                    // not for chart:
                    return kilos.ToString("0.00") + " " + AppResources.KilometersAbbreviation;
                case (int)Enumerations.DistanceUnit.Meters:
                    // value is already in meters:
                    if (forChart) return distanceInMeters.ToString() + AppResources.M;
                    // not for chart:
                    return distanceInMeters.ToString() + " " + AppResources.Meters.ToLower();
                default:
                    break;
            }

            return AppResources.NotAvailable;
        }
    }
}

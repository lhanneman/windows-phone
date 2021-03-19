using System;
using System.Collections.Generic;
//using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace MapMyElevation.Helpers
{
    public class SummaryData
    {
       // public List<GeoCoordinate> Coordinates { get; set; }

        private List<ChartData> _chartData;

        public List<ChartData> ChartData { get { return _chartData; } }
        
        public double TotalDistanceInMeters { get; set; }
        public TimeSpan TotalTime { get; set; }
        public double? LowestElevation { get; set; }
        public double? HighestElevation { get; set; }
        public double? AverageElevation { get; set; }
        

        public SummaryData(double distanceInMeters, TimeSpan time, List<ChartData> chartData)
        {

            _chartData = chartData;
            List<Geocoordinate> coords = new List<Geocoordinate>();
            for (int i = 0; i < _chartData.Count(); i++)
            {
                coords.Add(_chartData[i].Coordinate);
            }

            TotalDistanceInMeters = (distanceInMeters > 0) ? distanceInMeters : 0;
            TotalTime = time != null ? time : new TimeSpan();

            if (coords != null && coords.Count() > 0)
            {

                // look up the lowest & highest altitude in the collection:
                foreach (Geocoordinate gc in coords)
                {
                    // first time through, set the defaults::
                    if (LowestElevation == null && gc.Altitude > 0) LowestElevation = gc.Altitude;
                    if (HighestElevation == null) HighestElevation = gc.Altitude;

                    // if higher/lower than the previous, update accordingly:
                    if (gc.Altitude < LowestElevation) LowestElevation = gc.Altitude;
                    if (gc.Altitude > HighestElevation) HighestElevation = gc.Altitude;
                }

                // calculate the average altitude:
                AverageElevation = coords.Sum(c => c.Altitude) / coords.Count();
            }
        }
    }

    public class ChartData
    {
        //public GeoCoordinate Coordinate { get; set; }
        public Geocoordinate Coordinate { get; set; }
        public string CurrentDistance { get; set; }
        public double? Elevation { get { return Coordinate.Altitude; } }

        public ChartData() { }

        public ChartData(Geocoordinate gc, double currentDistanceInMeters, int distanceUnit)
        {
            Coordinate = gc;
            CurrentDistance = ValueFormatter.GetDistanceFormatted(currentDistanceInMeters, distanceUnit, true);
        }
    }
}

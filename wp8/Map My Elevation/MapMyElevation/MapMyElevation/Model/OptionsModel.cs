using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace MapMyElevation.Model
{
    public class OptionsModel : INotifyPropertyChanged
    {

        private Helpers.Enumerations.ElevationUnit _elevationUnit;
        public int ElevationUnit
        {
            get { return (int)_elevationUnit; }
            set 
            { 
                _elevationUnit = (Helpers.Enumerations.ElevationUnit)value;
                RaisePropertyChanged("ElevationUnit");
            }
        }

        private Helpers.Enumerations.DistanceUnit _distanceUnit;
        public int DistanceUnit
        {
            get { return (int)_distanceUnit; }
            set
            {
                _distanceUnit = (Helpers.Enumerations.DistanceUnit)value;
                RaisePropertyChanged("DistanceUnit");
            }

        }


        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                UserSettings.SaveOptions(this);
            }
        }
    }
}

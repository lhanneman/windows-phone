using System;
using System.Windows;
using System.Collections.ObjectModel;
using System.IO.IsolatedStorage;
using MapMyElevation.Model;

namespace MapMyElevation.ViewModel
{
    public class OptionsViewModel
    {
        public OptionsModel Options { get; set; }

        public void GetOptions()
        {
            Options = UserSettings.GetUserSettings().Options;
        }
    }
}

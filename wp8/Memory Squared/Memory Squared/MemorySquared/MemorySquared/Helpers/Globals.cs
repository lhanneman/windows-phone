using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace MemorySquared
{

    public static class Globals
    {
        // VERSION NUMBER:
        public const string sErrorEmailTo = "memorysquared@gmail.com";

        public static GameData Data;
                
        // LISTS
        public static List<int> userTapSequence = new List<int>();
        public static List<int> currentSequence = new List<int>(); // keeps track of the current order of rectangles to match

        // INTS
        public static int numberOfRectangles;
        public static int iCurrentlyDisplayingSequenceIndex = 0;
        public static int nSequenceCounter;

        // BOOLS
        public static bool bCurrentlyDisplayingSequence = false;
        public static bool bGameIsStarted = false; // will be true when the game has been started
        public static bool bGameIsPaused = false; // will be true when the game is paused
        public static bool bAllRectanglesAdded = false; // will be true once we add the 8 rectangles to the grid
        public static bool bDisplayAds = true;
        public static bool bDeactivated = false;
       
        //true only when we are displaying the last sequence when the user resumes the game after it was paused
        //this is how we can know to NOT display another random rectangle after the current sequence, because
        // we want the user to get this sequence right first!
        public static bool bJustResumedGame = false;

        public static bool bIncorrect = false;
        
        public static double WVGA_Width = 480;
        public static double WVGA_Height = 800;
        public static double WXGA_Width = 480;
        public static double WXGA_Height = 1280;
        public static double SEVEN_TWENTY_P_Width = 720;
        public static double SEVEN_TWENTY_P_Height = 1280;

        public static string VersionNumber()
        {
            var info = new AssemblyName(Assembly.GetExecutingAssembly().FullName);
            string version = info.Version.Major + "." + info.Version.MajorRevision + "." + info.Version.Minor + "." + info.Version.MinorRevision;
            return version;
        }

        public static bool bShowSequenceUponResume
        {
            get
            {
                bool bShow = false;
                if (PhoneApplicationService.Current.State.Keys.Contains("MemorySquared_ShowSequenceUponResume"))
                {
                    if (bool.TryParse(PhoneApplicationService.Current.State["MemorySquared_ShowSequenceUponResume"].ToString(), out bShow) == true)
                    {
                        return bShow;
                    }
                }
                return bShow;
            }
            set
            {
                PhoneApplicationService.Current.State["MemorySquared_ShowSequenceUponResume"] = value;
            }
        }

        public static Color HardRectRemoveEndColor(Visibility darkBackground)
        {
            if (darkBackground == Visibility.Visible)
                return Color.FromArgb(255,28,28,28);//#1C1C1C
            else
                return Colors.Transparent;
        }

        public static Color RectangleColor(Visibility darkBackground)
        {
            if (darkBackground == Visibility.Visible)
                return Colors.Black;
            else
                return Colors.Transparent; 
        }

        public static Color BorderColor(Visibility darkBackground)
        {
            if (darkBackground == Visibility.Visible)
                return Colors.Transparent;
            else
                return Colors.Black; 
        }

        public static Color CountdownColor(Visibility darkBackground)
        {
            if (darkBackground == Visibility.Visible)
                return Colors.White;
            else
                return Colors.Black; 
        }

        public static Color ListPickerBackgroundColor(Visibility darkBackground)
        {
            if (darkBackground == Visibility.Visible)
                return Colors.Black;
            else
                return Colors.White; 
        }
    }
}


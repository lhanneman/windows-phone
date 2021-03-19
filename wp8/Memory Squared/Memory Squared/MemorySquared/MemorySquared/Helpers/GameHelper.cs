using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Shell;

namespace MemorySquared
{
    public static class GameHelper
    {
        public static void PauseGame(bool bShowSequenceUponResume)
        {
            
            // SWTICH THE SWITCHES:
            Globals.bGameIsPaused = true;
            Globals.bCurrentlyDisplayingSequence = false;

            // SAVE THE CURRENT SEQUENCE:
            PhoneApplicationService.Current.State["MemorySquared_CurrentSequence"] = Globals.currentSequence;
            PhoneApplicationService.Current.State["MemorySquared_SequenceCounter"] = Globals.nSequenceCounter;

            // DO WE NEED TO SHOW THE SEQUENCE ONCE THEY TAP RESUME?
            Globals.bShowSequenceUponResume = bShowSequenceUponResume;

        }
    }
}

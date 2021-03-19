using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Advertising.Mobile.UI;

namespace MemorySquared
{
    public static class Advertising
    {

        public static void GetAdForGrid(Grid grid, int nRow, int nCol, int nColSpan)
        {
           
#if DEBUG
            // use test ads when debugging:
            AdControl adControl = new AdControl("test_client", "Image480_80", true);
#else
            // user real ad control when in production:
            AdControl adControl = new AdControl("dc4505ab-798c-48ed-a7d6-2c316de4c50d", "123763", true);
#endif

            // Make the AdControl size large enough that it can contain the image
            adControl.Width = 480;
            adControl.Height = 80;

            // PLACE THE AD ON THE CANVAS:
            grid.Children.Add(adControl);
                      
            Grid.SetRow(adControl, nRow);
            Grid.SetColumn(adControl, nCol);

            if(nColSpan > 0) Grid.SetColumnSpan(adControl, nColSpan);
        }
    }
}

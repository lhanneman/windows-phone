/* 
    Copyright (c) 2012 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
#if DEBUG
using MockIAPLib;
using Store = MockIAPLib;
using System.Collections.ObjectModel;
#else
using Store = Windows.ApplicationModel.Store;
using System.Collections.ObjectModel;
using Windows.ApplicationModel.Store;
#endif

namespace MemorySquared
{
    public class StoreManager
    {
        public Dictionary<string, string> StoreItems = new Dictionary<string, string>();
        public static ObservableCollection<PurchaseItem> purchasedItems = new ObservableCollection<PurchaseItem>();

        public StoreManager()
        {
            // Populate the store
            StoreItems.Add("RemoveAds", "Remove Ads");
        }

        public async Task<List<string>> GetOwnedItems()
        {
            List<string> items = new List<string>();

            try
            {
                ListingInformation li = await CurrentApp.LoadListingInformationAsync();

                foreach (string key in li.ProductListings.Keys)
                {
                    if (CurrentApp.LicenseInformation.ProductLicenses[key].IsActive && StoreItems.ContainsKey(key))
                        items.Add(StoreItems[key]);
                }

            }
            catch
            {
                // do nothing - just return the empty list
            }

            return items;
        }

        public static async void GetPurchasedItems()
        {
            StoreManager mySM = new StoreManager();
            List<string> li = await mySM.GetOwnedItems();

            foreach (string listItem in li)
            {
                if (listItem != string.Empty)
                {
                    purchasedItems.Add(new PurchaseItem { imgLink = listItem });
                }
            }

            //pics.ItemsSource = purchasedItems;
        }

        public static void CheckForAds()
        {
            string productID = "RemoveAds";
            StoreManager.GetPurchasedItems();

            if (Store.CurrentApp.LicenseInformation.ProductLicenses[productID].IsActive)
            {
                Globals.bDisplayAds = false;
            }
        }

    }

    public class PurchaseItem
    {
        public string imgLink { get; set; }
        public string Status { get; set; }
        public string key { get; set; }
    }
}

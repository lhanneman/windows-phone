using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Text.RegularExpressions;
using BeerTrends.Models;
using BeerTrends.Helpers;
using Parse;

namespace BeerTrends.Pages
{
    public partial class NewAccount : PhoneApplicationPage
    {

        public NewAccount()
        {
            InitializeComponent();
           // ContentPanel.Loaded += ContentPanel_Loaded;
            btnCreate.Tap += btnCreate_Tap;
        }

        //void ContentPanel_Loaded(object sender, RoutedEventArgs e)
       // {
        
       // }

        void btnCreate_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            // creating new account, verify info first:
            if (txtName.Text == "")
            {
                MessageBox.Show("Please enter your full name");
                return;
            }
            else if (!ValidChars(txtName.Text))
            {
                MessageBox.Show("Only use alphanumeric characters for full name");
                return;
            }


            // come back to age verification:
            if (!VerifyAge(dpDob.Value))
            {
                MessageBox.Show("You must be 21 years old to use this app");
                return;
            }

            if (txtEmail.Text == "")
            {
                MessageBox.Show("Please enter your email address"); 
                return;
            }

            if (txtEmail.Text.Contains("@") == false)
            {
                MessageBox.Show("Please enter a valid email address"); 
                return;
            }

            if (txtPassword.Password.Length < 6)
            {
                MessageBox.Show("Password must be at least 6 characters");
                return;
            }

            // get the gender from the list picker:
            ListPickerItem selectedGender = (ListPickerItem)lpGender.SelectedItem;
            string gender = selectedGender.Content.ToString();
            if (gender == "Choose a gender...") gender = "";


            // passed validation, create the account:
            ParseHelper.CreateNewAccount(txtEmail.Text, txtPassword.Password, txtName.Text,(DateTime)dpDob.Value, gender, txtZip.Text);
        }

        private bool VerifyAge(DateTime? dob)
        {
            // convert to non-nullable date time:
            DateTime dobTemp = (DateTime)dob;

            DateTime min = DateTime.Now.AddYears(-21);
            if (DateTime.Compare(min, dobTemp) < 1) return false;
            return true;
        }

        private bool ValidChars(string val)
        {
            Regex rgx = new Regex("[^a-zA-Z0-9] -");
            if (rgx.IsMatch(val)) return false;
            return true;
        }

        private void PasswordLostFocus(object sender, RoutedEventArgs e)
        {
            var passwordEmpty = string.IsNullOrEmpty(txtPassword.Password);
            PasswordWatermark.Visibility = passwordEmpty ? Visibility.Visible : Visibility.Collapsed;
            txtPassword.Visibility = passwordEmpty ? Visibility.Collapsed : Visibility.Visible;
        }

        private void PasswordGotFocus(object sender, RoutedEventArgs e)
        {
            PasswordWatermark.Visibility = Visibility.Collapsed;
            txtPassword.Visibility = Visibility.Visible;
            txtPassword.Focus();
        }
    }
}
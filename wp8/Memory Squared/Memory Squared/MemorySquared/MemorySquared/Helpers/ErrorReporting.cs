using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Phone.Tasks;
using MemorySquared.Resources;

namespace MemorySquared
{
    public class ErrorReporting
    {
        const string sFileName = @"MemorySquared\Error.txt";

        internal static void ReportException(Exception ex, string extra)
        {
            try
            {
                using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                {

                    SafeDeleteFile(store);
                    
                    using (TextWriter output = new StreamWriter(store.CreateFile(sFileName)))
                    {
                        output.WriteLine("Extra: " + extra);
                        output.WriteLine(ex.Message);
                        output.WriteLine(ex.StackTrace);
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        internal static void CheckForPreviousException()
        {
            try
            {
                string contents = null;
                using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (store.FileExists(sFileName))
                    {
                        using (TextReader reader = new StreamReader(store.OpenFile(sFileName, FileMode.Open, FileAccess.Read, FileShare.None)))
                        {
                            contents = reader.ReadToEnd();
                        }
                        SafeDeleteFile(store);
                    }
                }

                if (contents != null)
                {
                    if (MessageBox.Show(AppResources.ErrorLastTimeMessage, AppResources.ErrorReport, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        EmailComposeTask email = new EmailComposeTask();
                        email.To = Globals.sErrorEmailTo;
                        email.Subject = AppResources.ErrorReport;
                        email.Body = contents;
                        SafeDeleteFile(IsolatedStorageFile.GetUserStoreForApplication());
                        email.Show();
                    }
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                SafeDeleteFile(IsolatedStorageFile.GetUserStoreForApplication());
            }
        }

        private static void SafeDeleteFile(IsolatedStorageFile store)
        {
            try
            {
                // create the MemorySquared directory:
                if (store.DirectoryExists("MemorySquared") == false)
                {
                    store.CreateDirectory("MemorySquared");
                }

                store.DeleteFile(sFileName);
            }
            catch (Exception)
            {
            }
        }

        public static void SendFeedback()
        {
            EmailComposeTask email = new EmailComposeTask();
            email.To = Globals.sErrorEmailTo;
            email.Subject = "Memory Squared Feedback";
            email.Show();
        }

        public static void SendErrorDetails(string sDetails)
        {
            EmailComposeTask email = new EmailComposeTask();
            email.To = Globals.sErrorEmailTo;
            email.Subject = "Memory Squared Error";
            email.Body = sDetails;
            email.Show();
        }

    }
}

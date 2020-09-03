using PastebinAPI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Quasar.Internal.Tools
{
    public static class Pasterino
    {
        public static async void newPaste(string error, string details, string stack)
        {
            Pastebin.DevKey = Properties.Settings.Default.Mada;
            Paste pat = await Paste.CreateAsync(
                String.Format("Message : {0} \r\n Source : {1} \r\n, StackTrace : {2}", error,details, stack),
                "Quasar Log v" + Properties.Settings.Default.AppVersion,
                Language.CSharp, 
                PastebinAPI.Visibility.Unlisted, 
                Expiration.OneWeek);
            Process.Start(pat.Url);
        }

        public static void sendPaste(Exception e)
        {
            if (Properties.Settings.Default.EnablePastebin)
            {
                bool proceed = false;
                MessageBoxResult result = MessageBox.Show("An error occured with Quasar. Would you like to create a Pastebin log containing its details ?", "Error", MessageBoxButton.YesNo);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        proceed = true;
                        break;
                    case MessageBoxResult.No:
                        break;
                }
                if (proceed)
                {
                    newPaste(e.Message, e.Source, e.StackTrace);
                }
            }
            
        }
    }
}

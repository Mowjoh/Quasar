using Quasar.Singleton;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Quasar.Quasar_Sys
{
    static class Checker
    {
        public static Mutex Instances(Mutex _serverMutex,ObservableCollection<string> _DLS)
        {
            PipeClient Pc_principal;
            string[] Args = System.Environment.GetCommandLineArgs();
            //Checking if Quasar is running alright
            if (Mutex.TryOpenExisting("Quasarite", out Mutex mt))
            {
                //Client
                if (Args.Length == 2)
                {
                    Pc_principal = new PipeClient("Quasarite", Args[1]);
                }
                mt.Close();
                Application.Current.Shutdown();
            }
            else
            {
                //Server
                _serverMutex = new Mutex(true, "Quasarite");
                if (Args.Length == 2)
                {
                    new PipeServer("Quasarite", _DLS, Args[1]);
                }
                else
                {
                    new PipeServer("Quasarite", _DLS, "");
                }

            }

            return _serverMutex;
        }

        public static void FirstRun()
        {
            if (Properties.Settings.Default.FirstRun == "True")
            {
                //Getting system language
                CultureInfo ci = CultureInfo.InstalledUICulture;
                Properties.Settings.Default.Language = (String)ci.Name;

                //Getting User's Documents folder for storage purposes
                String DocumentsFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                DocumentsFolderPath += "\\Quasar";
                Properties.Settings.Default.DefaultDir = DocumentsFolderPath;

                //Getting execution path
                String AppPath = Environment.GetCommandLineArgs()[0];
                Properties.Settings.Default.AppPath = System.IO.Path.GetDirectoryName(AppPath);

                //Setting finished flag
                Properties.Settings.Default.FirstRun = "False";

                Properties.Settings.Default.Save();
            }
        }
    }
}

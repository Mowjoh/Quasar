using Quasar.FileSystem;
using Quasar.Quasar_Sys;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quasar.Internal.Tools
{
    public class Updater
    {
        public Updater()
        {
            bool Update = Checker.CheckQuasarUpdated();
            Folderino.CheckBaseFolders();
            Folderino.CompareReferences();
            Folderino.UpdateBaseFiles();

            int version = int.Parse(Properties.Settings.Default.AppVersion);
            int previous = int.Parse(Properties.Settings.Default.PreviousVersion);
            if (Update && version >= 1140 && previous < 1140)
            {
                String AssociationsPath = Properties.Settings.Default.DefaultDir + @"\Library\Associations.xml";
                String ContentPath = Properties.Settings.Default.DefaultDir + @"\Library\ContentMapping.xml";

                if (File.Exists(AssociationsPath))
                {
                    File.Delete(AssociationsPath);
                }
                if (File.Exists(ContentPath))
                {
                    File.Delete(ContentPath);
                }

            }

            Checker.BaseWorkspace();

            //Setting language
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(Properties.Settings.Default.Language);
        }
    }
}

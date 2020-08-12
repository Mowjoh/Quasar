using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Quasar.Internal.Tools
{
    static class IDGenerator
    {
        public static int getNewContentID()
        {
            int ID = Properties.Settings.Default.ContentID;
            ID++;

            Properties.Settings.Default.ContentID = ID;
            Properties.Settings.Default.Save();

            return ID;
        }

        public static int getNewWorkspaceID()
        {
            int ID = Properties.Settings.Default.WorkspaceID;
            ID++;

            Properties.Settings.Default.WorkspaceID = ID;
            Properties.Settings.Default.Save();

            return ID;
        }
    }
}

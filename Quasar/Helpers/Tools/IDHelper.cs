using System;

namespace Quasar.Helpers.Tools
{
    static class IDHelper
    {
        public static int getNewContentID()
        {
            int ID = Properties.Settings.Default.ContentID;
            ID++;

            Properties.Settings.Default.ContentID = ID;
            Properties.Settings.Default.Save();

            return ID;
        }

        public static int getNewLibraryID()
        {
            int ID = Properties.Settings.Default.LibraryID;
            ID++;

            Properties.Settings.Default.LibraryID = ID;
            Properties.Settings.Default.Save();

            return ID;
        }

        public static int getNewWorkspaceID()
        {
            int ID = Properties.Settings.Default.WorkspaceIdGenerator;

            Properties.Settings.Default.WorkspaceIdGenerator = ID + 1;
            Properties.Settings.Default.Save();

            return ID;
        }

        public static string getWorkspaceUniqueID()
        {
            Guid guid = Guid.NewGuid();
            return guid.ToString();
        }
    }
}

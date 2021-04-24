using System;

namespace Quasar.Helpers.Tools
{
    static class IDHelper
    {
        /// <summary>
        /// Generates a new Content ID
        /// </summary>
        /// <returns>a new unique INT</returns>
        public static int getNewContentID()
        {
            int ID = Properties.Settings.Default.ContentID;
            ID++;

            Properties.Settings.Default.ContentID = ID;
            Properties.Settings.Default.Save();

            return ID;
        }

        /// <summary>
        /// Generates a new Library ID
        /// </summary>
        /// <returns></returns>
        public static int getNewLibraryID()
        {
            int ID = Properties.Settings.Default.LibraryID;
            ID++;

            Properties.Settings.Default.LibraryID = ID;
            Properties.Settings.Default.Save();

            return ID;
        }

        /// <summary>
        /// Generates a new workspace ID
        /// </summary>
        /// <returns></returns>
        public static int getNewWorkspaceID()
        {
            int ID = Properties.Settings.Default.WorkspaceIdGenerator;

            Properties.Settings.Default.WorkspaceIdGenerator = ID + 1;
            Properties.Settings.Default.Save();

            return ID;
        }

        /// <summary>
        /// Generates a new workspace GUID
        /// </summary>
        /// <returns></returns>
        public static string getWorkspaceUniqueID()
        {
            Guid guid = Guid.NewGuid();
            return guid.ToString();
        }
    }
}

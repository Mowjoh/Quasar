using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.IO;
using System.Collections;
using System.Collections.ObjectModel;

namespace Quasar.XMLResources
{
    public static class Library
    {

        #region XML Interactions
        //Loading the Library Mod List
        public static List<LibraryMod> GetLibraryModList()
        {
            string LibraryFilePath = Properties.Settings.Default["DefaultDir"].ToString() + "\\Library\\Library.xml";
            List<LibraryMod> LibraryModList = new List<LibraryMod>();

            XmlSerializer LibrarySerializer = new XmlSerializer(typeof(LibraryModList));

            if (File.Exists(LibraryFilePath))
            {
                using (FileStream fileStream = new FileStream(LibraryFilePath, FileMode.Open))
                {
                    LibraryModList result = (LibraryModList)LibrarySerializer.Deserialize(fileStream);
                    LibraryModList = result.Cast<LibraryMod>().ToList();
                }
            }

            return LibraryModList;
        }

        //Writing the Mod List to the Library file
        public static void WriteModListFile(List<LibraryMod> LibraryMods)
        {
            string LibraryFilePath = Properties.Settings.Default["DefaultDir"].ToString() + "\\Library\\Library.xml";

            LibraryModList LibraryModList = new LibraryModList(LibraryMods);

            XmlSerializer LibrarySerializer = new XmlSerializer(typeof(LibraryModList));
            using (StreamWriter Writer = new StreamWriter(LibraryFilePath))
            {
                LibrarySerializer.Serialize(Writer, LibraryModList);
            }
        }
        #endregion

        #region Data Operations
        public static LibraryMod GetLibraryMod(APIMod mod,Game _Game)
        {
            //Setting base values
            int modTypeID = -1;
            int modCat = -1;
            string modCatName = "";
            string modTypeName = "";

            try
            {
                //Getting Mod Type
                GameModType modType = _Game.GameModTypes.Find(mt => mt.APIName == mod.ModType);
                modTypeID = modType.ID;
                modTypeName = modType.Name;

                //Getting category
                Category category = modType.Categories.Find(c => c.APICategory == mod.CategoryID);
                if(category != null)
                {
                    modCat = category.ID;
                    modCatName = category.Name;
                }
                else
                {
                    category = modType.Categories.Find(c => c.ID == 0);
                    modCat = 0;
                    modCatName = category.Name;
                }
               
               
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return new LibraryMod() { ID = mod.ID, TypeID = modTypeID, TypeLabel = modTypeName, APICategoryName = modCatName, APICategoryID = modCat, Name = mod.Name, FinishedProcessing = false, Authors = mod.Authors, Updates = mod.UpdateCount, GameID=_Game.ID};
        }
        #endregion

        #region XML Class Definitions
        //XML Serialization elements
        [Serializable()]
        [XmlRoot("Library")]
        public class LibraryModList : ICollection
        {
            public string CollectionName;
            public ArrayList empArray = new ArrayList();

            public LibraryModList()
            {

            }

            public LibraryModList(List<LibraryMod> _inputList)
            {
                foreach(LibraryMod m in _inputList)
                {
                    empArray.Add(m);
                }
            }

            public LibraryMod this[int index]
            {
                get { return (LibraryMod)empArray[index]; }
            }
            public void CopyTo(Array a, int index)
            {
                empArray.CopyTo(a, index);
            }
            public int Count
            {
                get { return empArray.Count; }
            }
            public object SyncRoot
            {
                get { return this; }
            }
            public bool IsSynchronized
            {
                get { return false; }
            }
            public IEnumerator GetEnumerator()
            {
                return empArray.GetEnumerator();
            }

            public void Add(LibraryMod newMod)
            {
                empArray.Add(newMod);
            }

        }

        #endregion

    }

}

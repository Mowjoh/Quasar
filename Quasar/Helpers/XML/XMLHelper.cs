using Quasar.Build.Models;
using Quasar.Data.V1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Quasar.Helpers.XML
{
    public static class XMLHelper
    {
        #region File Saving
        public static void SaveInternalModType(InternalModType type)
        {
            string destination = Properties.Settings.Default.DefaultDir + @"\Resources\InternalModTypes\" + type.Filename + ".xml";

            type.Files.Sort(delegate (InternalModTypeFile x, InternalModTypeFile y)
            {
                return x.ID.CompareTo(y.ID);
            });

            foreach (InternalModTypeFile imtf in type.Files)
            {
                imtf.Files.Sort(delegate (BuilderFile x, BuilderFile y)
                {
                    return x.BuilderID.CompareTo(y.BuilderID);
                });
            }

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(InternalModType));

            using (StreamWriter wr = new StreamWriter(destination))
            {
                xmlSerializer.Serialize(wr, type);
            }
        }
        public static void SaveHashes(string FilePath, List<Hash> Hashes)
        {

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Hashes));

            using (StreamWriter wr = new StreamWriter(FilePath))
            {
                xmlSerializer.Serialize(wr, new Hashes() { HashList = Hashes });
            }
        }
        public static void WriteGameData(List<GameData> GameData)
        {
            string GamesDataPath = Properties.Settings.Default.DefaultDir + @"\Resources\Sources\GameData.xml";

            GamesData gd = new GamesData();
            gd.Games = GameData;

            XmlSerializer LibrarySerializer = new XmlSerializer(typeof(GamesData));
            using (StreamWriter Writer = new StreamWriter(GamesDataPath))
            {
                LibrarySerializer.Serialize(Writer, gd);
            }
        }
        public static void WriteWorkspaces(List<Workspace> Workspaces)
        {
            string AssociationLibraryPath = Properties.Settings.Default["DefaultDir"].ToString() + "\\Library\\Workspaces.xml";

            foreach (Workspace w in Workspaces)
            {
                w.Associations = w.Associations.OrderBy(a => a.GameDataItemID).ThenBy(a => a.Slot).ThenBy(a => a.InternalModTypeID).ToList();
            }

            WorkspaceCollection al = new WorkspaceCollection(Workspaces);

            XmlSerializer LibrarySerializer = new XmlSerializer(typeof(WorkspaceCollection));
            using (StreamWriter Writer = new StreamWriter(AssociationLibraryPath))
            {
                LibrarySerializer.Serialize(Writer, al);
            }
        }
        public static void WriteModListFile(List<LibraryMod> LibraryMods)
        {
            string LibraryFilePath = Properties.Settings.Default["DefaultDir"].ToString() + "\\Library\\Library.xml";

            LibraryMods = LibraryMods.OrderBy(a => a.GameID).ThenBy(a => a.TypeID).ThenBy(a => a.ID).ToList();

            LibModList LibraryModList = new LibModList(LibraryMods);

            XmlSerializer LibrarySerializer = new XmlSerializer(typeof(LibModList));
            using (StreamWriter Writer = new StreamWriter(LibraryFilePath))
            {
                LibrarySerializer.Serialize(Writer, LibraryModList);
            }
        }
        #endregion

        #region File Loading
        public static List<Game> GetGames()
        {
            List<Game> Games = null;

            XmlSerializer serializer = new XmlSerializer(typeof(GameList));

            using (FileStream fileStream = new FileStream(Properties.Settings.Default.DefaultDir + @"\Resources\Sources\Games.xml", FileMode.Open))
            {
                GameList result = (GameList)serializer.Deserialize(fileStream);
                Games = result.Games;
            }
            foreach (Game game in Games)
            {
                game.ImagePath = Properties.Settings.Default.DefaultDir + @"\Resources\images\" + game.Image;
            }
            return Games;
        }
        public static List<InternalModType> GetInternalModTypes()
        {
            List<InternalModType> InteralModTypes = new List<InternalModType>();
            string typesFolderPath = Properties.Settings.Default.DefaultDir + @"\Resources\InternalModTypes\";
            XmlSerializer serializer = new XmlSerializer(typeof(InternalModType));

            try
            {
                foreach (string file in Directory.GetFiles(typesFolderPath, "*", SearchOption.AllDirectories))
                {
                    using (FileStream fileStream = new FileStream(file, FileMode.Open))
                    {

                        InternalModType result = (InternalModType)serializer.Deserialize(fileStream);
                        InteralModTypes.Add(result);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }


            return InteralModTypes;
        }
        public static List<GameData> GetGameData()
        {
            List<GameData> categories = new List<GameData>();

            XmlSerializer serializer = new XmlSerializer(typeof(GamesData));

            using (FileStream fileStream = new FileStream(Properties.Settings.Default.DefaultDir + @"\Resources\Sources\GameData.xml", FileMode.Open))
            {
                GamesData result = (GamesData)serializer.Deserialize(fileStream);
                categories = result.Games;
            }

            return categories;
        }
        public static List<ModLoader> GetModLoaders()
        {
            List<ModLoader> ModLoaders = null;

            XmlSerializer serializer = new XmlSerializer(typeof(ModLoaders));

            using (FileStream fileStream = new FileStream(Properties.Settings.Default.DefaultDir + @"\Resources\Sources\ModLoaders.xml", FileMode.Open))
            {
                ModLoaders result = (ModLoaders)serializer.Deserialize(fileStream);
                ModLoaders = result.ModLoaderList;
            }

            return ModLoaders;
        }
        public static List<Hash> GetHashes(string FilePath)
        {
            List<Hash> Hashes = null;

            XmlSerializer serializer = new XmlSerializer(typeof(Hashes));

            using (FileStream fileStream = new FileStream(FilePath, FileMode.Open))
            {
                Hashes result = (Hashes)serializer.Deserialize(fileStream);
                Hashes = result.HashList;
            }

            return Hashes;
        }
        public static List<Workspace> GetWorkspaces()
        {
            List<Workspace> Workspaces = new List<Workspace>();
            string AssociationLibraryPath = Properties.Settings.Default.DefaultDir + @"\Library\Workspaces.xml";
            XmlSerializer serializer = new XmlSerializer(typeof(WorkspaceCollection));

            if (File.Exists(AssociationLibraryPath))
            {
                using (FileStream fileStream = new FileStream(AssociationLibraryPath, FileMode.Open))
                {

                    WorkspaceCollection result = (WorkspaceCollection)serializer.Deserialize(fileStream);
                    foreach (Workspace workspace in result)
                    {
                        Workspaces.Add(workspace);
                    }
                }
            }
            return Workspaces;
        }
        public static List<LibraryMod> GetLibraryModList()
        {
            string LibraryFilePath = Properties.Settings.Default["DefaultDir"].ToString() + "\\Library\\Library.xml";
            List<LibraryMod> LibraryModList = new List<LibraryMod>();

            XmlSerializer LibrarySerializer = new XmlSerializer(typeof(LibModList));

            if (File.Exists(LibraryFilePath))
            {
                using (FileStream fileStream = new FileStream(LibraryFilePath, FileMode.Open))
                {
                    LibModList result = (LibModList)LibrarySerializer.Deserialize(fileStream);
                    LibraryModList = result.Cast<LibraryMod>().ToList();
                }
            }

            return LibraryModList;
        }
        public static List<ContentMapping> GetContentMappings()
        {
            List<ContentMapping> ContentMappings = new List<ContentMapping>();
            string ContentMappingLibraryPath = Properties.Settings.Default.DefaultDir + @"\Library\ContentMapping.xml";
            XmlSerializer serializer = new XmlSerializer(typeof(ContentMappingList));

            if (File.Exists(ContentMappingLibraryPath))
            {
                using (FileStream fileStream = new FileStream(ContentMappingLibraryPath, FileMode.Open))
                {

                    ContentMappingList result = (ContentMappingList)serializer.Deserialize(fileStream);
                    foreach (ContentMapping cm in result)
                    {
                        ContentMappings.Add(cm);
                    }

                }
            }


            return ContentMappings;
        }
        #endregion

        #region Other operations
        public static LibraryMod GetLibraryMod(APIMod mod, Game _Game)
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
                if (category != null)
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
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return new LibraryMod() { ID = mod.ID, TypeID = modTypeID, TypeLabel = modTypeName, APICategoryName = modCatName, APICategoryID = modCat, Name = mod.Name, FinishedProcessing = false, Authors = mod.Authors, Updates = mod.UpdateCount, GameID = _Game.ID };
        }
        public static void WriteContentMappingListFile(List<ContentMapping> contentMappings)
        {
            string ContentMappingFilePath = Properties.Settings.Default["DefaultDir"].ToString() + "\\Library\\ContentMapping.xml";

            contentMappings = contentMappings.OrderBy(a => a.GameDataItemID).ThenBy(a => a.InternalModType).ThenBy(a => a.Slot).ToList();

            ContentMappingList cml = new ContentMappingList(contentMappings);

            XmlSerializer LibrarySerializer = new XmlSerializer(typeof(ContentMappingList));
            using (StreamWriter Writer = new StreamWriter(ContentMappingFilePath))
            {
                LibrarySerializer.Serialize(Writer, cml);
            }
        }
        public static List<ContentMapping> RemoveUpdatedModMappings(List<ContentMapping> contentMappings, LibraryMod libraryMod)
        {
            List<ContentMapping> CorrespondingMappings = contentMappings.FindAll(cm => cm.ModID == libraryMod.ID);
            foreach (ContentMapping foundContent in CorrespondingMappings)
            {
                contentMappings.Remove(foundContent);
            }

            return contentMappings;
        }
        public static List<ContentMapping> RemoveUpdatedInternalModTypeMappings(List<ContentMapping> contentMappings, InternalModType imt)
        {
            List<ContentMapping> CorrespondingMappings = contentMappings.FindAll(cm => cm.InternalModType == imt.ID);
            foreach (ContentMapping foundContent in CorrespondingMappings)
            {
                contentMappings.Remove(foundContent);
            }

            return contentMappings;
        }
        #endregion

    }

}

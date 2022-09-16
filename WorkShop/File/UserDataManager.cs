using DataModels.Resource;
using DataModels.User;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using log4net;
using Workshop.Builder;

namespace Workshop.FileManagement
{
    public class UserDataManager
    {
        //User Loads
        public static void GetSeparatedContent()
        {

        }

        public static ObservableCollection<LibraryItem> GetSingleFileLibrary(string _QuasarFolderPath)
        {
            ObservableCollection<LibraryItem> Library = new ObservableCollection<LibraryItem>();

            string Path = _QuasarFolderPath + @"\Library\Library.json";

            try
            {
                if (File.Exists(Path))
                {
                    using (StreamReader file = File.OpenText(Path))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        Library = (ObservableCollection<LibraryItem>)serializer.Deserialize(file, typeof(ObservableCollection<LibraryItem>));
                    }
                }
            }
            catch(Exception e)
            {
                
            }

            return Library;
        }

        /// <summary>
        /// Loads the library from different files within their respective mod folders
        /// </summary>
        /// <param name="_library_folder_path">Path to the stored mods</param>
        /// <returns>A full Library containing data for every found mod</returns>
        public static ObservableCollection<LibraryItem> GetSeparatedLibrary(string _library_folder_path)
        {
            ObservableCollection<LibraryItem> Library = new ObservableCollection<LibraryItem>();

            string Path = _library_folder_path + @"\Library\Mods";

            foreach (string LibraryFile in Directory.GetFiles(Path, "LibraryData.json", SearchOption.AllDirectories))
            {
                try
                {
                    if (File.Exists(LibraryFile))
                    {
                        using (StreamReader file = File.OpenText(LibraryFile))
                        {
                            JsonSerializer serializer = new JsonSerializer();
                            Library.Add((LibraryItem)serializer.Deserialize(file, typeof(LibraryItem)));
                        }
                    }
                }
                catch (Exception e)
                {

                }
            }

            return Library;
        }

        public static LibraryItem GetModLibraryItem(string _mod_path)
        {
            string LibraryDataPath = $@"{_mod_path}\LibraryData.json";
            if (File.Exists(LibraryDataPath))
            {
                using (StreamReader file = File.OpenText(LibraryDataPath))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    return (LibraryItem)serializer.Deserialize(file, typeof(LibraryItem));
                }
            }

            return null;
        }
        public static APIData GetAPIData(string _ModInformationPath)
        {
            APIData Item = new APIData();
            try
            {
                if (File.Exists(_ModInformationPath))
                {
                    using (StreamReader file = File.OpenText(_ModInformationPath))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        Item = (APIData)serializer.Deserialize(file, typeof(APIData));
                    }
                }
            }
            catch (Exception e)
            {

            }

            return Item;
        }

        public static ObservableCollection<ContentItem> GetSingleFileContentItems(string _QuasarFolderPath)
        {
            ObservableCollection<ContentItem> ContentItems = new ObservableCollection<ContentItem>();

            string Path = _QuasarFolderPath + @"\Library\ContentItems.json";

            if (File.Exists(Path))
            {
                using (StreamReader file = File.OpenText(Path))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    ContentItems = (ObservableCollection<ContentItem>)serializer.Deserialize(file, typeof(ObservableCollection<ContentItem>));
                }
            }

            return ContentItems;
        }
        public static List<ContentItem> GetModContentItems(string _mod_path)
        {
            string ContentsDataPath = $@"{_mod_path}\ContentData.json";
            if (File.Exists(ContentsDataPath))
            {
                using (StreamReader file = File.OpenText(ContentsDataPath))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    return (List<ContentItem>)serializer.Deserialize(file, typeof(List<ContentItem>));
                }
            }

            return null;
        }
        public static ObservableCollection<ContentItem> GetSeparatedContentItems(string _library_folder_path)
        {
            ObservableCollection<ContentItem> ContentItems = new ObservableCollection<ContentItem>();

            string Path = _library_folder_path + @"\Library\Mods";

            foreach (string ContentDataFile in Directory.GetFiles(Path, "ContentData.json", SearchOption.AllDirectories))
            {
                try
                {
                    if (File.Exists(ContentDataFile))
                    {
                        using (StreamReader file = File.OpenText(ContentDataFile))
                        {
                            JsonSerializer serializer = new JsonSerializer();
                            foreach (ContentItem ci in (List<ContentItem>)serializer.Deserialize(file, typeof(List<ContentItem>)))
                            {
                                ContentItems.Add(ci);
                            }
                            
                        }
                    }
                }
                catch (Exception e)
                {

                }
            }

            return ContentItems;
        }
        public static GamebananaAPI GetSingleFileGamebananaApi(string InstallDirectory, string QuasarPath = "")
        {
            GamebananaAPI API = new GamebananaAPI();

            string Path = QuasarPath + @"\Resources\Gamebanana.json";
            if (!File.Exists(Path))
            {
                Path = InstallDirectory + @"\Resources\Gamebanana.json";
            }
            using (StreamReader file = File.OpenText(Path))
            {
                JsonSerializer serializer = new JsonSerializer();
                API = (GamebananaAPI)serializer.Deserialize(file, typeof(GamebananaAPI));
            }

            return API;
        }
        public static GamebananaAPI GetSeparatedGamebananaApi(string _library_folder_path)
        {
            GamebananaAPI API = new GamebananaAPI()
            {
                Games = new()
                {
                    new GamebananaGame()
                    {
                        Guid = new Guid("923429c3-6e2c-4d66-850d-16177d097106"),
                        RootCategories = new ObservableCollection<GamebananaRootCategory>(),
                        ID = 6498,
                        Name = @"Super Smash Bros. Ultimate"
                    }
                }
            };

            string Path = _library_folder_path + @"\Library\Mods";

            foreach (string APIDataFile in Directory.GetFiles(Path, "Gamebanana.json", SearchOption.AllDirectories))
            {
                try
                {
                    if (File.Exists(APIDataFile))
                    {
                        using (StreamReader file = File.OpenText(APIDataFile))
                        {
                            JsonSerializer serializer = new JsonSerializer();

                            GamebananaRootCategory RootCategory = (GamebananaRootCategory) serializer.Deserialize(file, typeof(GamebananaRootCategory));

                            GamebananaRootCategory FoundCategory = API.Games[0].RootCategories
                                .SingleOrDefault(rc => rc.Guid == RootCategory.Guid);

                            if (FoundCategory != null)
                            {
                                if (!FoundCategory.SubCategories.Any(sc =>
                                        sc.Guid == RootCategory.SubCategories[0].Guid))
                                    FoundCategory.SubCategories.Add(RootCategory.SubCategories[0]);
                            }
                            else
                            {
                                API.Games[0].RootCategories.Add(RootCategory);
                            }

                        }
                    }
                }
                catch (Exception e)
                {

                }

            }

            return API;
        }
        public static GamebananaRootCategory GetGamebananaRootCategory(string _mod_path)
        {
            string GamebananaRootCategoryPath = $@"{_mod_path}\Gamebanana.json";
            if (File.Exists(GamebananaRootCategoryPath))
            {
                using (StreamReader file = File.OpenText(GamebananaRootCategoryPath))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    return (GamebananaRootCategory)serializer.Deserialize(file, typeof(GamebananaRootCategory));
                }
            }

            return null;
        }

        public static GamebananaAPI GetBaseAPI()
        {
            GamebananaAPI API = new GamebananaAPI()
            {
                Games = new()
                {
                    new GamebananaGame()
                    {
                        Guid = new Guid("923429c3-6e2c-4d66-850d-16177d097106"),
                        RootCategories = new ObservableCollection<GamebananaRootCategory>(),
                        ID = 6498,
                        Name = @"Super Smash Bros. Ultimate"
                    }
                }
            };

            return API;
        }
        public static GamebananaAPI GetUpdatedGamebananaApi(GamebananaAPI _source_api, GamebananaRootCategory _root_category)
        {

            GamebananaRootCategory FoundCategory = _source_api.Games[0].RootCategories
                .SingleOrDefault(rc => rc.Guid == _root_category.Guid);

            if (FoundCategory != null)
            {
                if (!FoundCategory.SubCategories.Any(sc =>
                        sc.Guid == _root_category.SubCategories[0].Guid))
                    FoundCategory.SubCategories.Add(_root_category.SubCategories[0]);
            }
            else
            {
                _source_api.Games[0].RootCategories.Add(_root_category);
            }

            return _source_api;
        }
        public static ObservableCollection<FileReference> GetModFiles(string _QuasarFolderPath)
        {
            ObservableCollection<FileReference> ModFiles = new ObservableCollection<FileReference>();

            string Path = _QuasarFolderPath + @"\Library\Downloads\Quasar.json";

            if (File.Exists(Path))
            {
                using (StreamReader file = File.OpenText(Path))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    ModFiles = (ObservableCollection<FileReference>)serializer.Deserialize(file, typeof(ObservableCollection<FileReference>));
                }
            }

            return ModFiles;
        }
        


        //User Saves
        public static void SaveLibrary(ObservableCollection<LibraryItem> _LibraryItems, string _LibraryPath)
        {
            foreach (LibraryItem libraryItem in _LibraryItems)
            {
                string modpath = $@"{_LibraryPath}\Library\Mods\{libraryItem.Guid}\LibraryData.json";
                SaveJSonFile(modpath , libraryItem);
            }
        }
        public static void SaveAPIData(APIData _api_data, string _library_folder, string _library_item_guid)
        {
            SaveJSonFile(_library_folder + String.Format(@"\Library\Mods\{0}\APIData.json", _library_item_guid), _api_data);
        }
        public static void SaveContentItems(ObservableCollection<ContentItem> _ContentItems, string _QuasarFolderPath)
        {
            SaveJSonFile(_QuasarFolderPath + @"\Library\ContentItems.json", _ContentItems.OrderBy(i => i.Guid));
        }
        public static void SaveSeparatedContentItems(ObservableCollection<ContentItem> _ContentItems, string _LibraryPath)
        {
            List<ContentItem> Workload = _ContentItems.ToList();

            while(Workload.Count > 0)
            {

                List<ContentItem> Contents = Workload.Where(c => c.LibraryItemGuid == Workload[0].LibraryItemGuid).ToList();
                string modpath = $@"{_LibraryPath}\Library\Mods\{Contents[0].LibraryItemGuid}\ContentData.json";
                SaveJSonFile(modpath, Contents);

                foreach (ContentItem contentItem in Contents)
                {
                    Workload.Remove(contentItem);
                }

            }
            
        }
        public static void SaveModFiles(List<FileReference> _file_references, string _QuasarFolderPath)
        {
            SaveJSonFile(_QuasarFolderPath + @"\Library\Downloads\Quasar.json", _file_references);
        }
        public static void SaveSharedWorkspaces(ShareableWorkspace SW, string _DestinationFolderPath)
        {
            SaveJSonFile(_DestinationFolderPath + @"\SharedWorkspace.json", SW);
        }

        /// <summary>
        /// Saves the Gamebanana API resource file to the specific path
        /// </summary>
        /// <param name="_API">The resource to save</param>
        /// <param name="_AppDataPath">the user's App Data Path</param>
        public static void SaveSeparatedGamebananaApi(GamebananaAPI _API, ObservableCollection<LibraryItem> _LibraryItems, string _LibraryPath)
        {
            foreach (LibraryItem libraryItem in _LibraryItems)
            {
                
                string modpath = $@"{_LibraryPath}\Library\Mods\{libraryItem.Guid}\Gamebanana.json";
                GamebananaRootCategory RootCategory = _API.Games[0].RootCategories
                    .Single(rc => rc.Guid == libraryItem.GBItem.RootCategoryGuid);
                GamebananaSubCategory SubCategory =
                    RootCategory.SubCategories.Single(sc => sc.Guid == libraryItem.GBItem.SubCategoryGuid);

                GamebananaRootCategory NewCat = new()
                {
                    Guid = RootCategory.Guid,
                    Name = RootCategory.Name,
                    SubCategories = new()
                    {
                        SubCategory
                    }
                };
                    SaveJSonFile(modpath, NewCat);
            }
        }
        

        /// <summary>
        /// Saves a collection to a JSon File
        /// </summary>
        /// <param name="_Fullpath">Destination file path</param>
        /// <param name="_Source">Source collection</param>
        /// <param name="_ExternalPath">Override Destination file path</param>
        private static void SaveJSonFile(string _Fullpath, Object _Source, bool Headless = false)
        {
            string directory = _Fullpath.Replace(@"/", @"\");
            directory = _Fullpath.Replace(_Fullpath.Split(@"\")[_Fullpath.Split(@"\").Length - 1], "");
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            using (StreamWriter file = File.CreateText(_Fullpath))
            {
                JsonSerializer serializer = new JsonSerializer();
                if (Headless)
                    serializer.TypeNameHandling = TypeNameHandling.None;
                serializer.Serialize(file, _Source);
            }
        }

        /// <summary>
        /// Moves all user data Json files to the proper location
        /// </summary>
        /// <param name="_OldDataPath">Quasar's Mod folder</param>
        /// <param name="_NewDataPath">Quasar's App Data Folder</param>
        public static void VerifyUpdateFileLocation(string _OldDataPath, string _NewDataPath)
        {
            string[] OldLibraryFiles = Directory.GetFiles(_OldDataPath + @"\Library", "*", SearchOption.TopDirectoryOnly);

            //If there are old files
            if (OldLibraryFiles.Length > 0)
            {
                string NewLibraryDataPath = _NewDataPath + @"\Library\";

                if (!Directory.Exists(NewLibraryDataPath))
                {
                    Directory.CreateDirectory(NewLibraryDataPath);
                }

                //Moving the files to a new location
                foreach (string LibraryFile in OldLibraryFiles)
                {
                    string FileName = Path.GetFileName(LibraryFile);
                    string NewFilePath = _NewDataPath + @"\Library\" + FileName;

                    File.Copy(LibraryFile, NewFilePath);
                    File.Delete(LibraryFile);
                }
            }
        }

        public static ObservableCollection<LibraryItem> RecoverMods(string _QuasarFolderPath,string _AppDataPath, ObservableCollection<LibraryItem> Library, GamebananaAPI API, ILog QuasarLogger)
        {
            string[] ModFolders = Directory.GetDirectories(_QuasarFolderPath + @"\Library\Mods\", "*", SearchOption.TopDirectoryOnly);

            //foreach(string ModFolder in ModFolders)
            //{
            //    try
            //    {
            //        string ModInfoPath = ModFolder + @"\ModInformation.json";
            //        if (File.Exists(ModInfoPath))
            //        {
            //            QuasarLogger.Debug("Mod Info path is : "+ModInfoPath);

            //            APIData mi = GetAPIData(ModInfoPath);
            //            LibraryItem li = mi.LibraryItem;

            //            QuasarLogger.Debug("Item Parsed : " + li.Name);
            //            if (!Library.Any(l => l.GBItem.GamebananaItemID == li.GBItem.GamebananaItemID))
            //            {
            //                Library.Add(li);
            //            }
            //            API = ResourceManager.UpdateGamebananaAPI(mi.GamebananaRootCategory, API);
            //            QuasarLogger.Debug("Item Successfully processed");
            //        }
            //    }
            //    catch(Exception e)
            //    {
            //        QuasarLogger.Error(e.Message);
            //        QuasarLogger.Error(e.StackTrace);
            //    }
                
            //}
            //ResourceManager.SaveGamebananaAPI(API, _AppDataPath);

            return Library;
        }
    }
}

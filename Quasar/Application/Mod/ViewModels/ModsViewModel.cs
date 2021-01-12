using Quasar.Controls.Common.Models;
using Quasar.Controls.Mod.Models;
using Quasar.Controls.Mod.ViewModels;
using Quasar.Internal;
using Quasar.FileSystem;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using log4net;
using System.Windows.Data;
using Quasar.Controls.Settings.Model;
using System.Windows.Input;
using Quasar.Data.V1;
using Quasar.Helpers.XML;
using Quasar.Helpers.ModScanning;
using Quasar.Compression;

namespace Quasar.Controls.ModManagement.ViewModels
{
    public class ModsViewModel : ObservableObject
    {
        #region Fields
        private ObservableCollection<ModListItem> _ModListItems { get; set; }
        private ObservableCollection<LibraryMod> _Mods { get; set; }
        private ObservableCollection<LibraryMod> _WorkingModList { get; set; }
        private ObservableCollection<ContentMapping> _ContentMappings { get; set; }
        private ObservableCollection<Workspace> _Workspaces { get; set; }
        private Workspace _ActiveWorkspace { get; set; }
        private ObservableCollection<InternalModType> _InternalModTypes { get; set; }
        private ObservableCollection<GameData> _GameDatas { get; set; }
        private ObservableCollection<Game> _Games { get; set; }
        private Game _SelectedGame { get; set; }
        private GameModType _SelectedGameModType { get; set; }
        private ModListItem _SelectedModListItem { get; set; }
        private ObservableCollection<string> _QuasarDownloads { get; set; }
        private CollectionViewSource _CollectionViewSource { get; set; }

        private string _SearchText { get; set; } = "";
        private bool _BlackChecked { get; set; } = true;
        private bool _RedChecked { get; set; } = true;
        private bool _OrangeChecked { get; set; } = true;
        private bool _GreenChecked { get; set; } = true;
        private bool _CreatorMode { get; set; }
        private bool _AdvanceMode { get; set; }
        #endregion

        #region Properties
        public ObservableCollection<ModListItem> ModListItems
        {
            get => _ModListItems;
            set
            {
                if (_ModListItems == value)
                    return;

                _ModListItems = value;
                OnPropertyChanged("ModListItems");
            }
        }
        public ObservableCollection<LibraryMod> Mods
        {
            get => _Mods;
            set
            {
                if (_Mods == value)
                    return;

                _Mods = value;
                OnPropertyChanged("Mods");
            }
        }
        public ObservableCollection<LibraryMod> WorkingModList
        {
            get => _WorkingModList;
            set
            {
                if (_WorkingModList == value)
                    return;

                _WorkingModList = value;
                OnPropertyChanged("WorkingModList");
            }
        }
        public ObservableCollection<ContentMapping> ContentMappings
        {
            get => _ContentMappings;
            set
            {
                if (_ContentMappings == value)
                    return;

                _ContentMappings = value;
                OnPropertyChanged("ContentMappings");
            }
        }
        public ObservableCollection<Workspace> Workspaces
        {
            get => _Workspaces;
            set
            {
                if (_Workspaces == value)
                    return;

                _Workspaces = value;
                OnPropertyChanged("Workspaces");
            }
        }
        /// <summary>
        /// Represents the Active Workspace
        /// </summary>
        public Workspace ActiveWorkspace
        {
            get => _ActiveWorkspace;
            set
            {
                if (_ActiveWorkspace == value)
                    return;

                _ActiveWorkspace = value;
                OnPropertyChanged("ActiveWorkspace");
            }
        }
        /// <summary>
        /// List of all Internal Mod Types
        /// </summary>
        public ObservableCollection<InternalModType> InternalModTypes
        {
            get => _InternalModTypes;
            set
            {
                if (_InternalModTypes == value)
                    return;

                _InternalModTypes = value;
                OnPropertyChanged("InternalModTypes");
            }
        }
        /// <summary>
        /// List of all Game Data
        /// </summary>
        public ObservableCollection<GameData> GameDatas
        {
            get => _GameDatas;
            set
            {
                if (_GameDatas == value)
                    return;

                _GameDatas = value;
                OnPropertyChanged("GameDatas");
            }
        }
        public ObservableCollection<Game> Games
        {
            get => _Games;
            set
            {
                if (_Games == value)
                    return;

                _Games = value;
                OnPropertyChanged("Games");
            }
        }
        public Game SelectedGame
        {
            get => _SelectedGame;
            set
            {
                if (_SelectedGame == value)
                    return;

                _SelectedGame = value;
                OnPropertyChanged("SelectedGame");
            }
        }
        public GameModType SelectedGameModType
        {
            get => _SelectedGameModType;
            set
            {
                if (_SelectedGameModType == value)
                    return;

                _SelectedGameModType = value;
                CollectionViewSource.View.Refresh();
                OnPropertyChanged("SelectedGameModType");
            }
        }
        public ModListItem SelectedModListItem
        {
            get => _SelectedModListItem;
            set
            {
                if (_SelectedModListItem == value)
                    return;

                if(_SelectedModListItem != null)
                {
                    _SelectedModListItem.ModListItemViewModel.Smol = true;
                }

                _SelectedModListItem = value;
                if (_SelectedModListItem != null)
                {
                    _SelectedModListItem.ModListItemViewModel.Smol = false;
                    SelectedModListItem.ModListItemViewModel.ActionRequested = "ElementChanged";
                    EventSystem.Publish<ModListItem>(SelectedModListItem);
                }
                   
                OnPropertyChanged("SelectedModListItem");
            }
        }
        public CollectionViewSource CollectionViewSource
        {
            get => _CollectionViewSource;
            set
            {
                if (_CollectionViewSource == value)
                    return;

                _CollectionViewSource = value;
                OnPropertyChanged("CollectionViewSource");
            }
        }
        public string SearchText
        {
            get => _SearchText;
            set
            {
                if (_SearchText == value)
                    return;

                _SearchText = value;
                OnPropertyChanged("SearchText");
                CollectionViewSource.View.Refresh();
            }
        }
        public bool BlackChecked

        {
            get => _BlackChecked;
            set
            {
                if (value == false)
                    return;

                _BlackChecked = true;
                _RedChecked = false;
                _OrangeChecked = false;
                _GreenChecked = false;
                OnPropertyChanged("BlackChecked");
                OnPropertyChanged("RedChecked");
                OnPropertyChanged("OrangeChecked");
                OnPropertyChanged("GreenChecked");
                CollectionViewSource.View.Refresh();
            }
        }
        public bool RedChecked

        {
            get => _RedChecked;
            set
            {
                if (_RedChecked == value)
                    return;

                _RedChecked = value;
                OnPropertyChanged("RedChecked");
                CollectionViewSource.View.Refresh();
            }
        }
        public bool OrangeChecked
        {
            get => _OrangeChecked;
            set
            {
                if (_OrangeChecked == value)
                    return;

                _OrangeChecked = value;
                OnPropertyChanged("OrangeChecked");
                CollectionViewSource.View.Refresh();

            }
        }
        public bool GreenChecked
        {
            get => _GreenChecked;
            set
            {
                if (_GreenChecked == value)
                    return;

                _GreenChecked = value;
                OnPropertyChanged("GreenChecked");
                CollectionViewSource.View.Refresh();
            }
        }
        public bool CreatorMode

        {
            get => _CreatorMode;
            set
            {
                if (_CreatorMode == value)
                    return;


                _CreatorMode = value;
                ChangeCreatorVisibility(_CreatorMode);
                OnPropertyChanged("CreatorMode");
            }
        }

        public bool AdvanceMode

        {
            get => _AdvanceMode;
            set
            {
                if (_AdvanceMode == value)
                    return;


                _AdvanceMode = value;
                ChangeAdvanceVisibility(_AdvanceMode);
                OnPropertyChanged("AdvanceMode");
            }
        }


        public ILog log { get; set; }
        #endregion

        #region Commands
        private ICommand _AddManual { get; set; }
        public ICommand AddManual
        {
            get
            {
                if (_AddManual == null)
                {
                    _AddManual = new RelayCommand(param => AddManualMod());
                }
                return _AddManual;
            }
        }
        #endregion

        public ModsViewModel(ObservableCollection<LibraryMod> _Mods, ObservableCollection<Game> _Games, ObservableCollection<ContentMapping> _ContentMappings, ObservableCollection<Workspace> _Workspaces, Workspace _ActiveWorkspace, ObservableCollection<InternalModType> _InternalModTypes, ObservableCollection<GameData> _GameDatas, ILog _log)
        {
            Mods = _Mods;
            Games = _Games;
            SelectedGame = _Games[1];
            ContentMappings = _ContentMappings;
            Workspaces = _Workspaces;
            ActiveWorkspace = _ActiveWorkspace;
            InternalModTypes = _InternalModTypes;
            GameDatas = _GameDatas;
            log = _log;

            log.Debug("Parsing Mod List Items");
            ParseModListItems();

            CollectionViewSource = new CollectionViewSource();
            CollectionViewSource.Source = ModListItems;
            CollectionViewSource.SortDescriptions.Add(new System.ComponentModel.SortDescription() { PropertyName="ModListItemViewModel.LibraryMod.Name",Direction= System.ComponentModel.ListSortDirection.Ascending });
            CollectionViewSource.Filter += ModTypeFilter;

            EventSystem.Subscribe<ModListItemViewModel>(GetModListElementTrigger);
            EventSystem.Subscribe<QuasarDownload>(Download);
            WorkingModList = new ObservableCollection<LibraryMod>();

            EventSystem.Subscribe<SettingItem>(SettingChanged);
            EventSystem.Subscribe<Workspace>(WorkspaceChanged);
        }

        #region Actions
        //Window Actions
        public void ParseModListItems()
        {
            ModListItems = new ObservableCollection<ModListItem>();

            foreach (LibraryMod lm in Mods)
            {
                Game gamu = Games.Single(g => g.ID == lm.GameID);

                ModListItem mli = new ModListItem(this,_LibraryMod: lm, _Game: gamu);
                mli.ModListItemViewModel.LoadStats();
                ModListItems.Add(mli);
            }
        }
        public void WorkspaceChanged(Workspace workspace)
        {
            ActiveWorkspace = workspace;
        }
        public void GetModListElementTrigger(ModListItemViewModel ModListItemViewModel)
        {
            ModListItem MLI = ModListItems.Single(m => m.ModListItemViewModel == ModListItemViewModel);
            switch (ModListItemViewModel.ActionRequested)
            {
                case "Delete":
                    DeleteMod(MLI);
                    break;
                case "Add":
                    AddMod(MLI);
                    break;
                case "Remove":
                    RemoveMod(MLI);
                    break;
                case "ShowContents":
                    ShowModContents(MLI);
                    break;
                default:
                    break;
            }
        }
        public void ModTypeFilter(object sender, FilterEventArgs e)
        {
            ModListItem mli = e.Item as ModListItem;
            if(SelectedGameModType != null)
            {
                if (mli.ModListItemViewModel.LibraryMod.TypeID == SelectedGameModType.ID || SelectedGameModType.ID == -1)
                {
                    if ((RedChecked && mli.ModListItemViewModel.ContentStatValue == 0) || (OrangeChecked && mli.ModListItemViewModel.ContentStatValue == 1) || (GreenChecked && mli.ModListItemViewModel.ContentStatValue == 2))
                    {
                        if (SearchText != "")
                        {
                            if (mli.ModListItemViewModel.LibraryMod.APICategoryName.ToLower().Contains(SearchText.ToLower()))
                            {
                                e.Accepted = true;
                            }
                            else
                            {
                                e.Accepted = false;
                            }
                        }
                        else
                        {
                            e.Accepted = true;
                        }

                    }
                    else
                    {
                        e.Accepted = false;
                    }
                }
                else
                {
                    e.Accepted = false;
                }
            }
            else
            {
                if ((RedChecked && mli.ModListItemViewModel.ContentStatValue == 0) || (OrangeChecked && mli.ModListItemViewModel.ContentStatValue == 1) || (GreenChecked && mli.ModListItemViewModel.ContentStatValue == 2))
                {
                    if (SearchText != "")
                    {
                        if (mli.ModListItemViewModel.LibraryMod.APICategoryName.ToLower().Contains(SearchText.ToLower()))
                        {
                            e.Accepted = true;
                        }
                        else
                        {
                            e.Accepted = false;
                        }
                    }
                    else
                    {
                        e.Accepted = true;
                    }
                    
                }
                else
                {
                    e.Accepted = false;
                }
                
            }
            
        }

        public void SettingChanged(SettingItem Setting)
        {
            if (Setting.SettingName == "EnableCreator")
            {
                CreatorMode = Setting.IsChecked;
            }
            if (Setting.SettingName == "EnableAdvanced")
            {
                AdvanceMode = Setting.IsChecked;
            }
        }

        public void ChangeCreatorVisibility(bool val)
        {
            foreach(ModListItem mli in ModListItems)
            {
                mli.ModListItemViewModel.CreatorMode = val;
            }
        }

        public void ChangeAdvanceVisibility(bool val)
        {
            foreach (ModListItem mli in ModListItems)
            {
                mli.ModListItemViewModel.AdvancedMode = val;
            }
        }

        //Mod List Item Actions
        public void DeleteMod(ModListItem item)
        {
            Boolean proceed = false;
            if (!Properties.Settings.Default.SupressModDeletion)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this mod ?", "Mod Deletion", MessageBoxButton.YesNo);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        proceed = true;
                        break;
                    case MessageBoxResult.No:
                        break;
                }
            }

            if (proceed || Properties.Settings.Default.SupressModDeletion)
            {
                //Removing Files
                ModFileManager mfm = new ModFileManager(item.ModListItemViewModel.LibraryMod, Games[1]);
                mfm.DeleteFiles();

                //Removing from ContentMappings
                List<ContentMapping> relatedMappings = ContentMappings.Where(cm => cm.ModID == item.ModListItemViewModel.LibraryMod.ID).ToList();
                foreach (ContentMapping cm in relatedMappings)
                {
                    foreach (Workspace w in Workspaces)
                    {
                        List<Association> associations = w.Associations.FindAll(ass => ass.ContentMappingID == cm.ID);
                        if (associations != null)
                        {
                            foreach (Association ass in associations)
                            {
                                w.Associations.Remove(ass);
                            }
                        }
                        ContentMappings.Remove(cm);
                    }

                }

                ModListItems.Remove(item);
                Mods.Remove(item.ModListItemViewModel.LibraryMod);

                //Writing changes
                XMLHelper.WriteModListFile(Mods.ToList());
                XMLHelper.WriteContentMappingListFile(ContentMappings.ToList());
                XMLHelper.WriteWorkspaces(Workspaces.ToList());

                Application.Current.Dispatcher.Invoke((Action)delegate {
                    EventSystem.Publish<string>("RefreshContents");
                });
            }
        }
        public void AddMod(ModListItem MLI)
        {
            

           //Removing from ContentMappings
           List<ContentMapping> relatedMappings = ContentMappings.Where(cm => cm.ModID == MLI.ModListItemViewModel.LibraryMod.ID).ToList();
           foreach (ContentMapping cm in relatedMappings)
           {
               if (cm.GameDataItemID != -1)
               {
                    InternalModType imt = InternalModTypes.Single(i => i.ID == cm.InternalModType);
                    if (imt.OutsideFolder)
                    {
                        int Slot = 0;
                        bool foundSlot = false;

                        while (!foundSlot)
                        {
                            if (!ActiveWorkspace.Associations.Any(a => a.InternalModTypeID == imt.ID && a.Slot == Slot))
                            {
                                ActiveWorkspace.Associations.Add(new Association() { ContentMappingID = cm.ID, GameDataItemID = cm.GameDataItemID, InternalModTypeID = cm.InternalModType, Slot = Slot });
                                foundSlot = true;
                            }
                            Slot++;
                        }
                    }
                    else
                    {
                        Association associations = ActiveWorkspace.Associations.Find(ass => ass.GameDataItemID == cm.GameDataItemID && ass.InternalModTypeID == cm.InternalModType && ass.Slot == cm.Slot);
                        if (associations != null)
                        {
                            log.Debug(String.Format("Association exists for ContentMapping ID '{0}', slot '{1}', IMT '{2}', GDIID '{3}', removing it", associations.ContentMappingID, associations.Slot, associations.InternalModTypeID, associations.GameDataItemID));
                            ActiveWorkspace.Associations.Remove(associations);
                        }
                        log.Debug(String.Format("Association created for ContentMapping '{0}' ID '{1}', slot '{2}', IMT '{3}', GDIID '{4}'", cm.Name, cm.ID, cm.Slot, cm.InternalModType, cm.GameDataItemID));
                        ActiveWorkspace.Associations.Add(new Association() { ContentMappingID = cm.ID, GameDataItemID = cm.GameDataItemID, InternalModTypeID = cm.InternalModType, Slot = cm.Slot });
                    }
                }
           }
            XMLHelper.WriteWorkspaces(Workspaces.ToList());
            log.Debug("Written changes to Workspaces");
            ReloadAllStats();
            CollectionViewSource.View.Refresh();
        }
        public void RemoveMod(ModListItem MLI)
        {
            //Removing from ContentMappings
            List<ContentMapping> relatedMappings = ContentMappings.Where(cm => cm.ModID == MLI.ModListItemViewModel.LibraryMod.ID).ToList();
            foreach (ContentMapping cm in relatedMappings)
            {
                if (cm.GameDataItemID != -1)
                {
                    InternalModType imt = InternalModTypes.Single(i => i.ID == cm.InternalModType);
                    List<Association> associations = null;
                    if (imt.OutsideFolder)
                    {
                        associations = ActiveWorkspace.Associations.Where(ass => ass.InternalModTypeID == cm.InternalModType && ass.ContentMappingID == cm.ID).ToList();

                    }
                    else
                    {
                        associations = ActiveWorkspace.Associations.Where(ass => ass.GameDataItemID == cm.GameDataItemID && ass.InternalModTypeID == cm.InternalModType && ass.Slot == cm.Slot && ass.ContentMappingID == cm.ID).ToList();
                    }
                    if (associations != null)
                    {
                        foreach(Association ass in associations)
                        {
                            log.Debug(String.Format("Association found for ContentMapping ID '{0}', slot '{1}', IMT '{2}', GDIID '{3}', removing it it", ass.ContentMappingID, ass.Slot, ass.InternalModTypeID, ass.GameDataItemID));
                            ActiveWorkspace.Associations.Remove(ass);
                        }
                        
                    }
                }
            }
            XMLHelper.WriteWorkspaces(Workspaces.ToList());
            log.Debug("Written changes to Workspaces");
            MLI.ModListItemViewModel.LoadStats();
            CollectionViewSource.View.Refresh();

        }
        public void UpdateMod()
        {
            /*
           ModListItem element = (ModListItem)ManagementModListView.SelectedItem;
           if (element != null)
           {
               //Getting local Mod
               LibraryMod mod = Mods.Find(mm => mm.ID == element.LocalMod.ID && mm.TypeID == element.LocalMod.TypeID);
               Game game = Games.Find(g => g.ID == mod.GameID);
               GameModType mt = game.GameModTypes.Find(g => g.ID == mod.TypeID);
               //Parsing mod info from API
               APIMod newAPIMod = await APIRequest.GetAPIMod(mt.APIName, element.LocalMod.ID.ToString());

               //Create Mod from API information
               LibraryMod newmod = GetLibraryMod(newAPIMod, game);

               if (mod.Updates < newmod.Updates)
               {
                   string[] newDL = await APIRequest.GetDownloadFileName(mt.APIName, element.LocalMod.ID.ToString());
                   string quasarURL = APIRequest.GetQuasarDownloadURL(newDL[0], newDL[1], mt.APIName, element.LocalMod.ID.ToString());
                   LaunchDownload(quasarURL);
               }
           }*/
        }
        public void ShowModContents(ModListItem MLI)
        {
            if (SelectedModListItem == null)
                return;

            SelectedModListItem.ModListItemViewModel.ActionRequested = "ShowContents";
            EventSystem.Publish<ModListItem>(SelectedModListItem);
        }

        public void AddManualMod()
        {
            ModListItem mli = new ModListItem();
            mli.ModListItemViewModel = new ModListItemViewModel();
            mli.ModListItemViewModel.ActionRequested = "ShowContents";
            EventSystem.Publish<ModListItem>(mli);
        }

        //Mod List Item Downloading
        public void Download(QuasarDownload download)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate {
                Task.Run(() => Download(download.QuasarURL));
            });

        }

        public async void LaunchDL(string QuasarURL)
        {
            Task<bool> dl = Download(QuasarURL);
            if (!dl.Result)
            {
                
            }
        }

        public async Task<bool> Download(string QuasarURL)
        {
            ModListItemViewModel MIVM = new ModListItemViewModel();
            ModListItem MLI = null;

            log.Info("Download Started");

            //Setting base info
            ModFileManager ModFileManager = new ModFileManager(QuasarURL);
            if (ModFileManager.Failed)
            {
                AbortDownload(MLI);
                return false;
            }

            bool newElement = false;
            string downloadText = null;

            //Getting info from the API
            APIMod newAPIMod = await APIRequest.GetAPIMod(ModFileManager.APIType, ModFileManager.ModID);

            if (newAPIMod == null)
            {
                AbortDownload(MLI);
                return false;
            }

                

            //Getting corresponding game
            Game game = Games.Single(g => g.GameName == newAPIMod.GameName);

            //Updating ModFileManager
            ModFileManager = new ModFileManager(QuasarURL, game);
            if (ModFileManager.Failed)
            {
                AbortDownload(MLI);
                return false;
            }

            //Finding existing mod
            LibraryMod Mod = Mods.SingleOrDefault(mm => mm.ID == Int32.Parse(ModFileManager.ModID) && mm.TypeID == Int32.Parse(ModFileManager.ModTypeID));

            //Create Mod from API information
            LibraryMod newmod = XMLHelper.GetLibraryMod(newAPIMod, game);

            bool needupdate = true;
            //Checking if Mod is already in library
            if (Mod != null)
            {
                if (Mod.Updates < newmod.Updates)
                {/*
                        var query = ListMods.Where(ml => ml.LocalMod == Mod);
                        mli = query.ElementAt(0);*/
                    downloadText = "Updating mod";
                    log.Debug("Existing Mod");
                }
                else
                {
                    log.Debug("New Mod");
                    needupdate = false;
                }
            }
            else
            {
                Mod = new LibraryMod(Int32.Parse(ModFileManager.ModID), Int32.Parse(ModFileManager.ModTypeID), false);
                newElement = true;
                downloadText = "Downloading new mod";
            }
            if (!WorkingModList.Contains(Mod) && needupdate)
            {
                bool ContinueOperation = true;

                WorkingModList.Add(Mod);
                log.Debug("Mod wasn't in the working list");
                //Setting up new ModList
                if (newElement)
                {
                    //Adding element to list
                    Mods.Add(newmod);
                    try
                    {
                        Application.Current.Dispatcher.Invoke((Action)delegate {
                            MLI = new ModListItem(this,newmod, game, true);
                            ModListItems.Add(MLI);
                        });
                        
                    }
                    catch(Exception e)
                    {
                        MLI = null;
                        Console.WriteLine(e.Message);
                    }
                }
                else
                {
                    //Updating List
                    Mods[Mods.IndexOf(Mod)] = newmod;
                    MLI = ModListItems.Single(m => m.ModListItemViewModel.LibraryMod == newmod);
                }
                log.Debug("Setting UI");
                //Setting download UI
                MLI.ModListItemViewModel.ModStatusValue = downloadText;
                log.Debug("Launching Download");
                ModDownloader modDownloader = new ModDownloader(MLI.ModListItemViewModel);

                try
                {
                    //Wait for download completion
                    await modDownloader.DownloadArchiveAsync(ModFileManager);
                }
                catch(Exception e)
                {
                    log.Error(String.Format("Download failed : {0}",e.Message));
                    log.Error(String.Format("Trace : {0}", e.StackTrace));
                    AbortDownload(MLI);
                    return false;
                }
                log.Debug("Download Finished");
                log.Debug("Launching Extraction");
                //Setting extract UI
                MLI.ModListItemViewModel.ModStatusValue = "Extracting mod";

                //Preparing Extraction
                Unarchiver un = new Unarchiver(MLI.ModListItemViewModel);

                try
                {
                    //Wait for Archive extraction
                    await un.ExtractArchiveAsync(ModFileManager.DownloadDestinationFilePath, ModFileManager.ArchiveContentFolderPath, ModFileManager.ModArchiveFormat);
                }
                catch (Exception e)
                {
                    log.Error(String.Format("Extraction failed : {0}", e.Message));
                    log.Error(String.Format("Trace : {0}", e.StackTrace));
                    AbortDownload(MLI);
                    return false;
                }
                log.Debug("Extraction Finished");

                //Setting extract UI
                MLI.ModListItemViewModel.ModStatusValue = "Moving files";
                

                try
                {
                    //Moving files
                    await ModFileManager.MoveDownload();
                }
                catch (Exception e)
                {
                    log.Error(String.Format("Move Failed: {0}", e.Message));
                    log.Error(String.Format("Trace : {0}", e.StackTrace));
                    AbortDownload(MLI);
                    return false;
                }

                log.Debug("Move Finished");

                //Cleanup
                ModFileManager.ClearDownloadContents();
                try
                {
                    //Getting Screenshot from Gamebanana
                    await APIRequest.GetScreenshot(ModFileManager.APIType, ModFileManager.ModID, game.ID.ToString(), Mod.TypeID.ToString(), Mod.ID.ToString());
                }
                catch(Exception e)
                {
                    log.Error("Could not get Screenshot");
                    log.Error(e.Message);
                }
                

                log.Debug("Screenshot Finished");

                try
                {
                    //Scanning Files
                    FirstScanLibraryMod(newmod, game, InternalModTypes, GameDatas);
                }
                catch (Exception e)
                {
                    log.Error("Could not scan files");
                    log.Error(e.Message);
                    AbortDownload(MLI);
                    return false;
                }
                log.Debug("Scan Finished");

                //Refreshing  Interface
                MLI.ModListItemViewModel.Downloading = false;

                log.Debug("Writing Changes");

                //Saving XML
                XMLHelper.WriteModListFile(Mods.ToList()); ;

                //Removing mod from Working List
                WorkingModList.Remove(Mod);
                //QuasarTaskBar.ProgressState = System.Windows.Shell.TaskbarItemProgressState.None;
                ReloadAllStats();
            }
            else
            {
                log.Debug("Mod Already is in the working list");
            }

            return true;
        }

        //Mod List Item Scanning
        private bool FirstScanLibraryMod(LibraryMod LibraryMod, Game Game, ObservableCollection<InternalModType> InternalModTypes, ObservableCollection<GameData> GameDatas)
        {
            bool processed = false;
            ObservableCollection<ContentMapping> SearchList = Searchie.AutoDetectinator(LibraryMod, InternalModTypes, Game, GameDatas);

            ObservableCollection<ContentMapping> WorkingList = ContentMappings;
            foreach (ContentMapping cm in SearchList)
            {
                WorkingList.Add(cm);
                processed = true;
            }
            ContentMappings = WorkingList;
            AutoSlotDetectedItems(SearchList);
            XMLHelper.WriteContentMappingListFile(ContentMappings.ToList());
            
            Application.Current.Dispatcher.Invoke((Action)delegate {
                EventSystem.Publish<string>("RefreshContents");
            });
            
            return processed;
        }
        private void AutoSlotDetectedItems(ObservableCollection<ContentMapping> elements)
        {
            foreach (ContentMapping cm in elements)
            {
                if (cm.GameDataItemID != -1)
                {
                    Association associations = ActiveWorkspace.Associations.Find(ass => ass.GameDataItemID == cm.GameDataItemID && ass.InternalModTypeID == cm.InternalModType && ass.Slot == cm.Slot);
                    if (associations != null)
                    {
                        InternalModType imt = InternalModTypes.Single(i => i.ID == cm.InternalModType);
                        if (imt.OutsideFolder)
                        {
                            int Slot = 0;
                            bool foundSlot = false;

                            while (!foundSlot)
                            {
                                if (!ActiveWorkspace.Associations.Any(a => a.InternalModTypeID == imt.ID && a.Slot == Slot))
                                {
                                    ActiveWorkspace.Associations.Add(new Association() { ContentMappingID = cm.ID, GameDataItemID = cm.GameDataItemID, InternalModTypeID = cm.InternalModType, Slot = Slot });
                                    foundSlot = true;
                                }
                                Slot++;
                            }
                        }
                        else
                        {
                            ActiveWorkspace.Associations[ActiveWorkspace.Associations.IndexOf(associations)] = new Association() { ContentMappingID = cm.ID, GameDataItemID = cm.GameDataItemID, InternalModTypeID = cm.InternalModType, Slot = cm.Slot };

                        }
                    }
                    else
                    {

                        ActiveWorkspace.Associations.Add(new Association() { ContentMappingID = cm.ID, GameDataItemID = cm.GameDataItemID, InternalModTypeID = cm.InternalModType, Slot = cm.Slot });

                    }
                }
            }

            XMLHelper.WriteWorkspaces(Workspaces.ToList());
        }
        public void ReloadAllStats()
        {
            foreach(ModListItem i in ModListItems)
            {
                i.ModListItemViewModel.LoadStats();
            }
        }

        public void AbortDownload(ModListItem MLI)
        {
            if(MLI != null)
            {
                if (MLI.ModListItemViewModel == null)
                {
                    MLI.ModListItemViewModel = new ModListItemViewModel()
                    {
                        DownloadFailed = true
                    };
                }
                MLI.ModListItemViewModel.Downloading = false;
                MLI.ModListItemViewModel.DownloadFailed = true;
            }
        }
        #endregion
    }
}

using Quasar.Common.Models;
using Quasar.Controls.Mod.Models;
using Quasar.Controls.Mod.ViewModels;
using Quasar.Helpers;
using Quasar.FileSystem;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using log4net;
using System.Windows.Data;
using Quasar.Settings.Models;
using System.Windows.Input;
using DataModels.Common;
using DataModels.User;
using DataModels.Resource;
using Quasar.Helpers.ModScanning;
using Quasar.Helpers.Downloading;
using Quasar.Helpers.Mod_Scanning;
using Quasar.Helpers.Tools;
using Quasar.MainUI.ViewModels;
using Microsoft.WindowsAPICodePack.Taskbar;
using System.Diagnostics;
using Workshop.FileManagement;
using Workshop.Web;

namespace Quasar.Controls.ModManagement.ViewModels
{
    public class ModsViewModel : ObservableObject
    {

        public static string AppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Quasar";

        #region Data

        #region Private
        private ObservableCollection<ModListItem> _ModListItems { get; set; }
        private ObservableCollection<ModManager> _ActiveModManagers { get; set; }
        private GamebananaRootCategory _SelectedGamebananaRootCategory { get; set; }
        private ModListItem _SelectedModListItem { get; set; }
        private ObservableCollection<string> _QuasarDownloads { get; set; }
        private CollectionViewSource _CollectionViewSource { get; set; }
        private MainUIViewModel _MUVM { get; set; }
        
        #endregion

        #region Public
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
        public ObservableCollection<ModManager> ActiveModManagers
        {
            get => _ActiveModManagers;
            set
            {
                if (_ActiveModManagers == value)
                    return;

                _ActiveModManagers = value;
                OnPropertyChanged("ActiveModManagers");
            }
        }
        public GamebananaRootCategory SelectedGamebananaRootCategory
        {
            get => _SelectedGamebananaRootCategory;
            set
            {
                if (_SelectedGamebananaRootCategory == value)
                    return;

                _SelectedGamebananaRootCategory = value;
                CollectionViewSource.View.Refresh();
                OnPropertyChanged("SelectedGamebananaRootCategory");
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
                    //SelectedModListItem.ModListItemViewModel.ActionRequested = "ElementChanged";
                    //EventSystem.Publish<ModListItem>(SelectedModListItem);
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
        public MainUIViewModel MUVM
        {
            get => _MUVM;
            set
            {
                if (_MUVM == value)
                    return;

                _MUVM = value;
                OnPropertyChanged("MUVM");
            }
        }
        ModListItem mliToDelete { get; set; }
        
        #endregion

        #endregion

        #region View

        #region Private
        private string _SearchText { get; set; } = "";
        private bool _BlackChecked { get; set; } = true;
        private bool _RedChecked { get; set; } = true;
        private bool _OrangeChecked { get; set; } = true;
        private bool _GreenChecked { get; set; } = true;
        private bool _PurpleChecked { get; set; } = true;
        private bool _CreatorMode { get; set; }
        private bool _AdvanceMode { get; set; }
        private bool _TypeFilterSelected { get; set; }
        private bool _CategoryFilterSelected { get; set; }
        private bool _TimeFilterSelected { get; set; }
        #endregion

        #region Public

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
        public bool PurpleChecked
        {
            get => _PurpleChecked;
            set
            {
                if (_PurpleChecked == value)
                    return;

                _PurpleChecked = value;
                OnPropertyChanged("PurpleChecked");
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

        public bool TypeFilterSelected
        {
            get => _TypeFilterSelected;
            set
            {
                if (_TypeFilterSelected == value)
                    return;

                _TypeFilterSelected = value;
                OnPropertyChanged("TypeFilterSelected");

                if (value)
                {
                    if (!CollectionViewSource.SortDescriptions.Any(sd => sd.PropertyName == "ModListItemViewModel.LibraryItem.APICategoryName"))
                    {
                        CollectionViewSource.SortDescriptions.Insert((CollectionViewSource.SortDescriptions.Count - 1), new System.ComponentModel.SortDescription() { PropertyName = "ModListItemViewModel.LibraryItem.APICategoryName", Direction = System.ComponentModel.ListSortDirection.Ascending });
                    }
                }
                else
                {
                    CollectionViewSource.SortDescriptions.Remove(CollectionViewSource.SortDescriptions.Single(sd => sd.PropertyName == "ModListItemViewModel.LibraryItem.APICategoryName"));
                }

                CollectionViewSource.View.Refresh();
            }
        }
        public bool CategoryFilterSelected
        {
            get => _CategoryFilterSelected;
            set
            {
                if (_CategoryFilterSelected == value)
                    return;

                _CategoryFilterSelected = value;
                OnPropertyChanged("CategoryFilterSelected");

                if (value)
                {
                    if (!CollectionViewSource.SortDescriptions.Any(sd => sd.PropertyName == "ModListItemViewModel.APISubCategoryName"))
                    {
                        CollectionViewSource.SortDescriptions.Insert((CollectionViewSource.SortDescriptions.Count - 1), new System.ComponentModel.SortDescription() { PropertyName = "ModListItemViewModel.APISubCategoryName", Direction = System.ComponentModel.ListSortDirection.Ascending });
                    }
                }
                else
                {
                    CollectionViewSource.SortDescriptions.Remove(CollectionViewSource.SortDescriptions.Single(sd => sd.PropertyName == "ModListItemViewModel.APISubCategoryName"));
                }

                CollectionViewSource.View.Refresh();
            }
        }

        public bool TimeFilterSelected
        {
            get => _TimeFilterSelected;
            set
            {
                if (_TimeFilterSelected == value)
                    return;

                _TimeFilterSelected = value;
                OnPropertyChanged("TimeFilterSelected");
                if (value)
                {
                    if(!CollectionViewSource.SortDescriptions.Any(sd => sd.PropertyName == "ModListItemViewModel.LibraryItem.Time"))
                    {
                        CollectionViewSource.SortDescriptions.Insert((CollectionViewSource.SortDescriptions.Count - 1), new System.ComponentModel.SortDescription() { PropertyName = "ModListItemViewModel.LibraryItem.Time", Direction = System.ComponentModel.ListSortDirection.Descending });
                    }
                }
                else
                {
                    CollectionViewSource.SortDescriptions.Remove(CollectionViewSource.SortDescriptions.Single(sd => sd.PropertyName == "ModListItemViewModel.LibraryItem.Time"));
                }
                CollectionViewSource.View.Refresh();
            }
        }

        #endregion

        #endregion

        #region Commands

        #region Private
        private ICommand _AddManual { get; set; }
        private ICommand _ResetFilters { get; set; }
        #endregion

        #region Public
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
        public ICommand ResetFilters
        {
            get
            {
                if (_ResetFilters == null)
                {
                    _ResetFilters = new RelayCommand(param => ResetFilter());
                }
                return _ResetFilters;
            }
        }
        #endregion

        #endregion

        public ILog QuasarLogger { get; set; }

        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <param name="_MUVM"></param>
        /// <param name="_QuasarLogger"></param>
        public ModsViewModel(MainUIViewModel _MUVM, ILog _QuasarLogger)
        {
            MUVM = _MUVM;
            QuasarLogger = _QuasarLogger;

            QuasarLogger.Debug("Parsing Mod List Items");
            ParseModListItems();

            CollectionViewSource = new CollectionViewSource();
            CollectionViewSource.Source = ModListItems;
            CollectionViewSource.SortDescriptions.Add(new System.ComponentModel.SortDescription() { PropertyName= "ModListItemViewModel.LibraryItem.Name", Direction= System.ComponentModel.ListSortDirection.Ascending });
            CollectionViewSource.Filter += ModTypeFilter;

            EventSystem.Subscribe<ModListItemViewModel>(GetModListElementTrigger);
            EventSystem.Subscribe<QuasarDownload>(Download);
            EventSystem.Subscribe<ModalEvent>(ProcessIncomingModalEvent);

            ActiveModManagers = new ObservableCollection<ModManager>();

            EventSystem.Subscribe<SettingItem>(SettingChanged);
            EventSystem.Subscribe<Workspace>(WorkspaceChanged);
        }

        #region Actions

        /// <summary>
        /// Creates and fills the mod list
        /// </summary>
        public void ParseModListItems()
        {
            ModListItems = new ObservableCollection<ModListItem>();

            foreach (LibraryItem li in MUVM.Library)
            {
                Game gamu = MUVM.Games.Single(g => g.ID == li.GameID);

                ModListItem mli = new ModListItem(this, QuasarLogger,_LibraryItem: li, _Game: gamu);
                mli.ModListItemViewModel.LoadStats();
                ModListItems.Add(mli);
            }
        }
        
        /// <summary>
        /// Function called to filter the mod List
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ModTypeFilter(object sender, FilterEventArgs e)
        {
            //Getting Item
            ModListItem mli = e.Item as ModListItem;

            //Getting Color Match Status
            int ColorValue = mli.ModListItemViewModel.ContentStatValue;
            bool MatchingCheckBox = (RedChecked && ColorValue == 0)
                 || (OrangeChecked && ColorValue == 1)
                 || (GreenChecked && ColorValue == 2)
                 || (PurpleChecked && ColorValue == 3);

            //Getting Type Select Match
            bool NoSelectedType = SelectedGamebananaRootCategory == null;
            bool MatchingSelectedType = false;
            if(!NoSelectedType)
                MatchingSelectedType = mli.ModListItemViewModel.LibraryItem.GBItem.RootCategoryGuid == SelectedGamebananaRootCategory.Guid;

            //Getting Filter Text Match
            bool EmptyText = SearchText.Length == 0;
            bool MatchingName = mli.ModListItemViewModel.LibraryItem.Name.Contains(SearchText.ToLower()) && !EmptyText;
            bool MatchingCategory = mli.ModListItemViewModel.APISubCategoryName.ToLower().Contains(SearchText.ToLower()) && !EmptyText;

            //Match status
            if (MatchingCheckBox && (MatchingSelectedType || NoSelectedType) && (MatchingName || MatchingCategory || EmptyText))
            {
                e.Accepted = true;
            }
            else
            {
                e.Accepted = false;
            }
        }

        /// <summary>
        /// Trigger for downloads
        /// </summary>
        /// <param name="download"></param>
        public void Download(QuasarDownload download)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate {
                
                Task.Run(() => Flash());
                Task.Run(() => DownloadMod(download.QuasarURL));
            });
        }

        /// <summary>
        /// Download process
        /// </summary>
        /// <param name="QuasarURL"></param>
        /// <returns></returns>
        public async Task<bool> DownloadMod(string QuasarURL)
        {
            
            ModManager MM = new ModManager(QuasarURL);
            //If there is no Mod Manager already doing something for this mod
            if(!ActiveModManagers.Any(m => m.QuasarURL.GamebananaItemID == MM.QuasarURL.GamebananaItemID))
            {
                if (ActiveModManagers.Count == 0)
                {
                    EventSystem.Publish<SettingItem>(new SettingItem
                    {
                        IsChecked = true,
                        SettingName = "TabLock",
                        DisplayValue = "Mod"
                    });
                }

                //Adding it to the active list
                ActiveModManagers.Add(MM);

                //Evaluating if something needs to be done
                bool result = await MM.EvaluateActionNeeded(MUVM);
                if (result)
                {
                    if (MM.ActionNeeded)
                    {
                        ModListItem MLI = null;
                        try
                        {
                            if (MM.DownloadNeeded)
                            {
                                QuasarLogger.Debug("New Item");
                                //Creating new Mod List Item
                                Application.Current.Dispatcher.Invoke((Action)delegate {
                                    MLI = new ModListItem(this, QuasarLogger, MM.LibraryItem, MUVM.Games[0], true);
                                    ModListItems.Add(MLI);
                                });

                            }
                            else
                            {
                                QuasarLogger.Debug("Existing Item");
                                //Parsing Existing Mod List Item
                                MLI = ModListItems.Single(i => i.ModListItemViewModel.LibraryItem.GBItem.GamebananaItemID.ToString() == MM.QuasarURL.GamebananaItemID);
                            }
                            MM.ModListItem = MLI;

                            //Executing tasks for this mod
                            await MM.TakeAction(QuasarLogger);

                            //Updating Library
                            if (MM.DownloadNeeded)
                            {
                                QuasarLogger.Debug("Adding to Library");
                                //If the mod is new and downloaded
                                MUVM.Library.Add(MM.LibraryItem);
                                UserDataManager.SaveLibrary(MUVM.Library, AppDataPath);

                                GamebananaRootCategory RC = MUVM.API.Games[0].RootCategories.Single(c => c.Guid == MM.LibraryItem.GBItem.RootCategoryGuid);
                                GamebananaSubCategory SC = RC.SubCategories.Single(c => c.Guid == MM.LibraryItem.GBItem.SubCategoryGuid);
                                ModInformation MI = new ModInformation()
                                {
                                    LibraryItem = MM.LibraryItem,
                                    GamebananaRootCategory = new GamebananaRootCategory()
                                    {
                                        Guid = RC.Guid,
                                        Name = RC.Name,
                                        SubCategories = new ObservableCollection<GamebananaSubCategory>()
                                        {
                                            new GamebananaSubCategory()
                                            {
                                                Guid = SC.Guid,
                                                ID = SC.ID,
                                                Name = SC.Name
                                            }
                                        }
                                    }
                                };
                                UserDataManager.SaveModInformation(MI, Properties.Settings.Default.DefaultDir);
                            }
                            else
                            {
                                QuasarLogger.Debug("Editing Library");
                                //If the mod is updated
                                LibraryItem li = MUVM.Library.Single(i => i.Guid == MM.LibraryItem.Guid);
                                li = MM.LibraryItem;
                                UserDataManager.SaveLibrary(MUVM.Library, AppDataPath);

                                GamebananaRootCategory RC = MUVM.API.Games[0].RootCategories.Single(c => c.Guid == MM.LibraryItem.GBItem.RootCategoryGuid);
                                GamebananaSubCategory SC = RC.SubCategories.Single(c => c.Guid == MM.LibraryItem.GBItem.SubCategoryGuid);
                                ModInformation MI = new ModInformation()
                                {
                                    LibraryItem = MM.LibraryItem,
                                    GamebananaRootCategory = new GamebananaRootCategory()
                                    {
                                        Guid = RC.Guid,
                                        Name = RC.Name,
                                        SubCategories = new ObservableCollection<GamebananaSubCategory>()
                                        {
                                            new GamebananaSubCategory()
                                            {
                                                Guid = SC.Guid,
                                                ID = SC.ID,
                                                Name = SC.Name
                                            }
                                        }
                                    }
                                };
                                UserDataManager.SaveModInformation(MI, Properties.Settings.Default.DefaultDir);
                            }

                            QuasarLogger.Debug("Scanning Mod");
                            //Launching scan
                            await MM.Scan(MUVM.QuasarModTypes, MUVM.Games[0]);
                            Scannerino.UpdateContents(MUVM, MM.LibraryItem, MM.ScannedContents);

                            QuasarLogger.Debug("Saving Contents");
                            //Saving Contents
                            UserDataManager.SaveContentItems(MUVM.ContentItems, AppDataPath);

                            QuasarLogger.Debug("Slotting Contents and Saving Workspace");
                            //Slotting Contents
                            MUVM.ActiveWorkspace = Slotter.AutomaticSlot(MM.ScannedContents.ToList(), MUVM.ActiveWorkspace, MUVM.QuasarModTypes);
                            UserDataManager.SaveWorkspaces(MUVM.Workspaces, AppDataPath);
                            ReloadAllStats();
                        }
                        catch(Exception e)
                        {
                            MLI.ModListItemViewModel.DownloadFailed = true;

                            QuasarLogger.Error("Could not download mod");
                            QuasarLogger.Error(e.Message);
                            QuasarLogger.Error(e.StackTrace);
                        }
                        

                        MLI.ModListItemViewModel.Downloading = false;
                    }
                }
                
                ActiveModManagers.Remove(MM);
                if(ActiveModManagers.Count == 0)
                {
                    EventSystem.Publish<SettingItem>(new SettingItem
                    {
                        IsChecked = false,
                        SettingName = "TabLock"
                    });
                }
            }

            return true;
        }

        public void Flash()
        {
            TaskbarManager.Instance.SetProgressValue(100, 100, Process.GetCurrentProcess().MainWindowHandle);
            System.Threading.Thread.Sleep(500);
            TaskbarManager.Instance.SetProgressValue(0, 100, Process.GetCurrentProcess().MainWindowHandle);
        }
        /// <summary>
        /// Reloads the workspace presence status for all list items
        /// </summary>
        public void ReloadAllStats()
        {
            foreach(ModListItem i in ModListItems)
            {
                i.ModListItemViewModel.LoadStats();
            }
        }

        /// <summary>
        /// Refreshes the view
        /// </summary>
        public void ViewRefresh()
        {
            CollectionViewSource.Source = null;
            ParseModListItems();
            CollectionViewSource.Source = ModListItems;
            CollectionViewSource.View.Refresh();
        }

        #endregion

        #region User Actions

        /// <summary>
        /// Adds a new Manual mod entry to the library
        /// </summary>
        public void AddManualMod()
        {
            LibraryItem li = new LibraryItem()
            {
                Name = "Manually added mod",
                GameID = 1,
                Guid = Guid.NewGuid(),
                Time = DateTime.Now
            };

            ModListItem mli = new ModListItem(this, QuasarLogger, li, MUVM.Games[0]);

            MUVM.Library.Add(li);
            ModListItems.Add(mli);
            CollectionViewSource.View.Refresh();
            UserDataManager.SaveLibrary(MUVM.Library, AppDataPath);

        }

        /// <summary>
        /// Resets the selected Filters
        /// </summary>
        public void ResetFilter()
        {
            SelectedGamebananaRootCategory = null;
            SearchText = "";
            OnPropertyChanged("SelectedGamebananaRootCategory");
            OnPropertyChanged("SearchText");
        }

        #endregion

        #region Event System

        //Workspace Trigger
        /// <summary>
        /// Trigger when the workspace is changed
        /// </summary>
        /// <param name="workspace"></param>
        public void WorkspaceChanged(Workspace workspace)
        {
            MUVM.ActiveWorkspace = workspace;
        }

        //Settings Triggers
        /// <summary>
        /// Trigger when a setting is changed
        /// </summary>
        /// <param name="Setting"></param>
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

        /// <summary>
        /// Changes the visibility of items affected by the Creator setting
        /// </summary>
        /// <param name="val"></param>
        public void ChangeCreatorVisibility(bool val)
        {
            foreach (ModListItem mli in ModListItems)
            {
                mli.ModListItemViewModel.CreatorMode = val;
            }
        }

        /// <summary>
        /// Changes the visibility of items affected by the Advanced setting
        /// </summary>
        /// <param name="val"></param>
        public void ChangeAdvanceVisibility(bool val)
        {
            foreach (ModListItem mli in ModListItems)
            {
                mli.ModListItemViewModel.AdvancedMode = val;
            }
        }

        //Mod List Item Triggers
        /// <summary>
        /// Trigger for an event incoming from a Mod List Item View Model
        /// </summary>
        /// <param name="ModListItemViewModel"></param>
        public void GetModListElementTrigger(ModListItemViewModel ModListItemViewModel)
        {
            ModListItem MLI = ModListItems.Single(m => m.ModListItemViewModel == ModListItemViewModel);
            switch (ModListItemViewModel.ActionRequested)
            {
                case "Delete":
                    AskDeleteMod(MLI);
                    break;
                case "Add":
                    AddToWorkspace(MLI);
                    break;
                case "Remove":
                    RemoveMod(MLI);
                    break;
                case "ShowContents":
                    ShowModContents(MLI);
                    break;
                case "Update":
                    UpdateMod(MLI);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Asks if the user wants to delete the mod
        /// </summary>
        /// <param name="item"></param>
        public void AskDeleteMod(ModListItem item)
        {
            mliToDelete = item;

            if (!Properties.Settings.Default.SupressModDeletion)
            {
                ModalEvent meuh = new ModalEvent()
                {
                    EventName = "DeleteMod",
                    Type = ModalType.OkCancel,
                    Action = "Show",
                    Title = "Mod Deletion",
                    Content = "Are you sure you want to delete this mod ? \rIt's file and information will be removed from Quasar.",
                    OkButtonText = "I'm sure",
                    CancelButtonText = "Cancel"

                };

                EventSystem.Publish<ModalEvent>(meuh);
            }
            else
            {
                DeleteMod(mliToDelete);
            }
        }

        /// <summary>
        /// Adds this mod's contents to the workspace
        /// </summary>
        /// <param name="MLI"></param>
        public void AddToWorkspace(ModListItem MLI)
        {
            //Removing from ContentMappings
            List<ContentItem> relatedMappings = MUVM.ContentItems.Where(i => i.LibraryItemGuid == MLI.ModListItemViewModel.LibraryItem.Guid).ToList();
            MUVM.ActiveWorkspace = Slotter.AutomaticSlot(relatedMappings, MUVM.ActiveWorkspace, MUVM.QuasarModTypes);
            UserDataManager.SaveWorkspaces(MUVM.Workspaces, AppDataPath);
            QuasarLogger.Debug("Written changes to Workspaces");
            ReloadAllStats();
            CollectionViewSource.View.Refresh();
        }

        /// <summary>
        /// Removes this mod's contents from the workspace
        /// </summary>
        /// <param name="MLI"></param>
        public void RemoveMod(ModListItem MLI)
        {
            //Removing from ContentMappings
            List<ContentItem> relatedMappings = MUVM.ContentItems.Where(cm => cm.LibraryItemGuid == MLI.ModListItemViewModel.LibraryItem.Guid).ToList();
            foreach (ContentItem ci in relatedMappings)
            {
                if (ci.GameElementID != -1)
                {
                    QuasarModType qmt = MUVM.QuasarModTypes.Single(i => i.ID == ci.QuasarModTypeID);
                    List<Association> associations = null;
                    if (qmt.IsExternal)
                    {
                        associations = MUVM.ActiveWorkspace.Associations.Where(ass => ass.QuasarModTypeID == ci.QuasarModTypeID && ass.ContentItemGuid == ci.Guid).ToList();

                    }
                    else
                    {
                        associations = MUVM.ActiveWorkspace.Associations.Where(ass => ass.GameElementID == ci.GameElementID && ass.QuasarModTypeID == ci.QuasarModTypeID && ass.SlotNumber == ci.SlotNumber && ass.ContentItemGuid == ci.Guid).ToList();
                    }
                    if (associations != null)
                    {
                        foreach (Association ass in associations)
                        {
                            QuasarLogger.Debug(String.Format("Association found for ContentMapping ID '{0}', slot '{1}', IMT '{2}', GDIID '{3}', removing it it", ass.ContentItemGuid, ass.SlotNumber, ass.QuasarModTypeID, ass.GameElementID));
                            MUVM.ActiveWorkspace.Associations.Remove(ass);
                        }

                    }
                }
            }
            UserDataManager.SaveWorkspaces(MUVM.Workspaces, AppDataPath);
            QuasarLogger.Debug("Written changes to Workspaces");
            MLI.ModListItemViewModel.LoadStats();
            CollectionViewSource.View.Refresh();

        }

        /// <summary>
        /// Triggers this mod's update
        /// </summary>
        /// <param name="MLI"></param>
        public void UpdateMod(ModListItem MLI)
        {
            GamebananaRootCategory RCat = MUVM.API.Games[0].RootCategories.Single(c => c.Guid == MLI.ModListItemViewModel.LibraryItem.GBItem.RootCategoryGuid);

            Application.Current.Dispatcher.Invoke((Action)delegate {
                Task.Run(() =>
                DownloadMod(APIRequest.GetQuasarDownloadURL("",RCat.Name, MLI.ModListItemViewModel.LibraryItem.GBItem.GamebananaItemID.ToString())));;
            });
        }

        /// <summary>
        /// Show this mod's file view
        /// </summary>
        /// <param name="MLI"></param>
        public void ShowModContents(ModListItem MLI)
        {
            if (SelectedModListItem == null)
                return;

            SelectedModListItem.ModListItemViewModel.ActionRequested = "ShowContents";
            EventSystem.Publish<ModListItem>(SelectedModListItem);
        }


        #endregion

        #region Modal Events

        /// <summary>
        /// Trigger when an incoming ModalEvent is received
        /// </summary>
        /// <param name="me"></param>
        public void ProcessIncomingModalEvent(ModalEvent me)
        {
            if (me.EventName == "DeleteMod")
            {
                switch (me.Action)
                {
                    case "OK":
                        DeleteMod(mliToDelete);
                        break;
                    default:
                        break;
                }
            }

        }

        /// <summary>
        /// Deletes the mod from the Library
        /// </summary>
        /// <param name="item"></param>
        public void DeleteMod(ModListItem item)
        {
            //Removing Files
            ModFileManager mfm = new ModFileManager(item.ModListItemViewModel.LibraryItem);
            mfm.DeleteFiles();

            //Removing from ContentMappings
            List<ContentItem> relatedMappings = MUVM.ContentItems.Where(cm => cm.LibraryItemGuid == item.ModListItemViewModel.LibraryItem.Guid).ToList();
            foreach (ContentItem ci in relatedMappings)
            {
                foreach (Workspace w in MUVM.Workspaces)
                {
                    List<Association> associations = w.Associations.Where(ass => ass.ContentItemGuid == ci.Guid).ToList();
                    if (associations != null)
                    {
                        foreach (Association ass in associations)
                        {
                            w.Associations.Remove(ass);
                        }
                    }
                    MUVM.ContentItems.Remove(ci);
                }

            }

            ModListItems.Remove(item);
            MUVM.Library.Remove(item.ModListItemViewModel.LibraryItem);

            //Writing changes
            UserDataManager.SaveLibrary(MUVM.Library, AppDataPath);
            UserDataManager.SaveContentItems(MUVM.ContentItems, AppDataPath);
            UserDataManager.SaveWorkspaces(MUVM.Workspaces, AppDataPath);

            Application.Current.Dispatcher.Invoke((Action)delegate {
                EventSystem.Publish<string>("RefreshContents");
            });
        }

        #endregion
    }

    //Sort Type Enum
    public enum SortType { AtoZ = 1, ZtoA = 2, Disabled = 3}
}

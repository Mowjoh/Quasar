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
using Quasar.Data.V2;
using Quasar.Helpers.XML;
using Quasar.Helpers.ModScanning;
using Quasar.Compression;
using Quasar.Helpers.Json;
using Quasar.Helpers.Downloading;
using Quasar.Helpers.Mod_Scanning;
using Quasar.Internal.Tools;

namespace Quasar.Controls.ModManagement.ViewModels
{
    public class ModsViewModel : ObservableObject
    {
        #region Fields
        private ObservableCollection<ModListItem> _ModListItems { get; set; }
        private ObservableCollection<ModManager> _ActiveModManagers { get; set; }
        private GameAPICategory _SelectedGameAPICategory { get; set; }
        private ModListItem _SelectedModListItem { get; set; }
        private ObservableCollection<string> _QuasarDownloads { get; set; }
        private CollectionViewSource _CollectionViewSource { get; set; }

        private MainUIViewModel _MUVM { get; set; }
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
        public GameAPICategory SelectedGameAPICategory
        {
            get => _SelectedGameAPICategory;
            set
            {
                if (_SelectedGameAPICategory == value)
                    return;

                _SelectedGameAPICategory = value;
                CollectionViewSource.View.Refresh();
                OnPropertyChanged("SelectedGameAPICategory");
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
                    if (CollectionViewSource.SortDescriptions.Count == 1)
                    {
                        CollectionViewSource.SortDescriptions.Insert(0, new System.ComponentModel.SortDescription() { PropertyName = "ModListItemViewModel.LibraryItem.APICategoryName", Direction = System.ComponentModel.ListSortDirection.Ascending });
                    }
                    else
                    {
                        if (CollectionViewSource.SortDescriptions.Count == 2)
                        {
                            CollectionViewSource.SortDescriptions.Insert(1, new System.ComponentModel.SortDescription() { PropertyName = "ModListItemViewModel.LibraryItem.APICategoryName", Direction = System.ComponentModel.ListSortDirection.Ascending });
                        }
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
                    if (CollectionViewSource.SortDescriptions.Count == 1)
                    {
                        CollectionViewSource.SortDescriptions.Insert(0, new System.ComponentModel.SortDescription() { PropertyName = "ModListItemViewModel.APISubCategoryName", Direction = System.ComponentModel.ListSortDirection.Ascending });
                    }
                    else
                    {
                        if (CollectionViewSource.SortDescriptions.Count == 2)
                        {
                            CollectionViewSource.SortDescriptions.Insert(1, new System.ComponentModel.SortDescription() { PropertyName = "ModListItemViewModel.APISubCategoryName", Direction = System.ComponentModel.ListSortDirection.Ascending });
                        }
                    }
                    
                }
                else
                {
                    CollectionViewSource.SortDescriptions.Remove(CollectionViewSource.SortDescriptions.Single(sd => sd.PropertyName == "ModListItemViewModel.APISubCategoryName"));
                }
                CollectionViewSource.View.Refresh();
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

        public ModsViewModel(MainUIViewModel _MUVM, ILog _log)
        {
            MUVM = _MUVM;
            log = _log;

            log.Debug("Parsing Mod List Items");
            ParseModListItems();

            CollectionViewSource = new CollectionViewSource();
            CollectionViewSource.Source = ModListItems;
            CollectionViewSource.SortDescriptions.Add(new System.ComponentModel.SortDescription() { PropertyName= "ModListItemViewModel.LibraryItem.Name", Direction= System.ComponentModel.ListSortDirection.Ascending });
            CollectionViewSource.Filter += ModTypeFilter;

            EventSystem.Subscribe<ModListItemViewModel>(GetModListElementTrigger);
            EventSystem.Subscribe<QuasarDownload>(Download);
            ActiveModManagers = new ObservableCollection<ModManager>();

            EventSystem.Subscribe<SettingItem>(SettingChanged);
            EventSystem.Subscribe<Workspace>(WorkspaceChanged);
        }

        #region Actions
        //Window Actions
        public void ParseModListItems()
        {
            ModListItems = new ObservableCollection<ModListItem>();

            foreach (LibraryItem li in MUVM.Library)
            {
                Game gamu = MUVM.Games.Single(g => g.ID == li.GameID);

                ModListItem mli = new ModListItem(this,_LibraryItem: li, _Game: gamu);
                mli.ModListItemViewModel.LoadStats();
                ModListItems.Add(mli);
            }
        }
        public void WorkspaceChanged(Workspace workspace)
        {
            MUVM.ActiveWorkspace = workspace;
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
            if(SelectedGameAPICategory != null)
            {
                if (mli.ModListItemViewModel.LibraryItem.APICategoryName == SelectedGameAPICategory.APICategoryName || SelectedGameAPICategory.ID == -1)
                {
                    if ((RedChecked && mli.ModListItemViewModel.ContentStatValue == 0) || (OrangeChecked && mli.ModListItemViewModel.ContentStatValue == 1) || (GreenChecked && mli.ModListItemViewModel.ContentStatValue == 2) || (PurpleChecked && mli.ModListItemViewModel.ContentStatValue == 3))
                    {
                        if (SearchText != "")
                        {
                            if (mli.ModListItemViewModel.APISubCategoryName.ToLower().Contains(SearchText.ToLower()))
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
                if ((RedChecked && mli.ModListItemViewModel.ContentStatValue == 0) || (OrangeChecked && mli.ModListItemViewModel.ContentStatValue == 1) || (GreenChecked && mli.ModListItemViewModel.ContentStatValue == 2) || (PurpleChecked && mli.ModListItemViewModel.ContentStatValue == 3))
                {
                    if (SearchText != "")
                    {
                        if (mli.ModListItemViewModel.APISubCategoryName.ToLower().Contains(SearchText.ToLower()))
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
                ModFileManager mfm = new ModFileManager(item.ModListItemViewModel.LibraryItem, MUVM.Games[0]);
                mfm.DeleteFiles();

                //Removing from ContentMappings
                List<ContentItem> relatedMappings = MUVM.ContentItems.Where(cm => cm.LibraryItemID == item.ModListItemViewModel.LibraryItem.ID).ToList();
                foreach (ContentItem ci in relatedMappings)
                {
                    foreach (Workspace w in MUVM.Workspaces)
                    {
                        List<Association> associations = w.Associations.Where(ass => ass.ContentItemID == ci.ID).ToList();
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
                JSonHelper.SaveLibrary(MUVM.Library);
                JSonHelper.SaveContentItems(MUVM.ContentItems);
                JSonHelper.SaveWorkspaces(MUVM.Workspaces);

                Application.Current.Dispatcher.Invoke((Action)delegate {
                    EventSystem.Publish<string>("RefreshContents");
                });
            }
        }
        public void AddMod(ModListItem MLI)
        {
            //Removing from ContentMappings
            List<ContentItem> relatedMappings = MUVM.ContentItems.Where(i => i.LibraryItemID == MLI.ModListItemViewModel.LibraryItem.ID).ToList();
            MUVM.ActiveWorkspace = Slotter.AutomaticSlot(relatedMappings, MUVM.ActiveWorkspace, MUVM.QuasarModTypes);
            JSonHelper.SaveWorkspaces(MUVM.Workspaces);
            log.Debug("Written changes to Workspaces");
            ReloadAllStats();
            CollectionViewSource.View.Refresh();
        }
        public void RemoveMod(ModListItem MLI)
        {
            //Removing from ContentMappings
            List<ContentItem> relatedMappings = MUVM.ContentItems.Where(cm => cm.LibraryItemID == MLI.ModListItemViewModel.LibraryItem.ID).ToList();
            foreach (ContentItem ci in relatedMappings)
            {
                if (ci.GameElementID != -1)
                {
                    QuasarModType qmt = MUVM.QuasarModTypes.Single(i => i.ID == ci.QuasarModTypeID);
                    List<Association> associations = null;
                    if (qmt.IsExternal)
                    {
                        associations = MUVM.ActiveWorkspace.Associations.Where(ass => ass.QuasarModTypeID == ci.QuasarModTypeID && ass.ContentItemID == ci.ID).ToList();

                    }
                    else
                    {
                        associations = MUVM.ActiveWorkspace.Associations.Where(ass => ass.GameElementID == ci.GameElementID && ass.QuasarModTypeID == ci.QuasarModTypeID && ass.SlotNumber == ci.SlotNumber && ass.ContentItemID == ci.ID).ToList();
                    }
                    if (associations != null)
                    {
                        foreach(Association ass in associations)
                        {
                            log.Debug(String.Format("Association found for ContentMapping ID '{0}', slot '{1}', IMT '{2}', GDIID '{3}', removing it it", ass.ContentItemID, ass.SlotNumber, ass.QuasarModTypeID, ass.GameElementID));
                            MUVM.ActiveWorkspace.Associations.Remove(ass);
                        }
                        
                    }
                }
            }
            JSonHelper.SaveWorkspaces(MUVM.Workspaces);
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
            LibraryItem li = new LibraryItem()
            {
                ManualMod = true,
                Name = "Manually added mod",
                GameID = 1,
                ID = IDHelper.getNewLibraryID(), 
                Authors = new ObservableCollection<Author>()
                {
                    new Author()
                    {
                         Name = "Manual",
                         Role = "Imported this"
                    }
                }
            };

            ModListItem mli = new ModListItem(this,li,MUVM.Games[0]);

            MUVM.Library.Add(li);
            ModListItems.Add(mli);
            CollectionViewSource.View.Refresh();
            JSonHelper.SaveLibrary(MUVM.Library);

            mli.ModListItemViewModel.ActionRequested = "ShowContents";
            EventSystem.Publish<ModListItem>(mli);
        }

        //Mod List Item Downloading
        public void Download(QuasarDownload download)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate {
                Task.Run(() => DownloadMod(download.QuasarURL));
            });
        }

        public async Task<bool> DownloadMod(string QuasarURL)
        {
            ModManager MM = new ModManager(QuasarURL);
            //If there is no Mod Manager already doing something for this mod
            if(!ActiveModManagers.Any(m => m.QuasarURL.LibraryItemID == MM.QuasarURL.LibraryItemID))
            {
                //Adding it to the active list
                ActiveModManagers.Add(MM);

                //Evaluating if something needs to be done
                await MM.EvaluateActionNeeded(MUVM);

                if (MM.ActionNeeded)
                {
                    ModListItem MLI = null;
                    if (MM.DownloadNeeded)
                    {
                        //Creating new Mod List Item
                        Application.Current.Dispatcher.Invoke((Action)delegate {
                            MLI = new ModListItem(this, MM.LibraryItem, MUVM.Games[0], true);
                            ModListItems.Add(MLI);
                        });

                    }
                    else
                    {
                        //Parsing Existing Mod List Item
                        MLI = ModListItems.Single(i => i.ModListItemViewModel.LibraryItem.ID.ToString() == MM.QuasarURL.LibraryItemID);
                    }
                    MM.ModListItem = MLI;
                    
                    //Executing tasks for this mod
                    await MM.TakeAction();

                    //Updating Library
                    if (MM.DownloadNeeded)
                    {
                        //If the mod is new and downloaded
                        MUVM.Library.Add(MM.LibraryItem);
                        JSonHelper.SaveLibrary(MUVM.Library);
                    }
                    else
                    {
                        //If the mod is updated
                        LibraryItem li = MUVM.Library.Single(i => i.ID == MM.LibraryItem.ID);
                        li = MM.LibraryItem;
                        JSonHelper.SaveLibrary(MUVM.Library);
                    }

                    //Launching scan
                    await MM.Scan(MUVM.QuasarModTypes,MUVM.Games[0]);
                    Scannerino.UpdateContents(MUVM, MM.LibraryItem, MM.ScannedContents);

                    //Saving Contents
                    JSonHelper.SaveContentItems(MUVM.ContentItems);

                    //Slotting Contents
                    MUVM.ActiveWorkspace = Slotter.AutomaticSlot(MM.ScannedContents.ToList(), MUVM.ActiveWorkspace, MUVM.QuasarModTypes);
                    JSonHelper.SaveWorkspaces(MUVM.Workspaces);
                    ReloadAllStats();

                    MLI.ModListItemViewModel.Downloading = false;
                }
                ActiveModManagers.Remove(MM);
            }

            return true;
        }

        //Mod List Item Scanning
        public void ReloadAllStats()
        {
            foreach(ModListItem i in ModListItems)
            {
                i.ModListItemViewModel.LoadStats();
            }
        }

        public void ViewRefresh()
        {
            CollectionViewSource.Source = null;
            ParseModListItems();
            CollectionViewSource.Source = ModListItems;
            CollectionViewSource.View.Refresh();
        }

        #endregion

    }
    public enum SortType { AtoZ = 1, ZtoA = 2, Disabled = 3}
}

using log4net;
using Quasar.Controls.Associations.Models;
using Quasar.Controls.Associations.ViewModels;
using Quasar.Controls.Associations.Views;
using Quasar.Controls.Common.Models;
using Quasar.Controls.InternalModTypes.Models;
using Quasar.Internal;
using Quasar.XMLResources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Documents.DocumentStructures;
using System.Windows.Input;

namespace Quasar.Controls.Assignation.ViewModels
{
    public class AssociationViewModel : ObservableObject
    {
        #region Fields

        #region View
        private ObservableCollection<GameDataUXItem> _GameDataUXs { get; set; }
        private GameDataItem _SelectedGameDataItem { get; set; }
        private ObservableCollection<SlotItem> _SlotItems { get; set; }
        private SlotItem _SelectedSlotItem { get; set; }
        private ObservableCollection<SlotItem> _AvailableSlots { get; set; }
        private CollectionViewSource _ItemsCollectionViewSource { get; set; }
        private bool _SelectionVisible { get; set; }
        private bool _TypesGrouped { get; set; }
        private string _FilterText { get; set; }
        #endregion

        #region Data
        private ObservableCollection<Workspace> _Workspaces { get; set; }
        private Workspace _ActiveWorkspace { get; set; }
        private ObservableCollection<ContentMapping> _ContentMappings { get; set; }
        private ObservableCollection<GameData> _GameDatas { get; set; }
        private ObservableCollection<InternalModType> _InternalModTypes { get; set; }
        private ObservableCollection<InternalModType> _SelectedInternalModTypes { get; set; }
        private InternalModType _SelectedInternalModType { get; set; }
        private ObservableCollection<InternalModTypeGroup> _SelectedInternalModTypeGroups { get; set; }
        private InternalModTypeGroup _SelectedInternalModTypeGroup { get; set; }
        private GameData _SelectedGameData { get; set; }
        private GameDataUXItem _SelectedGameDataCategory { get; set; }
        #endregion

        #region Commands
        private ICommand _SelectItemCommand { get; set; }
        #endregion
        
        #endregion

        #region Properties

        #region View
        public ObservableCollection<GameDataUXItem> GameDataUXs
        {
            get => _GameDataUXs;
            set
            {
                if (_GameDataUXs == value)
                    return;

                _GameDataUXs = value;
                OnPropertyChanged("GameDataUXs");
            }
        }
        public GameDataItem SelectedGameDataItem
        {
            get => _SelectedGameDataItem;
            set
            {
                if (_SelectedGameDataItem == value)
                    return;

                _SelectedGameDataItem = value;
                OnPropertyChanged("SelectedGameDataItem");
                if (TypesGrouped && SelectedInternalModTypeGroup != null || !TypesGrouped && SelectedInternalModType != null)
                {
                    ShowSlots();
                }
            }
        }
        public ObservableCollection<SlotItem> SlotItems
        {
            get => _SlotItems;
            set
            {
                if (_SlotItems == value)
                    return;

                _SlotItems = value;
                OnPropertyChanged("SlotItems");
            }
        }
        public SlotItem SelectedSlotItem

        {
            get => _SelectedSlotItem;
            set
            {
                if (_SelectedSlotItem == value)
                    return;

                _SelectedSlotItem = value;
                OnPropertyChanged("SelectedSlotItem");
            }
        }
        public ObservableCollection<SlotItem> AvailableSlots
        {
            get => _AvailableSlots;
            set
            {
                if (_AvailableSlots == value)
                    return;

                _AvailableSlots = value;
                OnPropertyChanged("AvailableSlots");
            }
        }
        public CollectionViewSource ItemsCollectionViewSource

        {
            get => _ItemsCollectionViewSource;
            set
            {
                if (_ItemsCollectionViewSource == value)
                    return;

                _ItemsCollectionViewSource = value;
                OnPropertyChanged("ItemsCollectionViewSource");
            }
        }
        public bool SelectionVisible
        {
            get => _SelectionVisible;
            set
            {
                if (_SelectionVisible == value)
                    return;

                _SelectionVisible = value;
                OnPropertyChanged("SelectionVisible");
            }
        }
        public bool TypesGrouped
        {
            get => _TypesGrouped;
            set
            {
                Properties.Settings.Default.GroupInternalModTypes = value;
                Properties.Settings.Default.Save();

                if (_TypesGrouped == value)
                    return;

                _TypesGrouped = value;
                OnPropertyChanged("TypesGrouped");

                if (value)
                {
                    if(SelectedInternalModTypeGroups != null)
                        SelectedInternalModTypeGroup = SelectedInternalModTypeGroups[0];
                }
                else
                {
                    if (SelectedInternalModTypes != null)
                        SelectedInternalModType = SelectedInternalModTypes[0];
                }
                ShowSlots();
            }
        }
        public string FilterText
        {
            get => _FilterText;
            set
            {
                if (_FilterText == value)
                    return;

                _FilterText = value;
                if (ItemsCollectionViewSource != null)
                {
                    ItemsCollectionViewSource.View.Refresh();
                }

                OnPropertyChanged("FilterText");
            }
        }
        #endregion

        #region Data
        /// <summary>
        /// List of workspaces
        /// </summary>
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
        /// List of all content mappings
        /// </summary>
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
        public ObservableCollection<InternalModType> SelectedInternalModTypes
        {
            get => _SelectedInternalModTypes;
            set
            {
                if (_SelectedInternalModTypes == value)
                    return;

                _SelectedInternalModTypes = value;
                OnPropertyChanged("SelectedInternalModTypes");
            }
        }
        public InternalModType SelectedInternalModType
        {
            get => _SelectedInternalModType;
            set
            {
                if (_SelectedInternalModType == value)
                    return;

                _SelectedInternalModType = value;
                OnPropertyChanged("SelectedInternalModType");
                if (SelectedGameDataItem != null)
                {
                    ShowSlots();
                }

            }
        }
        public ObservableCollection<InternalModTypeGroup> SelectedInternalModTypeGroups
        {
            get => _SelectedInternalModTypeGroups;
            set
            {
                if (_SelectedInternalModTypeGroups == value)
                    return;

                _SelectedInternalModTypeGroups = value;
                OnPropertyChanged("SelectedInternalModTypeGroups");
            }
        }
        public InternalModTypeGroup SelectedInternalModTypeGroup
        {
            get => _SelectedInternalModTypeGroup;
            set
            {
                if (_SelectedInternalModTypeGroup == value)
                    return;

                _SelectedInternalModTypeGroup = value;
                OnPropertyChanged("SelectedInternalModTypeGroup");
                if (SelectedGameDataItem != null)
                {
                    ShowSlots();
                }
            }
        }
        public GameData SelectedGameData
        {
            get => _SelectedGameData;
            set
            {
                if (_SelectedGameData == value)
                    return;

                _SelectedGameData = value;
                OnPropertyChanged("SelectedGameData");
            }
        }
        public GameDataUXItem SelectedGameDataCategory

        {
            get => _SelectedGameDataCategory;
            set
            {
                SelectionVisible = value == null;

                if (_SelectedGameDataCategory == value)
                    return;

                if (value != null)
                {
                    ItemsCollectionViewSource.Source = value.GameDataCategory.Items;
                    ItemsCollectionViewSource.View.MoveCurrentToFirst();    

                    SelectedInternalModTypes = new ObservableCollection<InternalModType>(InternalModTypes.Where(i => i.Association == value.GameDataCategory.ID));
                    SelectedInternalModTypeGroups = new ObservableCollection<InternalModTypeGroup>();
                    foreach (InternalModType imt in SelectedInternalModTypes)
                    {
                        if (SelectedInternalModTypeGroups.Count == 0)
                        {
                            InternalModTypeGroup group = new InternalModTypeGroup() { InternalModTypes = new ObservableCollection<InternalModType>() { imt }, Name = imt.TypeGroup };
                            SelectedInternalModTypeGroups.Add(group);
                        }
                        else
                        {
                            if (SelectedInternalModTypeGroups.Any(g => g.Name == imt.TypeGroup))
                            {
                                InternalModTypeGroup group = SelectedInternalModTypeGroups.First(g => g.Name == imt.TypeGroup);
                                group.InternalModTypes.Add(imt);
                            }
                            else
                            {
                                InternalModTypeGroup group = new InternalModTypeGroup() { InternalModTypes = new ObservableCollection<InternalModType>() { imt }, Name = imt.TypeGroup };
                                SelectedInternalModTypeGroups.Add(group);
                            }
                        }
                    }

                SelectedInternalModType = SelectedInternalModTypes[0];
                SelectedInternalModTypeGroup = SelectedInternalModTypeGroups[0];

                }

                _SelectedGameDataCategory = value;
                OnPropertyChanged("SelectedGameDataCategory");
            }
        }

        #endregion

        #region Commands
        public ICommand SelectItemCommand
        {
            get
            {
                if (_SelectItemCommand == null)
                {
                    _SelectItemCommand = new RelayCommand(param => goSelection());
                }
                return _SelectItemCommand;
            }
        }
        #endregion

        public ILog Log { get; set; }
        #endregion

        public AssociationViewModel(ObservableCollection<GameData> _GameDatas, ObservableCollection<InternalModType> _InternalModTypes, ObservableCollection<Workspace> _Workspaces, Workspace _ActiveWorkspace, ObservableCollection<ContentMapping> _ContentMappings)
        {
            Workspaces = _Workspaces;
            ActiveWorkspace = _ActiveWorkspace;
            ContentMappings = _ContentMappings;

            GameDatas = _GameDatas;
            InternalModTypes = _InternalModTypes;
            SelectedGameData = GameDatas[0];
            GameDataUXs = new ObservableCollection<GameDataUXItem>();
            foreach(GameDataCategory c in SelectedGameData.Categories)
            {
                if(c.ID != 0)
                {
                    GameDataUXs.Add(new GameDataUXItem() { GameDataCategory = c, ImageSource = new Uri(Properties.Settings.Default.DefaultDir + @"\References\images\Games\" + c.image), HoverImageSource = new Uri(Properties.Settings.Default.DefaultDir + @"\References\images\Games\" + c.image.Split('.')[0]+"_selected.png") });
                }
            }
            FilterText = "";

            ItemsCollectionViewSource = new CollectionViewSource();
            ItemsCollectionViewSource.Filter += FilterItems;
            ItemsCollectionViewSource.IsLiveFilteringRequested = true;
            ItemsCollectionViewSource.SortDescriptions.Add(new System.ComponentModel.SortDescription("Name",System.ComponentModel.ListSortDirection.Ascending));
            SelectionVisible = true;

            EventSystem.Subscribe<Workspace>(SetActiveWorkspace);
            TypesGrouped = Properties.Settings.Default.GroupInternalModTypes;
        }

        #region Actions
        public void goSelection()
        {
            SelectedGameDataCategory = null;
        }

        public void SetSlot(SlotItem SourceItem, SlotItem AvailableSlotItem, bool Save = false)
        {
            SlotItemViewModel SlotItemViewModel = AvailableSlotItem.SlotItemViewModel != null? AvailableSlotItem.SlotItemViewModel : (SlotItemViewModel)AvailableSlotItem.DataContext;
            int AvailableSlotIndex = SlotItemViewModel.Index;
            AvailableSlots.RemoveAt(AvailableSlotIndex);
            AvailableSlots.Insert(AvailableSlotIndex, new SlotItem()
            {
                SlotItemViewModel = new SlotItemViewModel()
                {
                    ContentName = SourceItem.SlotItemViewModel.ContentName,
                    EmptySlot = false,
                    SlotNumber = SlotItemViewModel.SlotNumber,
                    SlotNumberName = SlotItemViewModel.SlotNumberName,
                    Index = SlotItemViewModel.Index,
                    ContentMappings = SourceItem.SlotItemViewModel.ContentMappings
                }
            });
            if (Save)
            {
                SetSlotAssociations(AvailableSlots[AvailableSlotIndex]);
            }
            
        }

        public void SetSlotAssociations(SlotItem item)
        {
            try
            {
                if(item.SlotItemViewModel.ContentMappings.Count == 1)
                {
                    //Single Type addition
                    ContentMapping cm = item.SlotItemViewModel.ContentMappings[0];
                    Association a = ActiveWorkspace.Associations.SingleOrDefault(az => az.InternalModTypeID == cm.InternalModType && az.Slot == item.SlotItemViewModel.Index && az.GameDataItemID == cm.GameDataItemID);
                    if(a != null)
                    {
                        a.ContentMappingID = cm.ID;
                    }
                    else
                    {
                        ActiveWorkspace.Associations.Add(new Association()
                        {
                            ContentMappingID = cm.ID,
                            GameDataItemID = cm.GameDataItemID,
                            InternalModTypeID = cm.InternalModType,
                            Slot = item.SlotItemViewModel.Index
                        });
                    }
                }
                else
                {
                    //Grouped Types Addition
                    List<Association> La = ActiveWorkspace.Associations.FindAll(az => az.Slot == item.SlotItemViewModel.Index && az.GameDataItemID == item.SlotItemViewModel.ContentMappings[0].GameDataItemID);
                    foreach(Association a in La)
                    {
                        ActiveWorkspace.Associations.Remove(a);
                    }
                    foreach(ContentMapping cm in item.SlotItemViewModel.ContentMappings)
                    {
                        ActiveWorkspace.Associations.Add(new Association()
                        {
                            ContentMappingID = cm.ID,
                            GameDataItemID = cm.GameDataItemID,
                            InternalModTypeID = cm.InternalModType,
                            Slot = item.SlotItemViewModel.Index
                        });
                    }
                }
                WorkspaceXML.WriteWorkspaces(Workspaces.ToList());
            }
            catch(Exception e)
            {
                Log.Error(e.Message);
            }
        }


        public void FilterItems(object sender, FilterEventArgs e)
        {
            GameDataItem gdi = e.Item as GameDataItem;
            if (FilterText != "")
            {
                if (gdi.Name.ToLower().Contains(FilterText.ToLower()))
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

        public void ShowSlots()
        {
            if (SelectedGameDataItem != null && (SelectedInternalModType != null && !TypesGrouped || SelectedInternalModTypeGroup != null && TypesGrouped))
            {
                AddEmptySlots();
                FillAssociationSlots();
            }
            else
            {
                SlotItems = new ObservableCollection<SlotItem>();
                AvailableSlots = new ObservableCollection<SlotItem>();
            }

        }

        public void AddEmptySlots()
        {
            AvailableSlots = new ObservableCollection<SlotItem>();
            if (TypesGrouped)
            {
                for (int i = 0; i < SelectedInternalModTypeGroup.InternalModTypes[0].Slots; i++)
                {
                    AvailableSlots.Add(new SlotItem()
                    {
                        SlotItemViewModel = new SlotItemViewModel()
                        {
                            ContentName = "Empty",
                            SlotNumber = i + 1,
                            Index = i,
                            SlotNumberName = i > 10 ? (i + 1 % 10).ToString() : (i + 1).ToString(),
                            EmptySlot = true
                        }
                    });
                }


            }
            else
            {
                for (int i = 0; i < SelectedInternalModType.Slots; i++)
                {
                    AvailableSlots.Add(new SlotItem()
                    {
                        SlotItemViewModel = new SlotItemViewModel()
                        {
                            ContentName = "Empty",
                            SlotNumber = i > 10 ? ((i + 1) % 10) : (i + 1),
                            Index = i,
                            SlotNumberName = (i+1).ToString(),
                            EmptySlot = true
                        }
                    });
                }
            }
        }
        public void FillAssociationSlots()
        {
            SlotItems = new ObservableCollection<SlotItem>();

            if (!TypesGrouped)
            {
                List<ContentMapping> contentMappings = ContentMappings.Where(a => a.InternalModType == SelectedInternalModType.ID && a.GameDataItemID == SelectedGameDataItem.ID).ToList();

                foreach (ContentMapping cm in contentMappings)
                {
                    SlotItems.Add(new SlotItem()
                    {
                        SlotItemViewModel = new SlotItemViewModel()
                        {
                            ContentName = cm.Name,
                            SlotNumber = cm.Slot + 1,
                            SlotNumberName = cm.Slot > 10 ? (cm.Slot + 1 % 10).ToString() : (cm.Slot + 1).ToString(),
                            EmptySlot = false,
                            ContentMappings = new List<ContentMapping>()
                            {
                                cm
                            }
                        }
                    });
                }



                List<Association> Associations = ActiveWorkspace.Associations.Where(a => a.InternalModTypeID == SelectedInternalModType.ID && a.GameDataItemID == SelectedGameDataItem.ID).ToList();
                foreach (Association ass in Associations)
                {
                    ContentMapping cm = contentMappings.Single(c => c.ID == ass.ContentMappingID);
                    SlotItem SI = new SlotItem()
                    {
                        SlotItemViewModel = new SlotItemViewModel()
                        {
                            ContentName = cm.Name,
                            SlotNumber = ass.Slot + 1,
                            SlotNumberName = ass.Slot > 10 ? (ass.Slot + 1 % 10).ToString() : (ass.Slot + 1).ToString(),
                            EmptySlot = false,
                            ContentMappings = new List<ContentMapping>()
                            {
                                cm
                            }
                        }
                    };
                    SetSlot(SI, AvailableSlots[ass.Slot]);
                }
            }
            else
            {
                List<ContentMapping> contentMapping = new List<ContentMapping>();
                foreach(InternalModType IMT in SelectedInternalModTypeGroup.InternalModTypes)
                {
                    List<ContentMapping> local = ContentMappings.Where(a => a.InternalModType == IMT.ID && a.GameDataItemID == SelectedGameDataItem.ID).ToList();
                    contentMapping.AddRange(local);
                }
                List<int> ProcessedSlots = new List<int>();

                foreach(ContentMapping cm in contentMapping)
                {
                    if (!ProcessedSlots.Contains(cm.Slot))
                    {
                        ProcessedSlots.Add(cm.Slot);
                        SlotItems.Add(new SlotItem()
                        {
                            SlotItemViewModel = new SlotItemViewModel()
                            {
                                ContentName = cm.Name,
                                SlotNumber = cm.Slot + 1,
                                SlotNumberName = cm.Slot > 10 ? (cm.Slot + 1 % 10).ToString() : (cm.Slot + 1).ToString(),
                                EmptySlot = false,
                                ContentMappings = new List<ContentMapping>()
                                {
                                    cm
                                }
                            }
                        });
                    }
                    else
                    {
                        SlotItem item = SlotItems.Single(i => i.SlotItemViewModel.SlotNumber == (cm.Slot + 1));
                        item.SlotItemViewModel.ContentMappings.Add(cm);
                    }
                }

                ProcessedSlots = new List<int>();
                foreach (InternalModType IMT in SelectedInternalModTypeGroup.InternalModTypes)
                {
                    List<Association> Associations = ActiveWorkspace.Associations.Where(a => a.InternalModTypeID == IMT.ID && a.GameDataItemID == SelectedGameDataItem.ID).ToList();
                    foreach (Association ass in Associations)
                    {
                        ContentMapping cm = contentMapping.Single(c => c.ID == ass.ContentMappingID);

                        if (!ProcessedSlots.Contains(ass.Slot))
                        {
                            ProcessedSlots.Add(ass.Slot);

                            
                            SlotItem SI = new SlotItem()
                            {
                                SlotItemViewModel = new SlotItemViewModel()
                                {
                                    ContentName = cm.Name,
                                    SlotNumber = ass.Slot + 1,
                                    SlotNumberName = ass.Slot > 10 ? (ass.Slot + 1 % 10).ToString() : (ass.Slot + 1).ToString(),
                                    EmptySlot = false,
                                    ContentMappings = new List<ContentMapping>()
                                    {
                                        cm
                                    }
                                }
                            };
                            SetSlot(SI, AvailableSlots[ass.Slot]);
                        }
                        else
                        {
                            AvailableSlots[ass.Slot].SlotItemViewModel.ContentMappings.Add(cm);
                        }
                    }
                }

                


            }

        }
        public void DeleteSlotItem()
        {
            List<Association> Associations = new List<Association>();
            foreach(ContentMapping cm in SelectedSlotItem.SlotItemViewModel.ContentMappings)
            {
                Association a = ActiveWorkspace.Associations.SingleOrDefault(az => az.InternalModTypeID == cm.InternalModType && az.Slot == SelectedSlotItem.SlotItemViewModel.Index && az.GameDataItemID == cm.GameDataItemID);
                if(a != null)
                {
                    ActiveWorkspace.Associations.Remove(a);
                }
            }

            int index = AvailableSlots.IndexOf(SelectedSlotItem);
            SlotItem EmptyItem = new SlotItem()
            {
                SlotItemViewModel = new SlotItemViewModel()
                {
                    ContentName = "Empty",
                    SlotNumber = AvailableSlots[index].SlotItemViewModel.SlotNumber,
                    Index = AvailableSlots[index].SlotItemViewModel.Index,
                    SlotNumberName = AvailableSlots[index].SlotItemViewModel.SlotNumberName,
                    EmptySlot = true
                }
            };

            AvailableSlots.RemoveAt(index);
            AvailableSlots.Insert(index, EmptyItem);

            WorkspaceXML.WriteWorkspaces(Workspaces.ToList());
        }
        public void SetActiveWorkspace(Workspace w)
        {
            ActiveWorkspace = w;
            ShowSlots();
        }
        #endregion

    }
}

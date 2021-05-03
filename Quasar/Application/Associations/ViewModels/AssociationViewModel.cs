using log4net;
using Quasar.Associations.Models;
using Quasar.Associations.ViewModels;
using Quasar.Associations.Views;
using Quasar.Common.Models;
using Quasar.Data.V2;
using Quasar.Helpers.Json;
using Quasar.Helpers;
using Quasar.MainUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;

namespace Quasar.Associations.ViewModels
{
    public class AssociationViewModel : ObservableObject
    {
        #region View

        #region Private

        private ObservableCollection<GameElementFamilySquare> _GameFamilySquareCollection { get; set; }
        private GameElementFamilySquare _SelectedGameElementFamilySquare { get; set; }
        private GameElementFamily _SelectedGameElementFamily { get; set; }
        private GameElement _SelectedGameElement { get; set; }
        private ObservableCollection<Slot> _SlotItems { get; set; }
        private Slot _SelectedSlotItem { get; set; }
        private ObservableCollection<Slot> _AvailableSlots { get; set; }
        private CollectionViewSource _ItemsCollectionViewSource { get; set; }
        private CollectionViewSource _FilterCollectionViewSource { get; set; }
        private ObservableCollection<FilterItem> _FilterValues { get; set; }
        private FilterItem _SelectedFilterItem { get; set; }
        private bool _SelectionVisible { get; set; }
        private bool _TypesGrouped { get; set; }
        private bool _ItemsWithStuff { get; set; }
        private string _FilterText { get; set; }
        private string _ViewType { get; set; }
        private string _FilterName { get; set; }
        #endregion

        #region Public
        public ObservableCollection<GameElementFamilySquare> GameFamilySquareCollection
        {
            get => _GameFamilySquareCollection;
            set
            {
                if (_GameFamilySquareCollection == value)
                    return;

                _GameFamilySquareCollection = value;
                OnPropertyChanged("GameFamilySquareCollection");
            }
        }
        public GameElement SelectedGameElement
        {
            get => _SelectedGameElement;
            set
            {
                if (_SelectedGameElement == value)
                    return;

                _SelectedGameElement = value;
                OnPropertyChanged("SelectedGameElement");
                if (TypesGrouped && SelectedQuasarModTypeGroup != null || !TypesGrouped && SelectedQuasarModType != null)
                {
                    RefreshSlotData();
                }
            }
        }
        public GameElementFamily SelectedGameElementFamily
        {
            get => _SelectedGameElementFamily;
            set
            {
                if (_SelectedGameElementFamily == value)
                    return;

                _SelectedGameElementFamily = value;
                OnPropertyChanged("SelectedGameElementFamily");
                if (TypesGrouped && SelectedQuasarModTypeGroup != null || !TypesGrouped && SelectedQuasarModType != null)
                {
                    RefreshSlotData();
                }
            }
        }
        public ObservableCollection<Slot> SlotItems
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
        public Slot SelectedSlotItem
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
        public ObservableCollection<Slot> AvailableSlots
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
        public CollectionViewSource FilterCollectionViewSource

        {
            get => _FilterCollectionViewSource;
            set
            {
                if (_FilterCollectionViewSource == value)
                    return;

                _FilterCollectionViewSource = value;
                OnPropertyChanged("FilterCollectionViewSource");
            }
        }
        public ObservableCollection<FilterItem> FilterValues

        {
            get => _FilterValues;
            set
            {
                if (_FilterValues == value)
                    return;

                _FilterValues = value;
                OnPropertyChanged("FilterValues");
            }
        }
        public FilterItem SelectedFilterItem
        {
            get => _SelectedFilterItem;
            set
            {
                _SelectedFilterItem = value;
                if (ItemsCollectionViewSource != null)
                {
                    ItemsCollectionViewSource.View.Refresh();
                }
                OnPropertyChanged("SelectedFilterItem");
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
                    if (QuasarModTypeGroupCollection != null)
                        SelectedQuasarModTypeGroup = QuasarModTypeGroupCollection[0];
                }
                else
                {
                    if (QuasarModTypeCollection != null)
                        SelectedQuasarModType = QuasarModTypeCollection[0];
                }
                RefreshSlotData();
            }
        }
        public bool ItemsWithStuff
        {
            get => _ItemsWithStuff;
            set
            {
                if (_ItemsWithStuff == value)
                    return;

                _ItemsWithStuff = value;

                if (ItemsCollectionViewSource != null)
                {
                    ItemsCollectionViewSource.View.Refresh();
                }
                OnPropertyChanged("ItemsWithStuff");

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
        public string ViewType
        {
            get => _ViewType;
            set
            {
                if (_ViewType == value)
                    return;

                _ViewType = value;

                OnPropertyChanged("ViewType");
            }
        }
        public string FilterName
        {
            get => _FilterName;
            set
            {
                if (_FilterName == value)
                    return;

                _FilterName = value;

                OnPropertyChanged("FilterName");
            }
        }
        public string dragimage { get; set; } = Properties.Settings.Default.DefaultDir + @"\Resources\images\drag.png";
        public string deleteimage { get; set; } = Properties.Settings.Default.DefaultDir + @"\Resources\images\Delete.png";
        #endregion

        #endregion

        #region Data

        #region Private
        private MainUIViewModel _MUVM { get; set; }
        private ObservableCollection<QuasarModType> _QuasarModTypeCollection { get; set; }
        private QuasarModType _SelectedQuasarModType { get; set; }
        private ObservableCollection<QuasarModTypeGroup> _SelectedQuasarModTypeGroupCollection { get; set; }
        private QuasarModTypeGroup _SelectedQuasarModTypeGroup { get; set; }
        #endregion

        #region Public

        public MainUIViewModel MUVM
        {
            get => _MUVM;
            set
            {
                _MUVM = value;
                OnPropertyChanged("MUVM");
            }
        }
        public ObservableCollection<QuasarModType> QuasarModTypeCollection
        {
            get => _QuasarModTypeCollection;
            set
            {
                if (_QuasarModTypeCollection == value)
                    return;

                _QuasarModTypeCollection = value;
                OnPropertyChanged("QuasarModTypeCollection");
            }
        }
        public QuasarModType SelectedQuasarModType
        {
            get => _SelectedQuasarModType;
            set
            {
                if (_SelectedQuasarModType == value)
                    return;

                _SelectedQuasarModType = value;
                OnPropertyChanged("SelectedQuasarModType");
                if (SelectedGameElement != null)
                {
                    RefreshSlotData();
                }

            }
        }
        public ObservableCollection<QuasarModTypeGroup> QuasarModTypeGroupCollection
        {
            get => _SelectedQuasarModTypeGroupCollection;
            set
            {
                if (_SelectedQuasarModTypeGroupCollection == value)
                    return;

                _SelectedQuasarModTypeGroupCollection = value;
                OnPropertyChanged("QuasarModTypeGroupCollection");
            }
        }
        public QuasarModTypeGroup SelectedQuasarModTypeGroup
        {
            get => _SelectedQuasarModTypeGroup;
            set
            {
                if (_SelectedQuasarModTypeGroup == value)
                    return;

                _SelectedQuasarModTypeGroup = value;
                OnPropertyChanged("SelectedQuasarModTypeGroup");
                if (SelectedGameElement != null)
                {
                    RefreshSlotData();
                }
            }
        }
        public GameElementFamilySquare SelectedGameElementFamilySquare

        {
            get => _SelectedGameElementFamilySquare;
            set
            {
                SelectionVisible = value == null;

                if (_SelectedGameElementFamilySquare == value)
                    return;

                if (value != null)
                {

                    if (value.GameElementFamily.Name == "Others")
                    {
                        TypesGrouped = false;
                    }
                    else
                    {
                        TypesGrouped = true;
                    }


                    ItemsCollectionViewSource.Source = value.GameElementFamily.GameElements;
                    SelectedGameElementFamily = value.GameElementFamily;
                    ItemsCollectionViewSource.View.MoveCurrentToFirst();

                    QuasarModTypeCollection = new ObservableCollection<QuasarModType>(MUVM.QuasarModTypes.Where(i => i.GameElementFamilyID == value.GameElementFamily.ID));
                    QuasarModTypeGroupCollection = new ObservableCollection<QuasarModTypeGroup>();
                    foreach (QuasarModType qmt in QuasarModTypeCollection)
                    {
                        if (QuasarModTypeGroupCollection.Count == 0)
                        {
                            QuasarModTypeGroup group = new QuasarModTypeGroup() { QuasarModTypeCollection = new ObservableCollection<QuasarModType>() { qmt }, Name = qmt.GroupName };
                            QuasarModTypeGroupCollection.Add(group);
                        }
                        else
                        {
                            if (QuasarModTypeGroupCollection.Any(g => g.Name == qmt.GroupName))
                            {
                                QuasarModTypeGroup group = QuasarModTypeGroupCollection.First(g => g.Name == qmt.GroupName);
                                group.QuasarModTypeCollection.Add(qmt);
                            }
                            else
                            {
                                QuasarModTypeGroup group = new QuasarModTypeGroup() { QuasarModTypeCollection = new ObservableCollection<QuasarModType>() { qmt }, Name = qmt.GroupName };
                                QuasarModTypeGroupCollection.Add(group);
                            }
                        }
                    }

                    SelectedQuasarModType = QuasarModTypeCollection[0];
                    SelectedQuasarModTypeGroup = QuasarModTypeGroupCollection[0];

                }
                else
                {
                    GetSlottedSoloContent(true);
                }

                _SelectedGameElementFamilySquare = value;
                OnPropertyChanged("SelectedGameElementFamilySquare");

                if (value != null)
                {
                    ViewType = value?.GameElementFamily.ViewType ?? "";
                    FilterName = value?.GameElementFamily.FilterName ?? "";
                    OnPropertyChanged("FilterName");
                    OnPropertyChanged("ViewType");

                    FilterValues = new ObservableCollection<FilterItem>();
                    FilterValues.Add(new FilterItem() { Name = "Any" });
                    foreach (GameElement ge in value?.GameElementFamily.GameElements)
                    {
                        if (ge.FilterValue != null)
                        {
                            if (!FilterValues.Any(f => f.Name == ge.FilterValue))
                            {
                                FilterValues.Add(new FilterItem() { Name = ge.FilterValue });
                            }
                        }
                    }

                    FilterCollectionViewSource.Source = FilterValues;
                }

            }
        }

        #endregion

        #endregion

        #region Commands

        #region Private
        private ICommand _SelectItemCommand { get; set; }
        #endregion

        #region Public
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

        #endregion

        #region Slots

        #region Private

        private SlotViewModel _Slot0ViewModel { get; set; }
        private SlotViewModel _Slot1ViewModel { get; set; }
        private SlotViewModel _Slot2ViewModel { get; set; }
        private SlotViewModel _Slot3ViewModel { get; set; }
        private SlotViewModel _Slot4ViewModel { get; set; }
        private SlotViewModel _Slot5ViewModel { get; set; }
        private SlotViewModel _Slot6ViewModel { get; set; }
        private SlotViewModel _Slot7ViewModel { get; set; }
        private SlotViewModel _Slot8ViewModel { get; set; }

        #endregion

        #region Public
        public SlotViewModel Slot1ViewModel
        {
            get => _Slot1ViewModel;
            set
            {
                _Slot1ViewModel = value;
                OnPropertyChanged("Slot1ViewModel");
            }
        }
        public SlotViewModel Slot2ViewModel
        {
            get => _Slot2ViewModel;
            set
            {
                _Slot2ViewModel = value;
                OnPropertyChanged("Slot2ViewModel");
            }
        }
        public SlotViewModel Slot3ViewModel
        {
            get => _Slot3ViewModel;
            set
            {
                _Slot3ViewModel = value;
                OnPropertyChanged("Slot3ViewModel");
            }
        }
        public SlotViewModel Slot4ViewModel
        {
            get => _Slot4ViewModel;
            set
            {
                _Slot4ViewModel = value;
                OnPropertyChanged("Slot4ViewModel");
            }
        }
        public SlotViewModel Slot5ViewModel
        {
            get => _Slot5ViewModel;
            set
            {
                _Slot5ViewModel = value;
                OnPropertyChanged("Slot5ViewModel");
            }
        }
        public SlotViewModel Slot6ViewModel
        {
            get => _Slot6ViewModel;
            set
            {
                _Slot6ViewModel = value;
                OnPropertyChanged("Slot6ViewModel");
            }
        }
        public SlotViewModel Slot7ViewModel
        {
            get => _Slot7ViewModel;
            set
            {
                _Slot7ViewModel = value;
                OnPropertyChanged("Slot7ViewModel");
            }
        }
        public SlotViewModel Slot8ViewModel
        {
            get => _Slot8ViewModel;
            set
            {
                _Slot8ViewModel = value;
                OnPropertyChanged("Slot8ViewModel");
            }
        }
        #endregion

        #endregion

        public ILog QuasarLogger { get; set; }

        /// <summary>
        /// Association View Model Constructor
        /// </summary>
        /// <param name="_MUVM">MainUI View Model to link</param>
        public AssociationViewModel(MainUIViewModel _MUVM, ILog _QuasarLogger)
        {
            QuasarLogger = _QuasarLogger;
            MUVM = _MUVM;
            GameFamilySquareCollection = new ObservableCollection<GameElementFamilySquare>();
            foreach(GameElementFamily gef in MUVM.Games[0].GameElementFamilies)
            {
                if(gef.ID != 0)
                {
                    GameFamilySquareCollection.Add(new GameElementFamilySquare() { GameElementFamily = gef, ImageSource = new Uri(Properties.Settings.Default.DefaultDir + @"\Resources\images\Games\" + gef.ImagePath), HoverImageSource = new Uri(Properties.Settings.Default.DefaultDir + @"\Resources\images\Games\" + gef.ImagePath.Split('.')[0]+"_selected.png") });
                }
            }
            FilterText = "";

            ItemsCollectionViewSource = new CollectionViewSource();
            ItemsCollectionViewSource.Filter += FilterItems;
            ItemsCollectionViewSource.IsLiveFilteringRequested = true;
            ItemsCollectionViewSource.SortDescriptions.Add(new System.ComponentModel.SortDescription("Name",System.ComponentModel.ListSortDirection.Ascending));
            SelectionVisible = true;

            FilterCollectionViewSource = new CollectionViewSource();
            FilterCollectionViewSource.SortDescriptions.Add(new System.ComponentModel.SortDescription("Filter", System.ComponentModel.ListSortDirection.Ascending));

            EventSystem.Subscribe<Workspace>(SetActiveWorkspace);
            EventSystem.Subscribe<SlotViewModel>(SlotViewModelAction);
            TypesGrouped = Properties.Settings.Default.GroupInternalModTypes;

            Slot1ViewModel = new SlotViewModel(QuasarLogger)
            {
                ContentName = "Empty",
                SlotNumber = 0,
                Index = 0,
                SlotNumberName = (1).ToString(),
                QuasarModTypes = MUVM.QuasarModTypes,
                EmptySlot = true
            };
            Slot2ViewModel = new SlotViewModel(QuasarLogger)
            {
                ContentName = "Empty",
                SlotNumber = 1,
                Index = 1,
                SlotNumberName = (2).ToString(),
                QuasarModTypes = MUVM.QuasarModTypes,
                EmptySlot = true
            };
            Slot3ViewModel = new SlotViewModel(QuasarLogger)
            {
                ContentName = "Empty",
                SlotNumber = 2,
                Index = 2,
                SlotNumberName = (3).ToString(),
                QuasarModTypes = MUVM.QuasarModTypes,
                EmptySlot = true
            };
            Slot4ViewModel = new SlotViewModel(QuasarLogger)
            {
                ContentName = "Empty",
                SlotNumber = 3,
                Index = 3,
                SlotNumberName = (4).ToString(),
                QuasarModTypes = MUVM.QuasarModTypes,
                EmptySlot = true
            };
            Slot5ViewModel = new SlotViewModel(QuasarLogger)
            {
                ContentName = "Empty",
                SlotNumber = 4,
                Index = 4,
                SlotNumberName = (5).ToString(),
                QuasarModTypes = MUVM.QuasarModTypes,
                EmptySlot = true
            };
            Slot6ViewModel = new SlotViewModel(QuasarLogger)
            {
                ContentName = "Empty",
                SlotNumber = 5,
                Index = 5,
                SlotNumberName = (6).ToString(),
                QuasarModTypes = MUVM.QuasarModTypes,
                EmptySlot = true
            };
            Slot7ViewModel = new SlotViewModel(QuasarLogger)
            {
                ContentName = "Empty",
                SlotNumber = 6,
                Index = 6,
                SlotNumberName = (7).ToString(),
                QuasarModTypes = MUVM.QuasarModTypes,
                EmptySlot = true
            };
            Slot8ViewModel = new SlotViewModel(QuasarLogger)
            {
                ContentName = "Empty",
                SlotNumber = 7,
                Index = 7,
                SlotNumberName = (8).ToString(),
                QuasarModTypes = MUVM.QuasarModTypes,
                EmptySlot = true
            };
        }

        #region Actions    
        
        /// <summary>
        /// Automatic Filter function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void FilterItems(object sender, FilterEventArgs e)
        {
            GameElement ge = e.Item as GameElement;
            if (FilterText != "")
            {
                if (ge.Name.ToLower().Contains(FilterText.ToLower()))
                {
                    if (ItemsWithStuff)
                    {
                        GameElementFamily family = MUVM.Games[0].GameElementFamilies.Single(gef => gef.GameElements.Contains(ge));
                        List<QuasarModType> qmt = MUVM.QuasarModTypes.Where(i => i.GameElementFamilyID == family.ID).ToList();
                        List<int> qmtIDList = new List<int>();
                        foreach (QuasarModType local_qmt in qmt)
                        {
                            qmtIDList.Add(local_qmt.ID);
                        }
                        if (MUVM.ActiveWorkspace.Associations.Any(a => a.GameElementID == ge.ID && qmtIDList.Contains(a.QuasarModTypeID)))
                        {
                            if (ge.FilterValue == SelectedFilterItem?.Name || SelectedFilterItem?.Name == "Any")
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
                            e.Accepted = false;
                        }
                    }
                    else
                    {
                        if (ge.FilterValue == SelectedFilterItem?.Name || SelectedFilterItem?.Name == "Any")
                        {
                            e.Accepted = true;
                        }
                        else
                        {
                            e.Accepted = false;
                        }
                    }
                    
                }
                else
                {
                    e.Accepted = false;
                }
            }
            else
            {
                if (ItemsWithStuff)
                {
                    GameElementFamily family = MUVM.Games[0].GameElementFamilies.Single(gef => gef.GameElements.Contains(ge));
                    List<QuasarModType> qmt = MUVM.QuasarModTypes.Where(i => i.GameElementFamilyID == family.ID).ToList();
                    List<int> qmtIDList = new List<int>();
                    foreach (QuasarModType local_qmt in qmt)
                    {
                        qmtIDList.Add(local_qmt.ID);
                    }
                    if (MUVM.ActiveWorkspace.Associations.Any(a => a.GameElementID == ge.ID && qmtIDList.Contains(a.QuasarModTypeID)))
                    {
                        if(ge.FilterValue == SelectedFilterItem?.Name || SelectedFilterItem?.Name == "Any")
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
                        e.Accepted = false;
                    }
                }
                else
                {
                    if (ge.FilterValue == SelectedFilterItem?.Name || SelectedFilterItem?.Name == "Any")
                    {
                        e.Accepted = true;
                    }
                    else
                    {
                        e.Accepted = false;
                    }
                }
            }
        }

        /// <summary>
        /// Refreshes the Slots contents and UI
        /// </summary>
        public void RefreshSlotData()
        {
            if (SelectedGameElement != null && (SelectedQuasarModType != null && !TypesGrouped || SelectedQuasarModTypeGroup != null && TypesGrouped))
            {
                GetAvailableSlots();
                GetSlottedSoloContent();
            }
            else
            {
                SlotItems = new ObservableCollection<Slot>();
                AvailableSlots = new ObservableCollection<Slot>();
            }

        }

        /// <summary>
        /// Refreshes the Available Slot list
        /// </summary>
        public void GetAvailableSlots()
        {
            if (!TypesGrouped)
            {
                SlotItems = GetAvailableSoloContent();
            }
            else
            {
                SlotItems = GetAvailableGroupedContent();
            }

        }

        /// <summary>
        /// Gets the new Available Slots Collection as Solo elements
        /// </summary>
        /// <returns>Observable Collection of Slots</returns>
        public ObservableCollection<Slot> GetAvailableSoloContent()
        {
            ObservableCollection<Slot> SoloContent = new ObservableCollection<Slot>();

            //Listing corresponding Content Items
            List<ContentItem> ContentItems = new List<ContentItem>();
            if (SelectedQuasarModType.IgnoreGameElementFamily)
            {
                ContentItems = MUVM.ContentItems.Where(a => a.QuasarModTypeID == SelectedQuasarModType.ID).ToList();
            }
            else
            {
                ContentItems = MUVM.ContentItems.Where(a => a.QuasarModTypeID == SelectedQuasarModType.ID && a.GameElementID == SelectedGameElement.ID).ToList();
            }

            //Adding them as Slots
            foreach (ContentItem ci in ContentItems)
            {
                SoloContent.Add(new Slot()
                {
                    SlotViewModel = new SlotViewModel(QuasarLogger)
                    {
                        ContentName = ci.Name,
                        SlotNumber = ci.SlotNumber + 1,
                        SlotNumberName = ci.SlotNumber > 10 ? (ci.SlotNumber + 1 % 10).ToString() : (ci.SlotNumber + 1).ToString(),
                        TypeName = MUVM.QuasarModTypes.Single(mt => mt.ID == ci.QuasarModTypeID).Name,
                        EmptySlot = false,
                        ContentItems = new List<ContentItem>()
                            {
                                ci
                            }
                    }
                });
            }

            return SoloContent;
        }

        /// <summary>
        /// Gets the new Available Slots Collection as Grouped elements
        /// </summary>
        /// <returns>Observable Collection of Slots</returns>
        public ObservableCollection<Slot> GetAvailableGroupedContent()
        {
            ObservableCollection<Slot> GroupedContent = new ObservableCollection<Slot>();

            //Parsing Content Items with a matching Mod Type Collection
            List<ContentItem> ContentItems = new List<ContentItem>();
            foreach (QuasarModType qmt in SelectedQuasarModTypeGroup.QuasarModTypeCollection)
            {
                List<ContentItem> local = new List<ContentItem>();
                if (qmt.IgnoreGameElementFamily)
                {
                    local = MUVM.ContentItems.Where(a => a.QuasarModTypeID == qmt.ID).ToList();
                }
                else
                {
                    local = MUVM.ContentItems.Where(a => a.QuasarModTypeID == qmt.ID && a.GameElementID == SelectedGameElement.ID).ToList();
                }

                ContentItems.AddRange(local);
            }

            List<int> ProcessedSlots = new List<int>();

            //Looping through every ContentItem
            foreach (ContentItem ci in ContentItems)
            {
                //Checking if no slot exists there
                if (!ProcessedSlots.Contains(ci.SlotNumber))
                {
                    //Creating a Slot
                    ProcessedSlots.Add(ci.SlotNumber);
                    QuasarModType qmt = MUVM.QuasarModTypes.Single(t => t.ID == ci.QuasarModTypeID);
                    GroupedContent.Add(new Slot()
                    {
                        SlotViewModel = new SlotViewModel(QuasarLogger)
                        {
                            ContentName = ci.Name,
                            SlotNumber = ci.SlotNumber + 1,
                            SlotNumberName = ci.SlotNumber > 10 ? (ci.SlotNumber + 1 % 10).ToString() : (ci.SlotNumber + 1).ToString(),
                            TypeName = qmt.GroupName,
                            EmptySlot = false,
                            ContentItems = new List<ContentItem>()
                                {
                                    ci
                                }
                        }
                    });
                }
                else
                {
                    //Parsing Slots with the same Slot Number
                    List<Slot> items = GroupedContent.Where(i => i.SlotViewModel.SlotNumber == (ci.SlotNumber + 1)).ToList();
                    bool added = false;

                    //Looping through matching slots
                    foreach (Slot item in items)
                    {
                        if(ci.ScanFiles[0].OriginPath.Split('\\').Length >= 3)
                        {
                            //Checking if an item exists with the same Origin Parent
                            if (item.SlotViewModel.ContentItems.Any(
                                cma => cma.LibraryItemGuid == ci.LibraryItemGuid && MatchOrigin(cma.ScanFiles[0].OriginPath, ci.ScanFiles[0].OriginPath)))
                            {
                                //Adding the Content Item to that slot
                                item.SlotViewModel.ContentItems.Add(ci);
                                added = true;
                            }
                        }
                        else
                        {
                            //Checking if an item exists with the same Origin
                            if (item.SlotViewModel.ContentItems.Any( cma => cma.LibraryItemGuid == ci.LibraryItemGuid && cma.ScanFiles[0].OriginPath == ci.ScanFiles[0].OriginPath ))
                            {
                                //Adding the Content Item to that slot
                                item.SlotViewModel.ContentItems.Add(ci);
                                added = true;
                            }
                        }
                        
                    }
                    //If no Slot matches
                    if (!added)
                    {
                        QuasarModType qmt = MUVM.QuasarModTypes.Single(t => t.ID == ci.QuasarModTypeID);
                        //Creating a Slot
                        GroupedContent.Add(new Slot()
                        {

                            SlotViewModel = new SlotViewModel(QuasarLogger)
                            {
                                ContentName = ci.Name,
                                SlotNumber = ci.SlotNumber + 1,
                                SlotNumberName = ci.SlotNumber > 10 ? (ci.SlotNumber + 1 % 10).ToString() : (ci.SlotNumber + 1).ToString(),
                                EmptySlot = false,
                                TypeName = qmt.GroupName,
                                ContentItems = new List<ContentItem>()
                                {
                                    ci
                                }
                            }
                        });
                    }
                }
            }

            return GroupedContent;
        }

        public bool MatchOrigin(string Source, string Destination)
        {
            string ParentSource = Source.Substring(0, Source.LastIndexOf('\\'));
            if (ParentSource.LastIndexOf('\\') == -1)
                return false;
            ParentSource = ParentSource.Substring(0, ParentSource.LastIndexOf('\\'));
            string ParentDestination = Destination.Substring(0, Destination.LastIndexOf('\\'));
            ParentDestination = ParentDestination.Substring(0, ParentDestination.LastIndexOf('\\'));
            return ParentDestination == ParentSource;
        }

        /// <summary>
        /// Parses all matching Content Items for each slot and sets them in the corresponding Slot View Model
        /// </summary>
        public void GetSlottedSoloContent(bool EmptySlots = false)
        {
            for(int i = 0; i < 9; i++)
            {
                List<ContentItem> contentItems = new List<ContentItem>();
                List<Association> associations = new List<Association>();

                if (!EmptySlots)
                {
                    if(SelectedGameElement != null)
                    {
                        associations = MUVM.ActiveWorkspace.Associations.Where(a => a.GameElementID == SelectedGameElement.ID
                    && a.SlotNumber == i
                    && MUVM.QuasarModTypes.Single(t => t.ID == a.QuasarModTypeID).GameElementFamilyID == SelectedGameElementFamily.ID).ToList();

                        foreach (Association a in associations)
                        {
                            contentItems.Add(MUVM.ContentItems.Single(c => c.Guid == a.ContentItemGuid));
                        }
                    }
                }              
                
                switch (i)
                {
                    case 0:
                        Slot1ViewModel.ContentItems = contentItems;
                        break;
                    case 1:
                        Slot2ViewModel.ContentItems = contentItems;
                        break;
                    case 2:
                        Slot3ViewModel.ContentItems = contentItems;
                        break;
                    case 3:
                        Slot4ViewModel.ContentItems = contentItems;
                        break;
                    case 4:
                        Slot5ViewModel.ContentItems = contentItems;
                        break;
                    case 5:
                        Slot6ViewModel.ContentItems = contentItems;
                        break;
                    case 6:
                        Slot7ViewModel.ContentItems = contentItems;
                        break;
                    case 7:
                        Slot8ViewModel.ContentItems = contentItems;
                        break;

                }
                 
            }
        }

        #endregion

        #region User Actions
        public void goSelection()
        {
            SelectedGameElementFamilySquare = null;
        }

        public void SetSlot(Slot SourceItem, Slot DestinationSlot)
        {
            SlotViewModel DestinationViewModel = (SlotViewModel)DestinationSlot.DataContext;
            int Index = DestinationViewModel.Index;
            try
            {
                if (SourceItem.SlotViewModel.ContentItems.Count == 1)
                {
                    //Single Type addition
                    ContentItem ci = SourceItem.SlotViewModel.ContentItems[0];
                    QuasarModType qmt = MUVM.QuasarModTypes.Single(t => t.ID == ci.QuasarModTypeID);
                    Association a = MUVM.ActiveWorkspace.Associations.SingleOrDefault(az => az.QuasarModTypeID == ci.QuasarModTypeID && az.SlotNumber == Index && az.GameElementID == ci.GameElementID);

                    if (a != null)
                    {
                        a.ContentItemGuid = ci.Guid;
                        MUVM.ActiveWorkspace.Associations.Remove(a);
                        QuasarLogger.Debug(String.Format("Association exists for ContentMapping ID '{0}', slot '{1}', IMT '{2}', GDIID '{3}', removing it", a.ContentItemGuid, a.SlotNumber, a.QuasarModTypeID, a.GameElementID));
                    }

                    MUVM.ActiveWorkspace.Associations.Add(new Association()
                    {
                        ContentItemGuid = ci.Guid,
                        GameElementID = SelectedGameElement.ID,
                        QuasarModTypeID = ci.QuasarModTypeID,
                        SlotNumber = Index
                    });
                    QuasarLogger.Debug(String.Format("Association created for ContentMapping '{0}' ID '{1}', slot '{2}', IMT '{3}', GDIID '{4}'", ci.Name, ci.Guid, Index, ci.QuasarModTypeID, ci.GameElementID));

                }
                else
                {
                    //Grouped Types Addition
                    List<Association> La = MUVM.ActiveWorkspace.Associations.Where(az => az.SlotNumber == Index && az.GameElementID == SourceItem.SlotViewModel.ContentItems[0].GameElementID).ToList();
                    foreach (Association a in La)
                    {
                        if(SourceItem.SlotViewModel.ContentItems.Any(item => item.QuasarModTypeID == a.QuasarModTypeID))
                        {
                            MUVM.ActiveWorkspace.Associations.Remove(a);
                            QuasarLogger.Debug(String.Format("Association exists for ContentMapping ID '{0}', slot '{1}', IMT '{2}', GDIID '{3}', removing it", a.GameElementID, a.SlotNumber, a.QuasarModTypeID, a.GameElementID));
                        }
                    }
                    foreach (ContentItem ci in SourceItem.SlotViewModel.ContentItems)
                    {
                        MUVM.ActiveWorkspace.Associations.Add(new Association()
                        {
                            ContentItemGuid = ci.Guid,
                            GameElementID = SelectedGameElement.ID,
                            QuasarModTypeID = ci.QuasarModTypeID,
                            SlotNumber = Index
                        });
                        QuasarLogger.Debug(String.Format("Association created for ContentMapping '{0}' ID '{1}', slot '{2}', IMT '{3}', GDIID '{4}'", ci.Name, ci.Guid, Index, ci.QuasarModTypeID, ci.GameElementID));

                    }
                }
                JSonHelper.SaveWorkspaces(MUVM.Workspaces);
                GetSlottedSoloContent();
            }
            catch (Exception e)
            {
                QuasarLogger.Error(e.Message);
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Responds to a trigger from a Slot View Model
        /// </summary>
        /// <param name="svm"></param>
        public void SlotViewModelAction(SlotViewModel svm)
        {
            if (svm.ContentItems != null)
            {
                RemoveAllSlotAssociations(svm);
            }
        }

        /// <summary>
        /// Removes all associations for a specific slot
        /// </summary>
        /// <param name="svm">Corresponding Slot View Model</param>
        public void RemoveAllSlotAssociations(SlotViewModel svm)
        {
            foreach (ContentItem ci in svm.ContentItems)
            {
                Association a = MUVM.ActiveWorkspace.Associations.SingleOrDefault(az => az.QuasarModTypeID == ci.QuasarModTypeID && az.SlotNumber == svm.Index && az.GameElementID == SelectedGameElement.ID);
                if (a != null)
                {
                    MUVM.ActiveWorkspace.Associations.Remove(a);
                    QuasarLogger.Debug(String.Format("Association exists for ContentMapping ID '{0}', slot '{1}', IMT '{2}', GDIID '{3}', removing it", a.ContentItemGuid, a.SlotNumber, a.QuasarModTypeID, a.GameElementID));

                }
            }

            GetSlottedSoloContent();
            JSonHelper.SaveWorkspaces(MUVM.Workspaces);
        }

        /// <summary>
        /// Refreshes the UI with the new Workspace's data
        /// </summary>
        /// <param name="w">Corresponding Workspace</param>
        public void SetActiveWorkspace(Workspace w)
        {
            RefreshSlotData();
        }

        #endregion

    }
}

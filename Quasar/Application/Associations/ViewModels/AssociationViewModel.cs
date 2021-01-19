using log4net;
using Quasar.Controls.Associations.Models;
using Quasar.Controls.Associations.ViewModels;
using Quasar.Controls.Associations.Views;
using Quasar.Controls.Common.Models;
using Quasar.Data.V2;
using Quasar.Helpers.Json;
using Quasar.Helpers.XML;
using Quasar.Internal;
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
        private ObservableCollection<GameElementFamilySquare> _GameFamilySquareCollection { get; set; }
        private GameElementFamilySquare _SelectedGameElementFamilySquare { get; set; }
        private GameElementFamily _SelectedGameElementFamily { get; set; }
        private GameElement _SelectedGameElement { get; set; }
        private ObservableCollection<SlotItem> _SlotItems { get; set; }
        private SlotItem _SelectedSlotItem { get; set; }
        private ObservableCollection<SlotItem> _AvailableSlots { get; set; }
        private CollectionViewSource _ItemsCollectionViewSource { get; set; }
        private bool _SelectionVisible { get; set; }
        private bool _TypesGrouped { get; set; }
        private bool _ItemsWithStuff { get; set; }
        private string _FilterText { get; set; }
        #endregion

        #region Data
        private MainUIViewModel _MUVM { get; set; }
        private ObservableCollection<QuasarModType> _QuasarModTypeCollection { get; set; }
        private QuasarModType _SelectedQuasarModType { get; set; }
        private ObservableCollection<QuasarModTypeGroup> _SelectedQuasarModTypeGroupCollection { get; set; }
        private QuasarModTypeGroup _SelectedQuasarModTypeGroup { get; set; }
        #endregion

        #region Commands
        private ICommand _SelectItemCommand { get; set; }
        #endregion
        
        #endregion

        #region Properties

        #region View
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
                    ShowSlots();
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
                    if(QuasarModTypeGroupCollection != null)
                        SelectedQuasarModTypeGroup = QuasarModTypeGroupCollection[0];
                }
                else
                {
                    if (QuasarModTypeCollection != null)
                        SelectedQuasarModType = QuasarModTypeCollection[0];
                }
                ShowSlots();
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

        public string dragimage { get; set; } = Properties.Settings.Default.DefaultDir + @"\Resources\images\drag.png";
        public string deleteimage { get; set; } = Properties.Settings.Default.DefaultDir + @"\Resources\images\Delete.png";
        #endregion

        #region Data
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
                    ShowSlots();
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
                    ShowSlots();
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
                    if(value.GameElementFamily.Name == "Others")
                    {
                        TypesGrouped = false;
                    }
                    else
                    {
                        TypesGrouped = true;
                    }
                    

                    ItemsCollectionViewSource.Source = value.GameElementFamily.GameElements;
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

                _SelectedGameElementFamilySquare = value;
                OnPropertyChanged("SelectedGameElementFamilySquare");
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

        public AssociationViewModel(MainUIViewModel _MUVM)
        {
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

            EventSystem.Subscribe<Workspace>(SetActiveWorkspace);
            TypesGrouped = Properties.Settings.Default.GroupInternalModTypes;
        }

        #region Actions
        public void goSelection()
        {
            SelectedGameElementFamilySquare = null;
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
                    ContentItems = SourceItem.SlotItemViewModel.ContentItems
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
                if(item.SlotItemViewModel.ContentItems.Count == 1)
                {
                    //Single Type addition
                    ContentItem ci = item.SlotItemViewModel.ContentItems[0];
                    QuasarModType qmt = MUVM.QuasarModTypes.Single(t => t.ID == ci.QuasarModTypeID);
                    Association a = MUVM.ActiveWorkspace.Associations.SingleOrDefault(az => az.QuasarModTypeID == ci.QuasarModTypeID && az.SlotNumber == item.SlotItemViewModel.Index && az.GameElementID == ci.GameElementID);
                    
                    if(a != null)
                    {
                        a.ContentItemID = ci.ID;
                        MUVM.ActiveWorkspace.Associations.Remove(a);
                        Log.Debug(String.Format("Association exists for ContentMapping ID '{0}', slot '{1}', IMT '{2}', GDIID '{3}', removing it", a.ContentItemID, a.SlotNumber, a.QuasarModTypeID, a.GameElementID));
                    }

                    MUVM.ActiveWorkspace.Associations.Add(new Association()
                    {
                        ContentItemID = ci.ID,
                        GameElementID = SelectedGameElement.ID,
                        QuasarModTypeID = ci.QuasarModTypeID,
                        SlotNumber = item.SlotItemViewModel.Index
                    });
                    Log.Debug(String.Format("Association created for ContentMapping '{0}' ID '{1}', slot '{2}', IMT '{3}', GDIID '{4}'", ci.Name, ci.ID, item.SlotItemViewModel.Index, ci.QuasarModTypeID, ci.GameElementID));
                    
                }
                else
                {
                    //Grouped Types Addition
                    List<Association> La = MUVM.ActiveWorkspace.Associations.Where(az => az.SlotNumber == item.SlotItemViewModel.Index && az.GameElementID == item.SlotItemViewModel.ContentItems[0].GameElementID).ToList();
                    foreach(Association a in La)
                    {
                        MUVM.ActiveWorkspace.Associations.Remove(a);
                        Log.Debug(String.Format("Association exists for ContentMapping ID '{0}', slot '{1}', IMT '{2}', GDIID '{3}', removing it", a.GameElementID, a.SlotNumber, a.QuasarModTypeID, a.GameElementID));
                    }
                    foreach(ContentItem ci in item.SlotItemViewModel.ContentItems)
                    {
                        MUVM.ActiveWorkspace.Associations.Add(new Association()
                        {
                            ContentItemID = ci.ID,
                            GameElementID = SelectedGameElement.ID,
                            QuasarModTypeID = ci.QuasarModTypeID,
                            SlotNumber = item.SlotItemViewModel.Index
                        });
                        Log.Debug(String.Format("Association created for ContentMapping '{0}' ID '{1}', slot '{2}', IMT '{3}', GDIID '{4}'", ci.Name, ci.ID, item.SlotItemViewModel.Index, ci.QuasarModTypeID, ci.GameElementID));

                    }
                }
                JSonHelper.SaveWorkspaces(MUVM.Workspaces);
            }
            catch(Exception e)
            {
                Log.Error(e.Message);
            }
        }


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
        }

        public void ShowSlots()
        {
            if (SelectedGameElement != null && (SelectedQuasarModType != null && !TypesGrouped || SelectedQuasarModTypeGroup != null && TypesGrouped))
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
                
                for (int i = 0; i < SelectedQuasarModTypeGroup.QuasarModTypeCollection[0].SlotCount; i++)
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
                    }) ;
                }


            }
            else
            {
                for (int i = 0; i < SelectedQuasarModType.SlotCount; i++)
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
                List<ContentItem> ContentItems = new List<ContentItem>();
                if (SelectedQuasarModType.IgnoreGameElementFamily)
                {
                    ContentItems = MUVM.ContentItems.Where(a => a.QuasarModTypeID == SelectedQuasarModType.ID).ToList();
                }
                else
                {
                    ContentItems = MUVM.ContentItems.Where(a => a.QuasarModTypeID == SelectedQuasarModType.ID && a.GameElementID == SelectedGameElement.ID).ToList();
                }
                foreach (ContentItem ci in ContentItems)
                {
                    SlotItems.Add(new SlotItem()
                    {
                        SlotItemViewModel = new SlotItemViewModel()
                        {
                            ContentName = ci.Name,
                            SlotNumber = ci.SlotNumber + 1,
                            SlotNumberName = ci.SlotNumber > 10 ? (ci.SlotNumber + 1 % 10).ToString() : (ci.SlotNumber + 1).ToString(),
                            EmptySlot = false,
                            ContentItems = new List<ContentItem>()
                            {
                                ci
                            }
                        }
                    });
                }



                List<Association> Associations = MUVM.ActiveWorkspace.Associations.Where(a => a.QuasarModTypeID == SelectedQuasarModType.ID && a.GameElementID == SelectedGameElement.ID).ToList();
                foreach (Association ass in Associations)
                {
                    ContentItem ci = ContentItems.Single(c => c.ID == ass.ContentItemID);
                    SlotItem SI = new SlotItem()
                    {
                        SlotItemViewModel = new SlotItemViewModel()
                        {
                            ContentName = ci.Name,
                            SlotNumber = ass.SlotNumber + 1,
                            SlotNumberName = ass.SlotNumber > 10 ? (ass.SlotNumber + 1 % 10).ToString() : (ass.SlotNumber + 1).ToString(),
                            EmptySlot = false,
                            ContentItems = new List<ContentItem>()
                            {
                                ci
                            }
                        }
                    };
                    SetSlot(SI, AvailableSlots[ass.SlotNumber]);
                }
            }
            else
            {
                List<ContentItem> ContentItems = new List<ContentItem>();
                foreach(QuasarModType qmt in SelectedQuasarModTypeGroup.QuasarModTypeCollection)
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

                foreach(ContentItem ci in ContentItems)
                {
                    if (!ProcessedSlots.Contains(ci.SlotNumber))
                    {
                        ProcessedSlots.Add(ci.SlotNumber);
                        SlotItems.Add(new SlotItem()
                        {
                            SlotItemViewModel = new SlotItemViewModel()
                            {
                                ContentName = ci.Name,
                                SlotNumber = ci.SlotNumber + 1,
                                SlotNumberName = ci.SlotNumber > 10 ? (ci.SlotNumber + 1 % 10).ToString() : (ci.SlotNumber + 1).ToString(),
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
                        List<SlotItem> items = SlotItems.Where(i => i.SlotItemViewModel.SlotNumber == (ci.SlotNumber + 1)).ToList();
                        bool added = false;
                        foreach(SlotItem item in items)
                        {
                            if (item.SlotItemViewModel.ContentItems.Any(cma => cma.LibraryItemID == ci.LibraryItemID))
                            {
                                item.SlotItemViewModel.ContentItems.Add(ci);
                                added = true;
                            }
                        }
                        if (!added)
                        {
                            SlotItems.Add(new SlotItem()
                            {
                                SlotItemViewModel = new SlotItemViewModel()
                                {
                                    ContentName = ci.Name,
                                    SlotNumber = ci.SlotNumber + 1,
                                    SlotNumberName = ci.SlotNumber > 10 ? (ci.SlotNumber + 1 % 10).ToString() : (ci.SlotNumber + 1).ToString(),
                                    EmptySlot = false,
                                    ContentItems = new List<ContentItem>()
                                {
                                    ci
                                }
                                }
                            });
                        }
                    }
                }

                ProcessedSlots = new List<int>();
                foreach (QuasarModType qmt in SelectedQuasarModTypeGroup.QuasarModTypeCollection)
                {
                    List<Association> Associations = MUVM.ActiveWorkspace.Associations.Where(a => a.QuasarModTypeID == qmt.ID && a.GameElementID == SelectedGameElement.ID).ToList();
                    foreach (Association ass in Associations)
                    {
                        ContentItem ci = MUVM.ContentItems.Single(c => c.ID == ass.ContentItemID);

                        if (!ProcessedSlots.Contains(ass.SlotNumber))
                        {
                            ProcessedSlots.Add(ass.SlotNumber);

                            
                            SlotItem SI = new SlotItem()
                            {
                                SlotItemViewModel = new SlotItemViewModel()
                                {
                                    ContentName = ci.Name,
                                    SlotNumber = ass.SlotNumber + 1,
                                    SlotNumberName = ass.SlotNumber > 10 ? (ass.SlotNumber + 1 % 10).ToString() : (ass.SlotNumber + 1).ToString(),
                                    EmptySlot = false,
                                    ContentItems = new List<ContentItem>()
                                    {
                                        ci
                                    }
                                }
                            };
                            SetSlot(SI, AvailableSlots[ass.SlotNumber]);
                        }
                        else
                        {
                            if(ci.LibraryItemID != AvailableSlots[ass.SlotNumber].SlotItemViewModel.ContentItems[0].LibraryItemID)
                            {
                                AvailableSlots[ass.SlotNumber].SlotItemViewModel.ContentName = "Mixed Contents";
                            }

                            AvailableSlots[ass.SlotNumber].SlotItemViewModel.ContentItems.Add(ci);
                        }
                    }
                }
            }

        }
        public void DeleteSlotItem()
        {
            if (SelectedSlotItem.SlotItemViewModel.EmptySlot)
                return;

            List<Association> Associations = new List<Association>();
            foreach(ContentItem ci in SelectedSlotItem.SlotItemViewModel.ContentItems)
            {
                Association a = MUVM.ActiveWorkspace.Associations.SingleOrDefault(az => az.QuasarModTypeID == ci.QuasarModTypeID && az.SlotNumber == SelectedSlotItem.SlotItemViewModel.Index && az.GameElementID == SelectedGameElement.ID);
                if(a != null)
                {
                    MUVM.ActiveWorkspace.Associations.Remove(a);
                    Log.Debug(String.Format("Association exists for ContentMapping ID '{0}', slot '{1}', IMT '{2}', GDIID '{3}', removing it", a.ContentItemID, a.SlotNumber, a.QuasarModTypeID, a.GameElementID));

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

            JSonHelper.SaveWorkspaces(MUVM.Workspaces);
        }
        public void SetActiveWorkspace(Workspace w)
        {
            ShowSlots();
        }
        #endregion

    }
}

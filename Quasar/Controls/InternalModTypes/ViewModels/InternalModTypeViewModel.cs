using Quasar.Controls.Common.Models;
using Quasar.FileSystem;
using Quasar.Internal;
using Quasar.XMLResources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Quasar.Controls.InternalModTypes.ViewModels
{
    public class InternalModTypeViewModel : ObservableObject
    {
        #region Fields
        private ObservableCollection<InternalModType> _InternalModTypes { get; set; }
        private ObservableCollection<Game> _Games { get; set; }
        private ObservableCollection<ModLoader> _ModLoaders { get; set; }
        private ObservableCollection<GameData> _GameDatas { get; set; }
        private GameData _CurrentGameData { get; set; }
        private InternalModType _SelectedInternalModType { get; set; }
        private InternalModTypeFile _SelectedInternalModTypeFile { get; set; }
        private BuilderFile _SelectedBuilderFile { get; set; }
        private GameDataCategory _SelectedGameDataCategory  { get; set; }
        private string _SlotString { get; set; }


        private ModListItem _SelectedModListItem { get; set; }

        private ICommand _InternalModTypeSaveCommand { get; set; }
        private ICommand _TestInternalModTypeCommand { get; set; }
        #endregion

        #region Parameters

        /// <summary>
        /// List of all InternalModTypes
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
        /// List of all Games
        /// </summary>
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
        /// <summary>
        /// List of all ModLoaders
        /// </summary>
        public ObservableCollection<ModLoader> ModLoaders
        {
            get => _ModLoaders;
            set
            {
                if (_ModLoaders == value)
                    return;

                _ModLoaders = value;
                OnPropertyChanged("ModLoaders");
            }
        }
        /// <summary>
        /// List of all GameData
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
        public GameData CurrentGameData
        {
            get => _CurrentGameData;
            set
            {
                if (_CurrentGameData == value)
                    return;

                _CurrentGameData = value;
                OnPropertyChanged("CurrentGameData");
            }
        }

        /// <summary>
        /// Selected InternalModType
        /// </summary>
        public InternalModType SelectedInternalModType
        {
            get => _SelectedInternalModType;
            set
            {
                if (_SelectedInternalModType == value)
                    return;

                _SelectedInternalModType = value;

                if (value != null)
                {
                    SelectedGameDataCategory = CurrentGameData.Categories.Find(c => c.ID == value.Association);
                    SlotString = value.Slots.ToString();
                }
                    

                OnPropertyChanged("SelectedInternalModType");
            }
        }
        /// <summary>
        /// Selected Internal Mod Type File
        /// </summary>
        public InternalModTypeFile SelectedInternalModTypeFile
        {
            get => _SelectedInternalModTypeFile;
            set
            {
                if (_SelectedInternalModTypeFile == value)
                    return;

                _SelectedInternalModTypeFile = value;

                if(value != null)
                {
                    if (value.Files == null)
                    {
                        _SelectedInternalModTypeFile.Files = new List<BuilderFile>()
                    {
                        new BuilderFile()
                        {
                            BuilderID = 1,
                            File = "",
                            Path = ""
                        },
                        new BuilderFile()
                        {
                            BuilderID = 2,
                            File = "",
                            Path = ""
                        }
                    };
                    }
                }
                
                OnPropertyChanged("SelectedInternalModTypeFile");
            }
        }
        /// <summary>
        /// Selected Builder File
        /// </summary>
        public BuilderFile SelectedBuilderFile
        {
            get => _SelectedBuilderFile;
            set
            {
                if (_SelectedBuilderFile == value)
                    return;

                _SelectedBuilderFile = value;
                OnPropertyChanged("SelectedBuilderFile");
            }
        }
        public GameDataCategory SelectedGameDataCategory
        {
            get => _SelectedGameDataCategory;
            set
            {
                if (_SelectedGameDataCategory == value)
                    return;

                _SelectedGameDataCategory = value;

                if(value != null)
                {
                    if(value.ID != SelectedInternalModType.Association)
                    {
                        SelectedInternalModType.Association = value.ID;
                    }
                }

                OnPropertyChanged("SelectedGameDataCategory");
            }
        }
        public string SlotString
        {
            get => _SlotString;
            set
            {
                if (_SlotString == value)
                    return;

                _SlotString = value;

                OnPropertyChanged("SlotString");
            }
        }


        public ModListItem SelectedModListItem
        {
            get => _SelectedModListItem;
            set
            {
                if (_SelectedModListItem == value)
                    return;

                _SelectedModListItem = value;

                OnPropertyChanged("SelectedModListItem");
            }
        }



        public ICommand InternalModTypeSaveCommand
        {
            get
            {
                if (_InternalModTypeSaveCommand == null)
                {
                    _InternalModTypeSaveCommand = new RelayCommand(param => Save());
                }
                return _InternalModTypeSaveCommand;
            }
        }
        public ICommand TestInternalModTypeCommand
        {
            get
            {
                if (_TestInternalModTypeCommand == null)
                {
                    _TestInternalModTypeCommand = new RelayCommand(param => Test());
                }
                return _TestInternalModTypeCommand;
            }
        }
        #endregion

        /// <summary>
        /// Default Constructor
        /// </summary>
        public InternalModTypeViewModel(ObservableCollection<InternalModType> _InternalModTypes, ObservableCollection<ModLoader> _ModLoaders, ObservableCollection<GameData> _GameDatas, ObservableCollection<Game> _Games)
        {
            InternalModTypes = _InternalModTypes;
            ModLoaders = _ModLoaders;
            GameDatas = _GameDatas;
            CurrentGameData = GameDatas[0];
            Games = _Games;

            EventSystem.Subscribe<ModListItem>(ChangeSelectedModListItem);
        }

        #region Actions
        public void Save()
        {
            int Slots = -1;
            int ID = 0;

            if(int.TryParse(SlotString,out Slots))
            {
                SelectedInternalModType.Slots = Slots;
                foreach(InternalModTypeFile file in SelectedInternalModType.Files)
                {
                    file.ID = ID;
                    ID++;
                }
                XML.SaveInternalModType(SelectedInternalModType);
            }
        }
        public void Test()
        {
            ModFileManager mfm = new ModFileManager(SelectedModListItem.ModListItemViewModel.LibraryMod, Games[1]);
            new DefinitionsWindow(mfm, "", "", "", "", CurrentGameData.Categories.ToList(), InternalModTypes.ToList(), 0).Show();
        }
        public void ChangeSelectedModListItem(ModListItem _SelectedModListItem)
        {
            SelectedModListItem = _SelectedModListItem;
        }
        #endregion
    }


}

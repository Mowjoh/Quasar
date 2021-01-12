using Quasar.Controls.Common.Models;
using Quasar.Data.V2;
using Quasar.FileSystem;
using Quasar.Helpers.XML;
using Quasar.Internal;
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
        private Game _CurrentGame { get; set; }
        private GameElementFamily _SelectedGameElementFamily { get; set; }
        private QuasarModType _SelectedQuasarModType { get; set; }
        private QuasarModTypeFileDefinition _SelectedQuasarModTypeFileDefinition { get; set; }
        private QuasarModTypeBuilderDefinition _SelectedQuasarModTypeBuilderDefinition { get; set; }
        private string _SlotCount { get; set; }


        private LibraryItem _SelectedLibraryItem { get; set; }

        private ICommand _InternalModTypeSaveCommand { get; set; }
        private ICommand _TestInternalModTypeCommand { get; set; }
        #endregion

        #region Parameters

        public Game CurrentGame
        {
            get => _CurrentGame;
            set
            {
                if (_CurrentGame == value)
                    return;

                _CurrentGame = value;
                OnPropertyChanged("CurrentGame");
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

                if (value != null)
                {
                    SelectedGameElementFamily = CurrentGame.GameElementFamilies.SingleOrDefault(gef => gef.ID == value.GameElementFamilyID);
                    //SlotCount = value.Slots.ToString();
                }
                    

                OnPropertyChanged("SelectedQuasarModType");
            }
        }
        public QuasarModTypeFileDefinition SelectedQuasarModTypeFileDefinition
        {
            get => _SelectedQuasarModTypeFileDefinition;
            set
            {
                if (_SelectedQuasarModTypeFileDefinition == value)
                    return;

                _SelectedQuasarModTypeFileDefinition = value;

                if(value != null)
                {
                    if (value.QuasarModTypeBuilderDefinitions == null)
                    {
                        _SelectedQuasarModTypeFileDefinition.QuasarModTypeBuilderDefinitions = new ObservableCollection<QuasarModTypeBuilderDefinition>()
                    {
                        new QuasarModTypeBuilderDefinition()
                        {
                            ModLoaderID = 1,
                            OutputFileName = "",
                            OutputPath = ""
                        },
                        new QuasarModTypeBuilderDefinition()
                        {
                            ModLoaderID = 2,
                            OutputFileName = "",
                            OutputPath = ""
                        },
                        new QuasarModTypeBuilderDefinition()
                        {
                            ModLoaderID = 3,
                            OutputFileName = "",
                            OutputPath = ""
                        }
                        ,
                        new QuasarModTypeBuilderDefinition()
                        {
                            ModLoaderID = 4,
                            OutputFileName = "",
                            OutputPath = ""
                        }
                    };
                    }
                }
                
                OnPropertyChanged("SelectedQuasarModTypeFileDefinition");
            }
        }
        public QuasarModTypeBuilderDefinition SelectedQuasarModTypeBuilderDefinition
        {
            get => _SelectedQuasarModTypeBuilderDefinition;
            set
            {
                if (_SelectedQuasarModTypeBuilderDefinition == value)
                    return;

                _SelectedQuasarModTypeBuilderDefinition = value;
                OnPropertyChanged("SelectedQuasarModTypeBuilderDefinition");
            }
        }
        public string SlotCount
        {
            get => _SlotCount;
            set
            {
                if (_SlotCount == value)
                    return;

                _SlotCount = value;

                OnPropertyChanged("SlotCount");
            }
        }


        public LibraryItem SelectedLibraryItem
        {
            get => _SelectedLibraryItem;
            set
            {
                if (_SelectedLibraryItem == value)
                    return;

                _SelectedLibraryItem = value;

                OnPropertyChanged("SelectedLibraryItem");
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
        public InternalModTypeViewModel(MainUIViewModel MUVM)
        {
            EventSystem.Subscribe<LibraryItem>(ChangeSelectedModListItem);
        }

        #region Actions
        public void Save()
        {/*
            int Slots = -1;
            int ID = 0;

            if(int.TryParse(SlotCount,out Slots))
            {
                SelectedQuasarModType.Slots = Slots;
                foreach(InternalModTypeFile file in SelectedQuasarModType.Files)
                {
                    file.ID = ID;
                    ID++;
                }
                XMLHelper.SaveInternalModType(SelectedQuasarModType);
            }*/
        }
        public void Test()
        {
            //ModFileManager mfm = new ModFileManager(SelectedLibraryItem.ModListItemViewModel.LibraryMod, Games[1]);
            //new DefinitionsWindow(mfm, "", "", "", "", SelectedGameElementFamily.Categories.ToList(), InternalModTypes.ToList(), 0).Show();
        }
        public void ChangeSelectedModListItem(LibraryItem _SelectedLibraryItem)
        {
            SelectedLibraryItem = _SelectedLibraryItem;
        }
        #endregion
    }


}

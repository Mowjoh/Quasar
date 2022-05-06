using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DataModels.Common;
using Quasar.Common.Models;
using Quasar.Helpers;
using Quasar.Skins.Views;

namespace Quasar.Skins.ViewModels
{
    public class CharacterItemViewModel : ObservableObject
    {
        private string _Chara { get; set; }
        public string CharacterString
        {
            get => _Chara;
            set
            {
                _Chara = value;
                OnPropertyChanged("CharacterString");
                OnPropertyChanged("Slot0String");
                OnPropertyChanged("Slot1String");
                OnPropertyChanged("Slot2String");
                OnPropertyChanged("Slot3String");
                OnPropertyChanged("Slot4String");
                OnPropertyChanged("Slot5String");
                OnPropertyChanged("Slot6String");
                OnPropertyChanged("Slot7String");

            }
        }

        public string Slot0String => $@"../../../Resources/images/Characters/{CharacterString}_00.png";
        public string Slot1String => $@"../../../Resources/images/Characters/{CharacterString}_01.png";
        public string Slot2String => $@"../../../Resources/images/Characters/{CharacterString}_02.png";
        public string Slot3String => $@"../../../Resources/images/Characters/{CharacterString}_03.png";
        public string Slot4String => $@"../../../Resources/images/Characters/{CharacterString}_04.png";
        public string Slot5String => $@"../../../Resources/images/Characters/{CharacterString}_05.png";
        public string Slot6String => $@"../../../Resources/images/Characters/{CharacterString}_06.png";
        public string Slot7String => $@"../../../Resources/images/Characters/{CharacterString}_07.png";

        private ICommand _Selected { get; set; }

        public ICommand Selected
        {
            get
            {
                if (_Selected == null)
                {
                    _Selected = new RelayCommand(param => CharacterItemSelection());
                }
                return _Selected;
            }
        }

        public CharacterItemViewModel(string ch)
        {
            CharacterString = ch;
        }

        public void CharacterItemSelection()
        {
            EventSystem.Publish<CharacterItemViewModel>(this);
        }
    }
}

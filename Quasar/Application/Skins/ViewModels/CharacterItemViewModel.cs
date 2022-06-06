using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DataModels.Common;
using DataModels.Resource;
using Quasar.Common.Models;
using Quasar.Helpers;
using Quasar.Skins.Views;

namespace Quasar.Skins.ViewModels
{
    public class CharacterItemViewModel : ObservableObject
    {
        #region Data
        private GameElement _GameElement { get; set; }

        public GameElement GameElement
        {
            get => _GameElement;
            set
            {
                _GameElement = value;
                OnPropertyChanged("GameElement");
                OnPropertyChanged("ScreenshotName");
            }
        }

        public string ScreenshotName => GameElement == null ? "" : GameElement.GameFolderName.Split(";").Length > 0
            ? GameElement.GameFolderName.Split(";")[0]
            : GameElement.GameFolderName;

        public string Slot0String => $@"../../../Resources/images/Characters/{ScreenshotName}_00.png";
        public string Slot1String => $@"../../../Resources/images/Characters/{ScreenshotName}_01.png";
        public string Slot2String => $@"../../../Resources/images/Characters/{ScreenshotName}_02.png";
        public string Slot3String => $@"../../../Resources/images/Characters/{ScreenshotName}_03.png";
        public string Slot4String => $@"../../../Resources/images/Characters/{ScreenshotName}_04.png";
        public string Slot5String => $@"../../../Resources/images/Characters/{ScreenshotName}_05.png";
        public string Slot6String => $@"../../../Resources/images/Characters/{ScreenshotName}_06.png";
        public string Slot7String => $@"../../../Resources/images/Characters/{ScreenshotName}_07.png";


        #endregion

        #region Commands
        private ICommand _Selected { get; set; }

        public ICommand Selected
        {
            get
            {
                if (_Selected == null)
                {
                    _Selected = new RelayCommand(param => CharacterItemSelected());
                }
                return _Selected;
            }
        }

        #endregion


        public CharacterItemViewModel(GameElement _game_element)
        {
            GameElement = _game_element;
        }

        //Selection Event
        public void CharacterItemSelected()
        {
            EventSystem.Publish<CharacterItemViewModel>(this);
        }
    }
}

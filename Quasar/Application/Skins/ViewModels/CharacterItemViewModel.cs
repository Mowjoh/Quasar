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

        public string BaseImage => $@"../../../Resources/images/Characters/{ScreenshotName}_00.png";

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

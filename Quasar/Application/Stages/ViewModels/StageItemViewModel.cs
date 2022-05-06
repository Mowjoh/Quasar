using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DataModels.Common;
using Quasar.Common.Models;
using Quasar.Helpers;
using Quasar.Skins.ViewModels;

namespace Quasar.Stages.ViewModels
{
    public class StageItemViewModel : ObservableObject
    {
        private string _StageString { get; set; }
        public string StageString
        {
            get => _StageString;
            set
            {
                _StageString = value;
                OnPropertyChanged("StageString");
                OnPropertyChanged("MiniatureString");
                OnPropertyChanged("DefaultString");
                OnPropertyChanged("OmegaString");
                OnPropertyChanged("BattlefieldString");
                OnPropertyChanged("Empty");
                OnPropertyChanged("Full");
            }
        }

        public string MiniatureString => $@"../../../Resources/images/Stages/miniature_{StageString}.png";
        public string DefaultString => $@"../../../Resources/images/Stages/standard_{StageString}.png";
        public string OmegaString => $@"../../../Resources/images/Stages/omega_{StageString}.png";
        public string BattlefieldString => $@"../../../Resources/images/Stages/battlefield_{StageString}.png";

        public bool Empty => StageString == "blank";
        public bool Full => StageString != "blank";
        private ICommand _Selected { get; set; }

        public ICommand Selected
        {
            get
            {
                if (_Selected == null)
                {
                    _Selected = new RelayCommand(param => StageItemSelection());
                }
                return _Selected;
            }
        }

        public StageItemViewModel(string _stage_name)
        {
            StageString = _stage_name;
        }

        public void StageItemSelection()
        {
            EventSystem.Publish<StageItemViewModel>(this);
        }
    }
}

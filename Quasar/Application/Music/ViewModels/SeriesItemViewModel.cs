using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DataModels.Common;
using Quasar.Common.Models;
using Quasar.Helpers;
using Quasar.Stages.ViewModels;

namespace Quasar.Music.ViewModels
{
    public class SeriesItemViewModel : ObservableObject
    {
        private string _SeriesString { get; set; }
        public string SeriesString
        {
            get => _SeriesString;
            set
            {
                _SeriesString = value;
                OnPropertyChanged("SeriesString");
            }
        }

        public string ImageString => $@"../../../Resources/images/Series/{SeriesString}Symbol.png";

        private ICommand _Selected { get; set; }

        public ICommand Selected
        {
            get
            {
                if (_Selected == null)
                {
                    _Selected = new RelayCommand(param => SeriesItemSelection());
                }
                return _Selected;
            }
        }

        public SeriesItemViewModel(string _series_name)
        {
            SeriesString = _series_name;
        }

        public void SeriesItemSelection()
        {
            EventSystem.Publish<SeriesItemViewModel>(this);
        }
    }
}

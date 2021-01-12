using Quasar.Controls.Common.Models;
using Quasar.Data.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quasar.Controls.Associations.Models
{
    public class GameElementSquare : ObservableObject
    {
        private Uri _ImageSource { get; set; }
        public Uri ImageSource
        {
            get => _ImageSource;
            set
            {
                if (_ImageSource == value)
                    return;

                _ImageSource = value;
                OnPropertyChanged("ImageSource");
            }
        }
        private Uri _HoverImageSource { get; set; }
        public Uri HoverImageSource
        {
            get => _HoverImageSource;
            set
            {
                if (_HoverImageSource == value)
                    return;

                _HoverImageSource = value;
                OnPropertyChanged("HoverImageSource");
            }
        }
        private GameDataCategory _GameDataCategory { get; set; }
        public GameDataCategory GameDataCategory
        {
            get => _GameDataCategory;
            set
            {
                if (_GameDataCategory == value)
                    return;

                _GameDataCategory = value;
                OnPropertyChanged("GameDataCategory");
            }
        }

    }
}

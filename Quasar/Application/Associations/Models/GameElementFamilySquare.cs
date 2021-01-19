using Quasar.Controls.Common.Models;
using Quasar.Data.V2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quasar.Controls.Associations.Models
{
    public class GameElementFamilySquare : ObservableObject
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
        private GameElementFamily _GameElementFamily { get; set; }
        public GameElementFamily GameElementFamily
        {
            get => _GameElementFamily;
            set
            {
                if (_GameElementFamily == value)
                    return;

                _GameElementFamily = value;
                OnPropertyChanged("GameElementFamily");
            }
        }

    }
}

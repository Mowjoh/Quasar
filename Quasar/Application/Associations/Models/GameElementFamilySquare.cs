using DataModels.Common;
using DataModels.User;
using DataModels.Resource;
using Quasar.Common.Models;
using System;

namespace Quasar.Associations.Models
{
    public class GameElementFamilySquare : ObservableObject
    {
        #region View

        #region Private
        private Uri _ImageSource { get; set; }
        private Uri _HoverImageSource { get; set; }
        #endregion

        #region Public
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
        #endregion

        #endregion

        #region Data

        #region Private
        private GameElementFamily _GameElementFamily { get; set; }
        #endregion

        #region Public
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
        #endregion

        #endregion

    }
}

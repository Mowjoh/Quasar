using Quasar.Controls.Common.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quasar.Controls.InternalModTypes.Models
{
    public class InternalModTypeGroup : ObservableObject
    {
        #region Fields
        private string _Name { get; set; }
        private ObservableCollection<InternalModType> _InternalModTypes { get; set; }
        #endregion

        #region Properties
        public string Name
        {
            get => _Name;
            set
            {
                if (_Name == value)
                    return;

                _Name = value;
                OnPropertyChanged("Name");
            }
        }
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
        #endregion



    }
}

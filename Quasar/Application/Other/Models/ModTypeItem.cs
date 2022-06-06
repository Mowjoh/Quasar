using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModels.Common;
using DataModels.Resource;

namespace Quasar.Other.Models
{
    public class ModTypeItem : ObservableObject
    {

        private QuasarModType _QuasarModType { get; set; }

        public QuasarModType QuasarModType
        {
            get => _QuasarModType;
            set
            {
                _QuasarModType = value;
                OnPropertyChanged("QuasarModType");
                OnPropertyChanged("QuasarModTypeName");
            }
        }
        public string QuasarModTypeName => $@"{QuasarModType.GroupName} / {QuasarModType.Name}";
    }
}

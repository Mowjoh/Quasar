using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quasar.Controls.Settings
{
    class SettingsClass
    {
        /// <summary>
        /// The list of Settings it encapsulates
        /// </summary>
        public ObservableCollection<SettingsItem> SettingsItems { get; set; }
    }
}

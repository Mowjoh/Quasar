using Quasar.Controls.Common.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quasar.Controls.Build.Models
{
    class USBDrive : ObservableObject
    {
        #region Fields
        private DriveInfo _Info { get; set; }

        #endregion

        #region Properties
        public DriveInfo Info
        {
            get => _Info;
            set
            {
                if (_Info == value)
                    return;

                _Info = value;
                OnPropertyChanged("Info");
            }
        }

        #endregion

        public USBDrive(DriveInfo _DriveInfo)
        {
            Info = _DriveInfo;
        }

        override public string ToString()
        {
            //return String.Format("{0}, {1} - {2}/{3}", Info.Name, Info.VolumeLabel, Info.AvailableFreeSpace, Info.TotalSize);
            return String.Format(@"{0} - '{1}'", Info.Name, Info.VolumeLabel);
        }
    }
}

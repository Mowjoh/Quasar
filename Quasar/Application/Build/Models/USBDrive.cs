using MediaDevices;
using DataModels.Common;
using System;
using System.IO;

namespace Quasar.Build.Models
{
    public class USBDrive : ObservableObject
    {
        #region Fields
        private DriveInfo _Info { get; set; }
        private MediaDevice _MediaD { get; set; }
        private string _DisplayName { get; set; }
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
        public MediaDevice MediaD
        {
            get => _MediaD;
            set
            {
                if (_MediaD == value)
                    return;

                _MediaD = value;
                OnPropertyChanged("MediaD");
            }
        }
        public string DisplayName
        {
            get => _DisplayName;
            set
            {
                if (_DisplayName == value)
                    return;

                _DisplayName = value;
                OnPropertyChanged("DisplayName");
            }
        }

        #endregion

        public USBDrive(DriveInfo _DriveInfo)
        {
            Info = _DriveInfo;
            DisplayName = String.Format("{0} ({1})", Info.VolumeLabel, Info.Name);
        }

        public USBDrive(MediaDevice md)
        {
            MediaD = md;
            DisplayName = String.Format("{0} (MTP)", md.FriendlyName == "" ? "Nintendo Switch" : md.FriendlyName); 
        }
    }
}

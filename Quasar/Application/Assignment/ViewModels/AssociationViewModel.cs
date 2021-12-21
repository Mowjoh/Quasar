using log4net;
using Quasar.Associations.Models;
using Quasar.Associations.ViewModels;
using Quasar.Associations.Views;
using Quasar.Common.Models;
using DataModels.User;
using DataModels.Common;
using DataModels.Resource;
using Quasar.Helpers;
using Quasar.MainUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using Workshop.FileManagement;

namespace Quasar.Associations.ViewModels
{
    public class AssociationViewModel : ObservableObject
    {
        public static string AppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Quasar";

        #region View

        #region Private

        #endregion

        #region Public
        
        #endregion

        #endregion

        #region Data

        #region Private
        private MainUIViewModel _MUVM { get; set; }

        #endregion

        #region Public

        public MainUIViewModel MUVM
        {
            get => _MUVM;
            set
            {
                _MUVM = value;
                OnPropertyChanged("MUVM");
            }
        }
        #endregion

        #endregion

        #region Commands

        #region Private

        #endregion

        #region Public

        #endregion

        #endregion

        public ILog QuasarLogger { get; set; }

        /// <summary>
        /// Association View Model Constructor
        /// </summary>
        /// <param name="_MUVM">MainUI View Model to link</param>
        public AssociationViewModel(MainUIViewModel _MUVM, ILog _QuasarLogger)
        {
            QuasarLogger = _QuasarLogger;
            MUVM = _MUVM;
        }

        #region Actions    

        #endregion

        #region User Actions

        #endregion

        #region Events

        #endregion

    }
}

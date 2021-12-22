using log4net;
using Quasar.Common.Models;
using DataModels.Common;
using DataModels.User;
using DataModels.Resource;
using Quasar.Helpers;
using System.Windows.Input;

namespace Quasar.MainUI.ViewModels
{

    public class ModalPopupViewModel : ObservableObject
    {
        #region Commands

        #region Private
        private ICommand _ModalOKCommand { get; set; }
        private ICommand _ModalCancelCommand { get; set; }
        #endregion

        #region Public
        public ICommand ModalOKCommand
        {
            get
            {
                if (_ModalOKCommand == null)
                {
                    _ModalOKCommand = new RelayCommand(param => SendOK());
                }
                return _ModalOKCommand;
            }
        }
        public ICommand ModalCancelCommand
        {
            get
            {
                if (_ModalCancelCommand == null)
                {
                    _ModalCancelCommand = new RelayCommand(param => SendKO());
                }
                return _ModalCancelCommand;
            }
        }
        #endregion

        #endregion

        #region Data

        #region Private
        private ModalEvent _Meuh { get; set; }
        #endregion

        #region Public
        /// <summary>
        /// ModalEvent tied to the Modal
        /// </summary>
        public ModalEvent Meuh
        {
            get => _Meuh;
            set
            {
                _Meuh = value;
                OnPropertyChanged("Meuh");
                OnPropertyChanged("ModalVisible");
                OnPropertyChanged("ModalLoading");
                OnPropertyChanged("ModalSuccess");
                OnPropertyChanged("ModalSuccessShown");
                OnPropertyChanged("TitleVisible");
                OnPropertyChanged("TitleInvisible");
                OnPropertyChanged("OkButtonVisible");
                OnPropertyChanged("OkCancelButtonVisible");
                OnPropertyChanged("OKButtonEnabled");
                OnPropertyChanged("ModalSuccessVisible");
                OnPropertyChanged("ModalFailureVisible");
            }
        }
        #endregion

        #endregion

        #region View

        #region Private
        private bool _ModalVisible { get; set; }
        #endregion

        #region Public
        //Base properties
        public bool ModalVisible
        {
            get => _ModalVisible;
            set
            {
                _ModalVisible = value;
                OnPropertyChanged("ModalVisible");
            }
        }
        public bool TitleVisible => (Meuh?.Title ?? "") != "";
        public bool TitleInvisible => !TitleVisible;
        public bool OkButtonVisible => Meuh?.Type == ModalType.Warning || Meuh?.Type == ModalType.Loader;
        public bool OkCancelButtonVisible => Meuh?.Type == ModalType.OkCancel;

        //Spinner properties
        public bool ModalSuccess { get; set; }
        public bool ModalSuccessShown { get; set; }
        public bool ModalSuccessVisible => ModalSuccessShown && ModalSuccess;
        public bool ModalFailureVisible => ModalSuccessShown && !ModalSuccess;

        //Loader properties
        public bool ModalLoading { get; set; }
        public bool OKButtonEnabled => !ModalLoading;

        #endregion

        #endregion

        /// <summary>
        /// Modal Popup Constructor
        /// </summary>
        public ModalPopupViewModel(ILog log)
        {
            EventSystem.Subscribe<ModalEvent>(ProcessModalEvent);
        }

        #region Actions

        /// <summary>
        /// Processes an incoming Modal Event
        /// </summary>
        /// <param name="meuh">Modal Event to act upon</param>
        public void ProcessModalEvent(ModalEvent meuh)
        {
            switch (meuh.Action ?? "")
            {
                case "Show":
                    ShowModal(meuh);
                    break;
                case "LoadOK":
                    EnableModal(true, meuh);
                    break;
                case "LoadKO":
                    EnableModal(false, meuh);
                    break;
                case "Update":
                    UpdateModal(meuh);
                    break;

            }
        }

        /// <summary>
        /// Enables the Modal and displays it's data
        /// </summary>
        /// <param name="meuh">ModalEvent to be displayed</param>
        public void ShowModal(ModalEvent meuh)
        {
            ModalVisible = true;
            Meuh = meuh;
            if (meuh.Type == ModalType.Loader)
            {
                ModalLoading = true;
                OnPropertyChanged("ModalLoading");
                OnPropertyChanged("OKButtonEnabled");
            }
        }

        /// <summary>
        /// Enables the OK button
        /// </summary>
        /// <param name="Success">State to show</param>
        /// <param name="meuh">Modal Event to update data with</param>
        public void EnableModal(bool Success, ModalEvent meuh)
        {

            ModalLoading = false;
            ModalSuccessShown = true;
            ModalSuccess = Success;
            OnPropertyChanged("ModalLoading");
            OnPropertyChanged("OKButtonEnabled");
            UpdateModal(meuh);

        }

        /// <summary>
        /// Resets the Modal to it's default state
        /// </summary>
        public void ResetModal()
        {
            ModalVisible = false;
            ModalLoading = false;
            ModalSuccessShown = false;
            Meuh = null;
        }

        /// <summary>
        /// Updates the Modal UI
        /// </summary>
        /// <param name="me">Modal Event to be based on</param>
        public void UpdateModal(ModalEvent me)
        {
            if ((me.Title ?? "") != "")
            {
                Meuh.Title = me.Title;
            }
            if ((me.Content ?? "") != "")
            {
                Meuh.Content = me.Content;
            }

            OnPropertyChanged("Meuh");
            OnPropertyChanged("ModalLoading");
            OnPropertyChanged("ModalSuccessShown");
            OnPropertyChanged("ModalSuccess");
            OnPropertyChanged("ModalSuccessVisible");
            OnPropertyChanged("ModalFailureVisible");
            OnPropertyChanged("OKButtonEnabled");
        }

        #endregion

        #region User Actions

        /// <summary>
        /// Sends an OK response from the User
        /// </summary>
        public void SendOK()
        {
            ModalEvent me = new ModalEvent()
            {
                EventName = Meuh.EventName,
                Action = "OK"
            };
            ResetModal();
            EventSystem.Publish<ModalEvent>(me);

        }

        /// <summary>
        /// Sends a KO response from the user
        /// </summary>
        public void SendKO()
        {
            ModalEvent me = new ModalEvent()
            {
                EventName = Meuh.EventName,
                Action = "KO"
            };
            ResetModal();
            EventSystem.Publish<ModalEvent>(me);
        }

        #endregion


    }

}

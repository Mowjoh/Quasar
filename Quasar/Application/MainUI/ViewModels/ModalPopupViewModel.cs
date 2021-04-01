using Quasar.Controls.Common.Models;
using Quasar.Internal;
using Quasar.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Quasar.ViewModels
{

    public class ModalPopupViewModel : ObservableObject
    {
        #region Commands
        private ICommand _ModalOKCommand { get; set; }
        private ICommand _ModalCancelCommand { get; set; }
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

        #region Modal Properties
        private ModalEvent _Meuh { get; set; }
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

        private bool _ModalVisible { get; set; }
        public bool ModalVisible
        {
            get => _ModalVisible;
            set
            {
                _ModalVisible = value;
                OnPropertyChanged("ModalVisible");
            }
        }
        public bool ModalLoading { get; set; }
        
        public bool ModalSuccess { get; set; }
        public bool ModalSuccessShown { get; set; }

        public bool TitleVisible => (Meuh?.Title ?? "") != "";
        public bool TitleInvisible => !TitleVisible;
        public bool OkButtonVisible => Meuh?.Type == ModalType.Warning || Meuh?.Type == ModalType.Loader;
        public bool OkCancelButtonVisible => Meuh?.Type == ModalType.OkCancel;

        public bool OKButtonEnabled => !ModalLoading;
        public bool ModalSuccessVisible => ModalSuccessShown && ModalSuccess;
        public bool ModalFailureVisible => ModalSuccessShown && !ModalSuccess;

        

        #endregion

        #region Modal
        
        public void CreateModal(ModalEvent meuh)
        {
            switch(meuh.Action ?? "")
            {
                case "Show":
                    ModalVisible = true;
                    Meuh = meuh;
                    if(meuh.Type == ModalType.Loader)
                    {
                        ModalLoading = true;
                        OnPropertyChanged("ModalLoading");
                    }
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
        public void EnableModal(bool Success, ModalEvent meuh)
        {
            
            ModalLoading = false;
            ModalSuccessShown = true;
            ModalSuccess = Success;
            UpdateModal(meuh);
            
        }
        public void ResetModal()
        {
            ModalVisible = false;
            ModalLoading = false;
            ModalSuccessShown = false;
            Meuh = null;
        }

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

       
        public ModalPopupViewModel()
        {
            EventSystem.Subscribe<ModalEvent>(CreateModal);
        }
    }

}

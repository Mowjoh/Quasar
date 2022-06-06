using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Services.Description;
using System.Windows.Input;
using DataModels.Common;
using DataModels.Resource;
using DataModels.User;
using Quasar.Common.Models;
using Quasar.Common.ViewModels;
using Quasar.Common.Views;
using Quasar.Helpers;
using Quasar.MainUI.ViewModels;
using Quasar.Other.Models;

namespace Quasar.Other.ViewModels
{
    public class OtherManagementViewModel : ObservableObject
    {
        #region Data

        #region Private

        private ObservableCollection<ModTypeItem> _UseableModTypes { get; set; }
        private ObservableCollection<ContentItemView> _AvailableContentItems { get; set; }
        private ObservableCollection<ContentItemView> _SlottedContentItems { get; set; }
        private ModTypeItem _SelectedModTypeItem { get; set; }
        #endregion

        #region Public
        public MainUIViewModel MainUiViewModel { get; set; }

        public ObservableCollection<ModTypeItem> UseableModTypes
        {
            get => _UseableModTypes;
            set
            {
                _UseableModTypes = value;
                OnPropertyChanged("UseableModTypes");
            }
        }
        public ObservableCollection<ContentItemView> AvailableContentItems
        {
            get => _AvailableContentItems;
            set
            {
                _AvailableContentItems = value;
                OnPropertyChanged("AvailableContentItems");
            }
        }
        public ObservableCollection<ContentItemView> SlottedContentItems
        {
            get => _SlottedContentItems;
            set
            {
                _SlottedContentItems = value;
                OnPropertyChanged("SlottedContentItems");
            }
        }
        public ModTypeItem SelectedModTypeItem
        {
            get => _SelectedModTypeItem;
            set
            {
                _SelectedModTypeItem = value;
                if (_SelectedModTypeItem != null)
                {
                    AvailableContentItems = GetAvailableContentItems();
                    SlottedContentItems = GetAvailableContentItems();
                    OnPropertyChanged("AvailableContentItems");
                    OnPropertyChanged("SlottedContentItems");
                }
                OnPropertyChanged("SelectedModTypeItem");
            }
        }

        #endregion

        #endregion

        #region View

        #region Private

        #endregion

        #region Public

        #endregion

        #endregion

        #region Commands

        #region Private
        private ICommand _GoBack { get; set; }
        #endregion

        #region Public
        public ICommand GoBack
        {
            get
            {
                if (_GoBack == null)
                {
                    //_GoBack = new RelayCommand(param => foo());
                }
                return _GoBack;
            }
        }
        #endregion

        #endregion

        public OtherManagementViewModel(MainUIViewModel _main_ui_view_model)
        {
            MainUiViewModel = _main_ui_view_model;
            UseableModTypes = GetUseableModTypes();
        }

        public ObservableCollection<ModTypeItem> GetUseableModTypes()
        {
            ObservableCollection<ModTypeItem> types = new();

            if (MainUiViewModel.ContentItems != null)
            {
                List<QuasarModType> typelist = MainUiViewModel.QuasarModTypes.Where(t => MainUiViewModel.ContentItems.Any(c => c.QuasarModTypeID == t.ID) && t.NoGameElement==true).ToList();
                foreach (QuasarModType quasarModType in typelist)
                {
                    types.Add(new() { QuasarModType = quasarModType });
                }
            }
            
            
            return types;
        }

        public ObservableCollection<ContentItemView> GetAvailableContentItems()
        {
            ObservableCollection<ContentItemView> Output = new();
            //foreach (ContentItem contentItem in MainUiViewModel.ContentItems.Where(ci => ci.QuasarModTypeID == SelectedModTypeItem.QuasarModType.ID))
            //{
            //    string typeName = $@"{SelectedModTypeItem.QuasarModType.GroupName} - {SelectedModTypeItem.QuasarModType.Name}";
            //    Output.Add(new(contentItem, typeName, contentItem.Name));
            //}
            return Output;
        }
    }
}

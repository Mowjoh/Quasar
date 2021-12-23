using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModels.User;
using DataModels.Common;
using DataModels.Resource;

namespace Quasar.Associations.ViewModels
{
    public class ContentListItemViewModel : ObservableObject
    {
        #region View

        #region Private
        private string _Name { get; set; }
        private string _ElementName { get; set; }
        private ContentTypes _Type { get; set; }
        #endregion

        #region Public

        public string TypeName
        {
            get => _Name;
            set
            {
                _Name = value;
                OnPropertyChanged("TypeName");
            }
        }

        public string ElementName
        {
            get => _ElementName;
            set
            {
                _ElementName = value;
                OnPropertyChanged("ElementName");
            }
        }

        public ContentTypes Type
        {
            get => _Type;
            set
            {
                _Type = value;
                OnPropertyChanged("Type");
                OnPropertyChanged("SlotSelection");
                OnPropertyChanged("ElementSelection");
            }
        }

        public bool SlotSelection => Type == ContentTypes.Slotted;
        public bool ElementSelection => Type == ContentTypes.ElementSelected;

        #endregion

        #endregion

        #region Data

        #region Private
        private ContentItem _ContentItem { get; set; }
        private List<Option> _Options { get; set; }
        private Option _SelectedOption { get; set; }
        private int _SelectedSlot { get; set; }
        #endregion

        #region Public

        public ContentItem ContentItem
        {
            get => _ContentItem;

            set
            {
                _ContentItem = value;
                OnPropertyChanged("ContentItem");
            }
        }
        public List<Option> Options
        {
            get => _Options;

            set
            {
                _Options = value;
                OnPropertyChanged("Options");
            }
        }

        public Option SelectedOption
        {
            get => _SelectedOption;

            set
            {
                _SelectedOption = value;
                OnPropertyChanged("SelectedOption");

                if (Type == ContentTypes.Slotted)
                {
                    SlotSelected();
                }
                else
                {
                    ElementSelected();
                }
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

        public ContentListItemViewModel(ContentItem ci, string _TypeName, string _ElementName, ContentTypes _Type, List<Option> _Options)
        {
            ContentItem = ci;
            TypeName = _TypeName;
            ElementName = _ElementName;
            Type = _Type;
            Options = _Options;

            if (Type == ContentTypes.Slotted)
            {
                SelectedOption = Options.Single(o => o.Key == ci.SlotNumber.ToString());
            }
            else
            {
                SelectedOption = Options.Single(o => o.Key == ci.GameElementID.ToString());
            }
        }

        #region Actions

        public void SlotSelected()
        {

        }

        public void ElementSelected()
        {

        }

        #endregion
    }

    public class Option
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public enum ContentTypes { Slotted, ElementSelected }
}

using System.Collections.Generic;
using System.Linq;
using DataModels.User;
using DataModels.Common;

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
        private AssignmentContent _AssignmentContent { get; set; }
        private List<Option> _Options { get; set; }
        private Option _SelectedOption { get; set; }
        private int _SelectedSlot { get; set; }
        
        #endregion

        #region Public

        public AssignmentContent AssignmentContent
        {
            get => _AssignmentContent;

            set
            {
                _AssignmentContent = value;
                OnPropertyChanged("AssignmentContent");
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
        public string RequestedAction { get; set; }
        
        #endregion

        #endregion

        #region Commands

        #region Private

        #endregion

        #region Public

        #endregion

        #endregion

        public ContentListItemViewModel(AssignmentContent _assignment_content, string _type_name, string _element_name, ContentTypes _type, List<Option> _options)
        {
            AssignmentContent = _assignment_content;
            TypeName = _type_name;
            ElementName = _element_name;
            Type = _type;
            Options = _options;

            SelectedOption = Type == ContentTypes.Slotted ? 
                Options.Single(_o => _o.Key == _assignment_content.SlotNumber.ToString()) : 
                Options.Single(_o => _o.Key == _assignment_content.AssignmentContentItems[0].GameElementID.ToString());

            
        }

        #region Actions

        public void SlotSelected()
        {
            RequestedAction = "SlotChange";
        }

        public void ElementSelected()
        {
            RequestedAction = "ElementChange";
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

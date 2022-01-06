using System.Collections.Generic;
using System.Linq;
using DataModels.User;
using DataModels.Common;
using Quasar.Helpers;
using System;

namespace Quasar.Associations.ViewModels
{
    public class ContentListItemViewModel : ObservableObject
    {
        #region View

        #region Private
        private string _Name { get; set; }
        private string _ElementName { get; set; }
        private string _Origin { get; set; }
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

        public string Origin
        {
            get => _Origin;
            set
            {
                _Origin = value;
                OnPropertyChanged("Origin");
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

                if (!Instanciating)
                {
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
        }
        public string RequestedAction { get; set; }
        public bool Instanciating { get; set; }
        
        #endregion

        #endregion

        #region Commands

        #region Private

        #endregion

        #region Public

        #endregion

        #endregion

        public ContentListItemViewModel(AssignmentContent _assignment_content, string _type_name, string _element_name,string origin, ContentTypes _type, List<Option> _options)
        {
            AssignmentContent = _assignment_content;
            TypeName = _type_name;
            ElementName = _element_name;
            Type = _type;
            Options = _options;
            Origin = _type == ContentTypes.Slotted ? String.Format("Origin : Slot {0}",origin) : String.Format("Origin : {0}", origin);

            string Key = Type == ContentTypes.Slotted ? _assignment_content.SlotNumber.ToString() : _assignment_content.AssignmentContentItems[0].GameElementID.ToString();

            Instanciating = true;

            SelectedOption = Key == "-1" ? 
                Options.Single(_o => _o.Key == "none") : 
                Options.Single(_o => _o.Key == Key);

            Instanciating = false;

        }

        #region Actions

        public void SlotSelected()
        {
            RequestedAction = "SlotChange";
            EventSystem.Publish<ContentListItemViewModel>(this);
        }

        public void ElementSelected()
        {
            RequestedAction = "ElementChange";
            EventSystem.Publish<ContentListItemViewModel>(this);
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

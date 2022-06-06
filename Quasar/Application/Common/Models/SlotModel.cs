using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModels.Common;
using DataModels.Resource;
using DataModels.User;
using Quasar.Common.Views;
using Workshop.Associations;

namespace Quasar.Common.Models
{
    public class SlotModel : ObservableObject
    {
        private ObservableCollection<ContentItemView> _Contents { get; set; }

        public ObservableCollection<ContentItemView> Contents
        {
            get => _Contents;
            set
            {
                _Contents = value;
                OnPropertyChanged("Contents");
            }
        }
        public List<QuasarModType> ModTypes { get; set; }
        public List<int> Slots { get; set; }
        public GameElement GameElement { get; set; }

        public void GetMatchingContents(ObservableCollection<ContentItem> _contents, bool _grouped)
        {
            List<ContentItem> MatchingContents = GameElement == null
                ? _contents.Where(c => ModTypes.Any(t => t.ID == c.QuasarModTypeID) && Slots.Contains(c.SlotNumber))
                    .ToList()
                : _contents.Where(c => ModTypes.Any(t => t.ID == c.QuasarModTypeID) && Slots.Contains(c.SlotNumber) && c.GameElementID == GameElement.ID)
                    .ToList();

            Contents = new();

            List<AssignmentContent> assignments = _grouped ?
                Grouper.GetGroupedAssignmentContents(new(MatchingContents)) :
                Grouper.GetSeparatedAssignmentContents(new(MatchingContents));

            foreach (AssignmentContent assignment in assignments)
            {
                QuasarModType t = ModTypes.Single(q => q.ID == assignment.DefaultContentItem.QuasarModTypeID);
                assignment.AssignmentType = $@"{t.GroupName} - {t.Name}";

                Contents.Add(new(assignment));
            }
        }
    }
}

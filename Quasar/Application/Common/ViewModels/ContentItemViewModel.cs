using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModels.Common;
using DataModels.User;

namespace Quasar.Common.ViewModels
{
    public class ContentItemViewModel : ObservableObject
    {
        public AssignmentContent Assignment { get; set; }

        public ContentItemViewModel()
        {

        }

        public ContentItemViewModel(AssignmentContent _assignment)
        {
            Assignment = _assignment;
        }
    }
}

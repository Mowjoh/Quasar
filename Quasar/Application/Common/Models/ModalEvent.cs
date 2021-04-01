using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quasar.Models
{
    public class ModalEvent
    {
        public string EventName { get; set; }
        public string Action { get; set; }
        public ModalType Type { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string OkButtonText { get; set; }
        public string CancelButtonText { get; set; }

    }

    public enum ModalType { Warning, OkCancel, Loader}
}

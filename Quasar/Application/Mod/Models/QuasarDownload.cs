using Quasar.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quasar.Controls.Mod.Models
{
    public class QuasarDownload : ObservableObject
    {
        public string QuasarURL { get; set; }
        public string DownloadURL => QuasarURL.Substring(7).Split(',')[0];
        public string APICategoryName => QuasarURL.Substring(7).Split(',')[1];
        public string GamebananaItemID => QuasarURL.Substring(7).Split(',')[2];
        public string ModArchiveFormat => QuasarURL.Substring(7).Split(',')[3];
    }
}

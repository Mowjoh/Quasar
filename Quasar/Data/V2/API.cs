using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quasar.Data.V2
{
    public class APIMod
    {
        public int ID { get; set; }
        public string ModType { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("Credits().aAuthors()")]
        public string[][] Authors { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("downloads")]
        public int Downloads { get; set; }

        [JsonProperty("catid")]
        public int CategoryID { get; set; }

        [JsonProperty("Game().name")]
        public string GameName { get; set; }

        [JsonProperty("Updates().nGetUpdatesCount()")]
        public int UpdateCount { get; set; }

    }

    public partial class APIDownload
    {
        [JsonProperty("Url().sGetDownloadUrl()")]
        public Uri DownloadURL { get; set; }

        [JsonProperty("Files().aFiles()")]
        public Files Files { get; set; }
    }

    public partial class Files
    {
        public ModArchive ModArchive { get; set; }
    }

    public partial class ModArchive
    {
        [JsonProperty("_sFile")]
        public string Filename { get; set; }

        [JsonProperty("_nFilesize")]
        public long Filesize { get; set; }

        [JsonProperty("_sDownloadUrl")]
        public Uri DownloadURL { get; set; }
    }
}

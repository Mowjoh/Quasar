using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels.Common
{
    public class APIMod
    {
        public int ID { get; set; }
        public string GamebananaRootCategoryName { get; set; }

        [JsonProperty("_sName")]
        public string Name { get; set; }

        [JsonProperty("_aCredits")]
        public APIAuthors Authors { get; set; }

        [JsonProperty("_aGame")]
        public APIGame Game { get; set; }

        [JsonProperty("Updates().nGetUpdatesCount()")]
        public int UpdateCount { get; set; }
        [JsonProperty("_aCategory")]
        public APISubCategory SubCategory { get; set; }

        [JsonProperty("_aSuperCategory")]
        public APISuperCategory SuperCategory { get; set; }

        [JsonProperty("_aModManagerIntegrations")]
        public APIModManager ModManager { get; set; }

        [JsonProperty("_aSubmitter")]
        public APISubmitter Submitter { get; set; }

    }

    public partial class APISubCategory
    {
        [JsonProperty("_sName")]
        public string Name { get; set; }
        [JsonProperty("_idRow")]
        public int ID{ get; set; }
    }

    public partial class APISuperCategory
    {
        [JsonProperty("_sName")]
        public string Name { get; set; }
        [JsonProperty("_idRow")]
        public int ID { get; set; }
    }

    public partial class APIAuthors
    {
        [JsonProperty("Key Authors")]
        public string[][] KeyAuthors { get; set; }
        [JsonProperty("Authors")]
        public string[][] Authors { get; set; }
    }

    public partial class APIGame
    {
        [JsonProperty("_sName")]
        public string Name { get; set; }
        [JsonProperty("_idRow")]
        public int ID { get; set; }
    }

    public partial class APIModManager
    {
        [JsonProperty()]
        APIManager[] Integrations { get; set; }
    }

    public partial class APIManager
    {
        [JsonProperty("_sInstallerName")]
        public string InstallerName { get; set; }
        [JsonProperty("_sInstallerUrl")]
        public string InstallerURL { get; set; }
        [JsonProperty("_sIconClasses")]
        public string IconClass { get; set; }
        [JsonProperty("_sDownloadUrl")]
        public string DownloadURL { get; set; }
    }

    public partial class APISubmitter
    {
        [JsonProperty("_sName")]
        public string Name { get; set; }
        [JsonProperty("_idRow")]
        public int ID { get; set; }
    }

}

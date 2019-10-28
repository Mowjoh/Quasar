using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quasar
{
    #region Json Classes Definitions
    public class Member
    {
        public int id { get; set; }

        public string name { get; set; }
    }


    public class APIMod
    {
        public int id { get; set; }
        public string type { get; set; }
        
        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("Credits().aAuthors()")]
        public string[][] authors { get; set; }

        [JsonProperty("description")]
        public string description { get; set; }

        [JsonProperty("downloads")]
        public int downloads { get; set; }

        [JsonProperty("catid")]
        public int catid { get; set; }

        [JsonProperty("Updates().nGetUpdatesCount()")]
        public int Updates { get; set; }

    }

    public class APISkin : APIMod
    {
        public string files { get; set; }
    }
    #endregion

    #region API Interactions
    public class QueryStringItem
    {
        public string name { get; set; }
        public string value { get; set; }


        public QueryStringItem(string name, string value)
        {
            this.name = name;
            this.value = value;
        }

        public string output()
        {
            string outie = name + "=" + value;
            return outie;
        }
    }

    public class APIRequest
    {
        static string HTTPUrl = "https://api.gamebanana.com/";

        static QueryStringItem jsonFormat = new QueryStringItem("format", "json_min");
        static QueryStringItem jsonObject = new QueryStringItem("return_keys", "1");

        static List<QueryStringItem> queryParameters;

        #region Mod Actions
        public static async Task<APIMod> getMod(string itemtype, string itemID)
        {
            APIMod downloadedMod = null;

            queryParameters = getDefaultParameters();
            queryParameters.Add(new QueryStringItem("itemid", itemID));
            queryParameters.Add(new QueryStringItem("itemtype", itemtype));
            queryParameters.Add(new QueryStringItem("fields", "name,Credits().aAuthors(),description,downloads,catid,Updates().nGetUpdatesCount()"));

            string queryURL = formatApiRequest("Core/Item/Data", queryParameters);

            using (HttpClient webClient = new HttpClient())
            {
                HttpResponseMessage response = await webClient.GetAsync(queryURL);

                if (response.IsSuccessStatusCode)
                {
                    string responseText = await response.Content.ReadAsStringAsync();

                    downloadedMod = JsonConvert.DeserializeObject<APIMod>(responseText);
                }
            }

            downloadedMod.id = Int32.Parse(itemID);
            downloadedMod.type = itemtype;


            return downloadedMod;
        }

        #endregion

        #region API Formatting

        public static string formatApiRequest(string path, List<QueryStringItem> parameters)
        {
            string formattedURL = HTTPUrl + path + "?";

            Boolean first = true;
            foreach (QueryStringItem QSI in parameters)
            {
                formattedURL += first ? QSI.output() : "&" + QSI.output();
                if (first) { first = false; }
            }

            return formattedURL;
        }

        public static List<QueryStringItem> getDefaultParameters()
        {
            List<QueryStringItem> newParameters = new List<QueryStringItem>();

            //Adding default QueryString parameters
            newParameters.Add(jsonFormat);
            newParameters.Add(jsonObject);

            return newParameters;
        }

        #endregion



    }
    #endregion



}

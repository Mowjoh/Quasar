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

    public class Member
    {
        public int id { get; set; }

        public string name { get; set; }
    }

    public class ModItem
    {
        public int id { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public string authors { get; set; }
        public string description { get; set; }
        public int downloads { get; set; }

    }



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
        static HttpClient client;
        static string HTTPUrl = "https://api.gamebanana.com/";
        
        static QueryStringItem jsonFormat = new QueryStringItem("format", "json_min");
        static QueryStringItem jsonObject = new QueryStringItem("return_object", "1");

        static List<QueryStringItem> queryParameters;

        public APIRequest()
        { 
        }

        #region Runs

        public static async Task<List<ModItem>> RunGetModAsync(string itemType)
        {
            resetClient();

            List<string> latestModIDS = new List<string>();
            List<ModItem> newestMods = new List<ModItem>();

            latestModIDS = await getLatestModID(itemType);
            newestMods = await getModsFromList(itemType, latestModIDS);

            return newestMods;
        }

        #endregion

        #region User Actions
        public static async Task<Member> getUser(int userID)
        {
            Member member = null;

            queryParameters = getDefaultParameters();
            queryParameters.Add(new QueryStringItem("itemid", userID.ToString()));
            queryParameters.Add(new QueryStringItem("itemtype", "Member"));
            queryParameters.Add(new QueryStringItem("fields", "lastpost_date,Posts().Postcount().nGetPostCount(),OnlineStatus().bIsOnline()"));
            string currentURL = formatApiRequest("Core/Item/Data", queryParameters);

            HttpResponseMessage response = await client.GetAsync(currentURL);

            if (response.IsSuccessStatusCode)
            {
                string responseText = await response.Content.ReadAsStringAsync();
                var array = JsonConvert.DeserializeObject<JArray>(responseText);

                member = new Member();
                member.name = JsonConvert.DeserializeObject<List<string>>(responseText)[0];
            }
            return member;
        }
        #endregion

        #region Mod Actions
        public static async Task<List<string>> getLatestModID(string itemtype)
        {
            List<string> idList = new List<string>();

            queryParameters = getDefaultParameters();
            queryParameters.Add(new QueryStringItem("itemtype", itemtype));
            queryParameters.Add(new QueryStringItem("sort", "id"));
            queryParameters.Add(new QueryStringItem("direction", "desc"));
            queryParameters.Add(new QueryStringItem("page", "1"));

            string queryURL = formatApiRequest("Core/List/Section", queryParameters);

            HttpResponseMessage response = await client.GetAsync(queryURL);

            if (response.IsSuccessStatusCode)
            {
                string responseText = await response.Content.ReadAsStringAsync();

                idList = JsonConvert.DeserializeObject<List<string>>(responseText);

            }

            return idList;

        }

        public static async Task<List<ModItem>> getModsFromList(string itemtype, List<string> idList)
        {
            List<ModItem> newestMods = new List<ModItem>();

            int count = 0;
            queryParameters = getDefaultParameters();
            foreach (string id in idList)
            {
                queryParameters.Add(new QueryStringItem("itemid[" + count + "]", id));
                queryParameters.Add(new QueryStringItem("itemtype[" + count + "]", "Skin"));
                queryParameters.Add(new QueryStringItem("fields[" + count + "]", "name,authors,description,downloads"));
                count++;
            }

            string queryURL = formatApiRequest("Core/Item/Data", queryParameters);

            HttpResponseMessage response = await client.GetAsync(queryURL);

            if (response.IsSuccessStatusCode)
            {
                string responseText = await response.Content.ReadAsStringAsync();

                newestMods = JsonConvert.DeserializeObject<List<ModItem>>(responseText);

            }
            return newestMods;
        }

        public static async Task<ModItem> getMod(string itemtype, int itemID)
        {
            ModItem downloadedMod = null;

            queryParameters = getDefaultParameters();
            queryParameters.Add(new QueryStringItem("itemid", itemID.ToString()));
            queryParameters.Add(new QueryStringItem("itemtype", itemtype));
            queryParameters.Add(new QueryStringItem("fields", "name,authors,description,downloads"));

            string queryURL = formatApiRequest("Core/Item/Data", queryParameters);

            HttpResponseMessage response = await client.GetAsync(queryURL);

            if (response.IsSuccessStatusCode)
            {
                string responseText = await response.Content.ReadAsStringAsync();

                downloadedMod = JsonConvert.DeserializeObject<ModItem>(responseText);

            }
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

        #region HttpClient Management

        public static void resetClient()
        {
            client = new HttpClient();

            client.BaseAddress = new Uri(HTTPUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }
        #endregion



    }
}

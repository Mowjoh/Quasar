using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quasar.Data.V2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Quasar
{

    


    public class QueryStringItem
    {
        public string Name { get; set; }
        public string Value { get; set; }


        public QueryStringItem(string _Name, string _Value)
        {
            this.Name = _Name;
            this.Value = _Value;
        }

        public string Output()
        {
            string outie = Name + "=" + Value;
            return outie;
        }
    }

    public class APIRequest
    {
        static readonly string HTTPUrl = "https://api.gamebanana.com/";

        static readonly QueryStringItem jsonFormat = new QueryStringItem("format", "json_min");
        static readonly QueryStringItem jsonObject = new QueryStringItem("return_keys", "1");

        static List<QueryStringItem> queryParameters;

        #region Mod Actions
        public static async Task<APIMod> GetAPIMod(string _ItemType, string _ItemID)
        {
            
            APIMod DownloadedAPIMod = null;
            try
            {
                queryParameters = GetDefaultParameters();
                queryParameters.Add(new QueryStringItem("itemid", _ItemID));
                queryParameters.Add(new QueryStringItem("itemtype", _ItemType));
                queryParameters.Add(new QueryStringItem("fields", "name,Credits().aAuthors(),description,downloads,catid,Updates().nGetUpdatesCount(),Game().name"));

                string queryURL = FormatAPIRequest("Core/Item/Data", queryParameters);

                using (HttpClient webClient = new HttpClient())
                {
                    HttpResponseMessage response = await webClient.GetAsync(queryURL);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseText = await response.Content.ReadAsStringAsync();

                        DownloadedAPIMod = JsonConvert.DeserializeObject<APIMod>(responseText);
                    }
                }

                DownloadedAPIMod.ID = Int32.Parse(_ItemID);
                DownloadedAPIMod.ModType = _ItemType;
            }
            catch(Exception e)
            {
                
            }

            return DownloadedAPIMod;
        }

        public static async Task<string[]> GetDownloadFileName(string _ItemType, string _ItemID)
        {
            string filename = "";
            string downloadURL = "";

            queryParameters = GetDefaultParameters();
            queryParameters.Add(new QueryStringItem("itemid", _ItemID));
            queryParameters.Add(new QueryStringItem("itemtype", _ItemType)); 
            queryParameters.Add(new QueryStringItem("fields", "Files().aFiles()"));

            string queryURL = FormatAPIRequest("Core/Item/Data", queryParameters);

            using (HttpClient webClient = new HttpClient())
            {
                HttpResponseMessage response = await webClient.GetAsync(queryURL);

                if (response.IsSuccessStatusCode)
                {
                    string responseText = await response.Content.ReadAsStringAsync();

                    Regex fileMatch = new Regex("\"_sFile\":\"(.*?)\"");
                    Match match = fileMatch.Match(responseText);

                    Regex downloadRegex = new Regex("\"_sDownloadUrl\":\"(.*?)\"");
                    Match downloadMatch = downloadRegex.Match(responseText);


                    filename = match.Value;
                    filename = filename.Split('"')[3];

                    downloadURL = downloadMatch.Value;
                    downloadURL = downloadURL.Split('"')[3];
                    downloadURL = downloadURL.Replace("\\/", "/");
                }
            }

            return new string[]{ filename, downloadURL };
        }

        public static async Task<int> GetScreenshot(string _ItemType, string _ItemID, string _GameID, string _TypeID)
        {
            string downloadURL = "";

            queryParameters = GetDefaultParameters();
            queryParameters.Add(new QueryStringItem("itemid", _ItemID));
            queryParameters.Add(new QueryStringItem("itemtype", _ItemType));
            queryParameters.Add(new QueryStringItem("fields", "Preview().sSubFeedImageUrl()"));


            string queryURL = FormatAPIRequest("Core/Item/Data", queryParameters);
            try
            {
                using (HttpClient webClient = new HttpClient())
                {
                    HttpResponseMessage response = await webClient.GetAsync(queryURL);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseText = await response.Content.ReadAsStringAsync();

                        Regex fileMatch = new Regex("sSubFeedImageUrl\\(\\)\":\"(.*?)\"");
                        Match match = fileMatch.Match(responseText);

                        downloadURL = match.Value;
                        downloadURL = downloadURL.Split('"')[2];
                        downloadURL = downloadURL.Replace("\\/", "/");

                        string downloadextension = downloadURL.Split('.')[downloadURL.Split('.').Length - 1];

                        string imageSource = Properties.Settings.Default.DefaultDir + @"\Library\Screenshots\" + _GameID + "_" + _TypeID + "_" + _ItemID + "." + downloadextension;

                        await ModDownloader.DownloadFile(downloadURL, imageSource);
                    }
                }
            }
            catch(Exception e)
            {
                Console.Write("fak");
            }

            return 0;
        }
        #endregion

        #region API Formatting

        public static string FormatAPIRequest(string path, List<QueryStringItem> _Parameters)
        {
            string formattedURL = HTTPUrl + path + "?";

            bool first = true;
            foreach (QueryStringItem item in _Parameters)
            {
                formattedURL += first ? item.Output() : "&" + item.Output();
                if (first) { first = false; }
            }

            return formattedURL;
        }

        public static List<QueryStringItem> GetDefaultParameters()
        {
            return new List<QueryStringItem>{jsonFormat,jsonObject}; 
        }

        #endregion

        public static string GetQuasarDownloadURL(string _Filename, string _ResourceURL, string _APITypeName, string _ModID)
        {
            string extension = _Filename.Split('.')[1];

            string url = "quasar:";
            url += _ResourceURL;
            url += "," + _APITypeName;
            url += "," + _ModID;
            url += "," + extension;


            return url;
        }

    }



}

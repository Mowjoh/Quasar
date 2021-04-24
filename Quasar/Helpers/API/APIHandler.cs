using Newtonsoft.Json;
using Quasar.Data.V2;
using Quasar.Helpers.Downloading;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Quasar.Helpers.API
{
    /// <summary>
    /// Represents a Query String entry for an HTTP URL
    /// </summary>
    public class QueryStringItem
    {

        public string Name { get; set; }
        public string Value { get; set; }
        public string Output()
        {
            string outie = Name + "=" + Value;
            return outie;
        }

        public QueryStringItem(string _Name, string _Value)
        {
            this.Name = _Name;
            this.Value = _Value;
        }
        
    }

    /// <summary>
    /// Represents a Gamebanana API Request
    /// </summary>
    public class APIRequest
    {
        //Gamebanana's Endpoint
        static readonly string HTTPUrl = "https://api.gamebanana.com/";

        //Default Parameters
        static readonly QueryStringItem jsonFormat = new QueryStringItem("format", "json_min");
        static readonly QueryStringItem jsonObject = new QueryStringItem("return_keys", "1");

        static List<QueryStringItem> queryParameters;

        #region API Actions
        /// <summary>
        /// Gets Mod information from Gamebanana's API
        /// </summary>
        /// <param name="_ItemType">Gamebanana's Mod Type</param>
        /// <param name="_ItemID">Gamebanana's Mod ID</param>
        /// <returns>an APIMod object with all info</returns>
        public static async Task<APIMod> GetModInformation(string _ItemType, string _ItemID)
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

        /// <summary>
        /// Gets the download link for a specific Mod
        /// </summary>
        /// <param name="_ItemType">Gamebanana's Mod Type</param>
        /// <param name="_ItemID">Gamebanana's Mod ID</param>
        /// <returns>Download link informations</returns>
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

        /// <summary>
        /// Gets the screenshot from Gamebanana's API
        /// </summary>
        /// <param name="_ItemType">Gamebanana's Mod Type</param>
        /// <param name="_ItemID">Gamebanana's Mod ID</param>
        /// <param name="_GameID">Gamebanana's Game ID</param>
        /// <param name="_TypeID">Gamebanana's Category ID</param>
        /// <returns></returns>
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


        //Bullshit function
        public static async Task<List<APIMod>> GetAPIModUpdates(ObservableCollection<LibraryItem> Library)
        {
            bool abort = false;

            List<APIMod> ElementCollection = new List<APIMod>();

            queryParameters = GetDefaultParameters();

            foreach (LibraryItem li in Library)
            {
                queryParameters.Add(new QueryStringItem("itemid[]", li.ID.ToString()));
                queryParameters.Add(new QueryStringItem("itemtype[]", li.APICategoryName));
                queryParameters.Add(new QueryStringItem("fields[]", "Updates().nGetUpdatesCount()"));
            }

            try
            {
                string queryURL = FormatAPIRequest("Core/Item/Data", queryParameters);

                using (HttpClient webClient = new HttpClient())
                {
                    HttpResponseMessage response = await webClient.GetAsync(queryURL);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseText = await response.Content.ReadAsStringAsync();
                        ElementCollection = JsonConvert.DeserializeObject<List<APIMod>>(responseText);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }



            if (!abort)
            {
                try
                {
                    int i = 0;
                    foreach (LibraryItem li in Library)
                    {
                        ElementCollection[i].ID = li.ID;
                        ElementCollection[i].ModType = li.APICategoryName;
                        i++;
                    }
                }
                catch (Exception e)
                {
                    abort = true;
                }
            }
            if (abort)
            {
                return null;
            }
            else
            {
                return ElementCollection;
            }

        }
        #endregion

        #region API Formatting

        /// <summary>
        /// Formats the full URL to contact
        /// </summary>
        /// <param name="path">API Endpoint</param>
        /// <param name="_Parameters">List of Query String Parameters</param>
        /// <returns>The full URL</returns>
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

        /// <summary>
        /// Sets up the default Parameter List
        /// </summary>
        /// <returns>the default Parameter List</returns>
        public static List<QueryStringItem> GetDefaultParameters()
        {
            return new List<QueryStringItem>{jsonFormat,jsonObject}; 
        }

        /// <summary>
        /// Returns a formatted Quasar URL
        /// </summary>
        /// <param name="_Filename"></param>
        /// <param name="_ResourceURL"></param>
        /// <param name="_APITypeName"></param>
        /// <param name="_ModID"></param>
        /// <returns></returns>
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
        #endregion

    }

}

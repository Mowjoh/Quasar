using DataModels.Common;
using ImageProcessor;
using ImageProcessor.Plugins.WebP.Imaging.Formats;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Drawing;
using System.Net;

namespace Workshop.Web
{
    

    /// <summary>
    /// Represents a Gamebanana API Request
    /// </summary>
    public class APIRequest
    {
        //Gamebanana's Endpoint
        static readonly string HTTPUrl = "https://gamebanana.com/apiv7/Mod";

        //Default Parameters
        static readonly QueryStringItem jsonFormat = new QueryStringItem("format", "json_min");
        static readonly QueryStringItem jsonObject = new QueryStringItem("return_keys", "1");

        static string ModInformationFields = @"_sName,_aCredits,_aGame,_aCategory,_aSuperCategory,_aModManagerIntegrations,_aSubmitter";
        static string DownloadInfoFields = @"_aFiles";
        static string ScreenshotFields = @"_aPreviewMedia";

        #region API Actions
        /// <summary>
        /// Gets Mod information from Gamebanana's API
        /// </summary>
        /// <param name="_ItemType">Gamebanana's Mod Type</param>
        /// <param name="_ItemID">Gamebanana's Mod ID</param>
        /// <returns>an APIMod object with all info</returns>
        public static async Task<APIMod> GetModInformation(string _ItemID)
        {

            APIMod ParsedAPIMod = null;

            try
            {
                string queryURL = String.Format(@"{0}/{1}?_csvProperties={2}", HTTPUrl, _ItemID, ModInformationFields);

                using ( HttpClient webClient = new HttpClient())
                {
                    HttpResponseMessage response = await webClient.GetAsync(queryURL);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseText = await response.Content.ReadAsStringAsync();

                        ParsedAPIMod = JsonConvert.DeserializeObject<APIMod>(responseText);
                    }
                }

                ParsedAPIMod.ID = Int32.Parse(_ItemID);
                ParsedAPIMod.GamebananaRootCategoryName = "Mod";
            }
            catch (Exception e)
            {

            }

            return ParsedAPIMod;
        }

        /// <summary>
        /// Gets the download link for a specific Mod
        /// </summary>
        /// <param name="_ItemType">Gamebanana's Mod Type</param>
        /// <param name="_ItemID">Gamebanana's Mod ID</param>
        /// <returns>Download link informations</returns>
        public static async Task<APIDownloadInformation> GetDownloadFileName(string _ItemID)
        {
            APIDownloadInformation DownloadInformation = new();

            string queryURL = String.Format(@"{0}/{1}?_csvProperties={2}", HTTPUrl, _ItemID, DownloadInfoFields);

            using (HttpClient webClient = new HttpClient())
            {
                HttpResponseMessage response = await webClient.GetAsync(queryURL);

                if (response.IsSuccessStatusCode)
                {
                    string responseText = await response.Content.ReadAsStringAsync();

                    DownloadInformation = JsonConvert.DeserializeObject<APIDownloadInformation>(responseText);
                    DownloadInformation.QuasarURL = GetQuasarDownloadURL(DownloadInformation.Files[0].File, DownloadInformation.Files[0].DownloadURL, _ItemID);
                }
            }

            return DownloadInformation;
        }

        /// <summary>
        /// Gets the screenshot from Gamebanana's API
        /// </summary>
        /// <param name="_ItemType">Gamebanana's Mod Type</param>
        /// <param name="_ItemID">Gamebanana's Mod ID</param>
        /// <param name="_GameID">Gamebanana's Game ID</param>
        /// <param name="_TypeID">Gamebanana's Category ID</param>
        /// <returns></returns>
        public static async Task<APIScreenshot> GetScreenshotInformation(string _ItemID)
        {
            APIScreenshot ScreenshotInformation = new();

            string queryURL = String.Format(@"{0}/{1}?_csvProperties={2}", HTTPUrl, _ItemID, ScreenshotFields);

            try
            {
                using (HttpClient webClient = new HttpClient())
                {
                    HttpResponseMessage response = await webClient.GetAsync(queryURL);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseText = await response.Content.ReadAsStringAsync();
                        ScreenshotInformation = JsonConvert.DeserializeObject<APIScreenshot>(responseText);
                    }
                }
            }
            catch (Exception e)
            {
                Console.Write("fak");
            }

            return ScreenshotInformation;
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
            string formattedURL = "https://api.gamebanana.com/" + path + "?";

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
            return new List<QueryStringItem> { jsonFormat, jsonObject };
        }

        /// <summary>
        /// Returns a formatted Quasar URL
        /// </summary>
        /// <param name="_Filename"></param>
        /// <param name="_ResourceURL"></param>
        /// <param name="_APITypeName"></param>
        /// <param name="_ModID"></param>
        /// <returns></returns>
        public static string GetQuasarDownloadURL(string _Filename, string _ResourceURL, string ModID)
        {
            string extension = _Filename.Split('.')[1];
            string ResourceURL = _ResourceURL.Replace("dl", "mmdl");
            string url = "quasar:";
            url += ResourceURL;
            url += "," + "Mod";
            url += "," + ModID;
            url += "," + extension;


            return url;
        }
        #endregion

    }

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

}

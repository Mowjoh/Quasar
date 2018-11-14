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
        static HttpClient client = new HttpClient();
        static QueryStringItem jsonFormat = new QueryStringItem("format", "json_min");

        static List<QueryStringItem> parameters;

        public APIRequest()
        {
            parameters = new List<QueryStringItem>();
            parameters.Add(jsonFormat);
        }

        public APIRequest(string url)
        {
            HTTPUrl = url;
        }

        public static async Task<Member> RunAsync(string userID)
        {
            Member member = null;

            client.BaseAddress = new Uri(HTTPUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                member = await GetMemberAsync(userID);
                Console.WriteLine(member.name);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return member;
        }

        public static async Task<Member> GetMemberAsync(string userID)
        {
            parameters = setBaseParameters();

            Member member = null;
            string responseText = null;

            parameters.Add(new QueryStringItem("userid", userID));
            string currentURL = formatApiRequest("Core/Member/IdentifyById", parameters);
            HttpResponseMessage response = await client.GetAsync(currentURL);

            if (response.IsSuccessStatusCode)
            {
                responseText = await response.Content.ReadAsStringAsync();
                string name = responseText;

                member = new Member();
                member.name = name;

                Console.WriteLine(responseText);
            }
            return member;
        }

        public static async Task<string> GetResponseAsync(string path)
        {
            string responseText = null;
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                responseText = await response.Content.ReadAsStringAsync();
            }
            return responseText;
        }

        public static string formatApiRequest(string path, List<QueryStringItem> parameters)
        {
            Boolean first = true;
            string formattedURL = HTTPUrl + path + "?";


            foreach(QueryStringItem QSI in parameters)
            {
                formattedURL += first ? QSI.output() : "&" + QSI.output();
                if (first) { first = false; }
            }

            return formattedURL;
        }

        public static List<QueryStringItem> setBaseParameters()
        {
            List<QueryStringItem> paramies = new List<QueryStringItem>();
            paramies.Add(jsonFormat);

            return paramies;
        }
    }
}

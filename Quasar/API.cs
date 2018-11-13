using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quasar
{
    class Member
    {
        public String name { get; set; }
    }


    public class APIRequest
    {
        static String HTTPUrl = "https://api.gamebanana.com/";
        static HttpClient client = new HttpClient();

        public APIRequest()
        {
            RunAsync();
        }

        public APIRequest(String url)
        {
            HTTPUrl = url;
        }

        public static async Task RunAsync()
        {
            Member member = new Member();
            String responseText = null;

            client.BaseAddress = new Uri(HTTPUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                member = await GetMemberAsync("https://api.gamebanana.com/Core/Item/Data?itemtype=Member&itemid=1382&fields=name");
                Console.WriteLine(member.name);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        static async Task<Member> GetMemberAsync(string path)
        {
            Member member = null;
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                member = await response.Content.ReadAsAsync<Member>();
            }
            return member;
        }

        static async Task<string> GetResponseAsync(String path)
        {
            String responseText = null;
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                responseText = await response.Content.ReadAsStringAsync();
            }
            return responseText;
        }
    }
}

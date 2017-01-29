using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace StockBot
{
    public class LUISStockClient
    {
        public static async Task<Rootobject> ParseUserInput(string strInput)
        {
            string strRet = string.Empty;
            string strEscaped = Uri.EscapeDataString(strInput);

            using (var client = new HttpClient())
            {
                string uri = "https://api.projectoxford.ai/luis/v2.0/apps/a7be928c-14db-4c0b-86ff-d492200428a9?subscription-key=caf34c8bbdec40178df501e2c171654a&q=" + strEscaped;
                uri = uri + "&verbose=true";
                HttpResponseMessage msg = await client.GetAsync(uri);
                if (msg.IsSuccessStatusCode)
                {
                    var jsonResponse = await msg.Content.ReadAsStringAsync();
                    var _Data = JsonConvert.DeserializeObject<Rootobject>(jsonResponse);
                    return _Data;
                }
            }
            return null;
        }
    }
    public class Rootobject
    {
        public string query { get; set; }
        public Topscoringintent topScoringIntent { get; set; }
        public Intent[] intents { get; set; }
        public Entity[] entities { get; set; }
    }

    public class Topscoringintent
    {
        public string intent { get; set; }
        public float score { get; set; }
    }

    public class Intent
    {
        public string intent { get; set; }
        public float score { get; set; }
    }

    public class Entity
    {
        public string entity { get; set; }
        public string type { get; set; }
        public int startIndex { get; set; }
        public int endIndex { get; set; }
        public float score { get; set; }
    }

}
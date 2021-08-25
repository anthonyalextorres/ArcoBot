using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcoBot.JsonObjects
{
    public class JPubSub
    {
        public class Data
        {

            [JsonProperty("topics")]
            public string[] Topics { get; set; }

            [JsonProperty("auth_token")]
            public string AuthToken { get; set; }
        }

        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("nonce")]
        public string Nonce { get; set; }

        [JsonProperty("data")]
        public Data DataWrap { get; set; }

        public JPubSub()
        {
            DataWrap = new Data();
        }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}

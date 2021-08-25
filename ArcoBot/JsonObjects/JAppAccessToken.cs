using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcoBot.JsonObjects
{
    [JsonObject]
    public class JAppAccessToken : JObject<JAppAccessToken>
    {
        [JsonProperty("access_token")]
        public string AccessToken;

        [JsonProperty("expires_in")]
        public int Expires;

        public JAppAccessToken(object json) 
            : base(json) { }
    }
}

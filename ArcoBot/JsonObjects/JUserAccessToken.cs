using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcoBot.JsonObjects
{
    public class JUserAccessToken : JObject<JUserAccessToken>
    {
        [JsonProperty("access_token")]
        public string AccessToken;

        [JsonProperty("expires_in")]
        public int Expires;

        [JsonProperty("refresh_token")]
        public string RefreshToken;


        public JUserAccessToken(object json)
            : base(json) { }
    }
}

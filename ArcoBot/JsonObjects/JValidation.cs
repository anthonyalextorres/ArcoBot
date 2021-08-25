using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcoBot.JsonObjects
{
    public class JValidation : JObject<JValidation>
    {
        [JsonProperty("client_id")]
        public string ClientID;

        [JsonProperty("login")]
        public string Login;

        [JsonProperty("user_id")]
        public string UserID;
        
        [JsonProperty("expires_in")]
        public int ExpirationSeconds;


        public JValidation(object json)
            : base(json) { }
    }
}

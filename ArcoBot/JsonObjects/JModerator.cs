using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcoBot.JsonObjects
{
    [JsonObject]
    public class JModerator : JObject<JUser>
    {
        [JsonProperty("user_id")]
        public string ID;

        [JsonProperty("user_login")]
        public string Login;

        [JsonProperty("user_name")]
        public string UserName;
        public JModerator(object json)
    : base(json) { }
    }
}

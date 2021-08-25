using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ArcoBot.JsonObjects
{
    [JsonObject]
    public class JFollow : JObject<JFollow>
    {
        [JsonProperty("from_id")]
        public ulong FromID;
        [JsonProperty("from_login")]
        public string FromLogin;
        [JsonProperty("from_name")]
        public string FromName;
        [JsonProperty("to_id")]
        public ulong ToID;
        [JsonProperty("to_login")]
        public string ToLogin;
        [JsonProperty("to_name")]
        public string ToName;
        [JsonProperty("followed_at")]
        public DateTime FollowDate;

        public JFollow(object json)
            : base(json) { }
        
    }
}

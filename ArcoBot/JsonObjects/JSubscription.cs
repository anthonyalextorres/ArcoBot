using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcoBot.JsonObjects
{
    [JsonObject]
    public class JSubscription : JObject<JSubscription>
    {
        [JsonProperty("broadcaster_id")]
        public ulong BroadcasterID;

        [JsonProperty("broadcaster_login")]
        public string BroadcasterLogin;

        [JsonProperty("broadcaster_name")]
        public string BroadcasterName;

        [JsonProperty("gifter_id")]
        public string GifterID;

        [JsonProperty("gifter_login")]
        public string GifterLogin;

        [JsonProperty("gifter_name")]
        public string GifterName;

        [JsonProperty("is_gift")]
        public bool IsGift;

        [JsonProperty("plan_name")]
        public string PlanName;

        [JsonProperty("user_id")]
        public ulong UserID;

        [JsonProperty("user_name")]
        public string UserName;

        [JsonProperty("user_login")]
        public string UserLogin;

        public JSubscription(object json)
            : base(json) { }
    }
}

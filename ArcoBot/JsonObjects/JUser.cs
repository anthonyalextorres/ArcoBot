using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcoBot.JsonObjects
{
    [JsonObject]
    public class JUser : JObject<JUser>
    {
        [JsonProperty("id")]
        public string ID;

        [JsonProperty("login")]
        public string Login;

        [JsonProperty("display_name")]
        public string DisplayName;

        [JsonProperty("type")]
        public string Type;

        [JsonProperty("broadcaster_type")]
        public string BroadcasterType;

        [JsonProperty("description")]
        public string Description;

        [JsonProperty("profile_image_url")]
        public string ProfileImageURL;

        [JsonProperty("offline_image_url")]
        public string OfflineImageURL;

        [JsonProperty("view_count")]
        public ulong ViewCount;

        [JsonProperty("email")]
        public string Email;

        public JUser(object json)
            : base(json) { }
    }
}

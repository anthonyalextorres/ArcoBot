
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcoBot.JsonObjects
{
    public class JLiveStream : JObject <JLiveStream>
    {
        [JsonProperty("id")]
        ulong id;

        [JsonProperty("user_id")]
        ulong user_id;

        [JsonProperty("user_login")]
        string user_login;

        [JsonProperty("user_name")]
        public string user_name;

        [JsonProperty("game_id")]
        string game_id;

        [JsonProperty("type")]
        string type;

        [JsonProperty("title")]
        public string title;

        [JsonProperty("viewer_count")]
        public ulong viewer_count;

        [JsonProperty("started_at")]
        string started_at;

        [JsonProperty("language")]
        string language;

        [JsonProperty("thumbnail_url")]
        string thumbnail_url;

        public JLiveStream(object json)
            :base(json)
        {

        }
    }
}

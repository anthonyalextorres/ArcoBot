using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcoBot.JsonObjects
{
    class JClip : JObject<JClip>
    {
        [JsonProperty("id")]
        public string ID { get; internal set; }

        [JsonProperty("url")]
        public string URL;

        [JsonProperty("embed_url")]
        public string EmbedURL;

        [JsonProperty("broadcaster_id")]
        public string BroadcasterID;

        [JsonProperty("broadcaster_name")]
        public string BroadcasterName;

        [JsonProperty("creator_id")]
        public string CreatorID;

        [JsonProperty("creator_name")]
        public string CreatorName;

        [JsonProperty("video_id")]
        public string VideoID;

        [JsonProperty("game_id")]
        public string GameID;

        [JsonProperty("language")]
        public string Language;

        [JsonProperty("title")]
        public string Title;

        [JsonProperty("view_count")]
        public string ViewCount;

        [JsonProperty("created_at")]
        public string CreationDate;

        [JsonProperty("thumbnail_url")]
        public string ThumbnailURL;

        [JsonProperty("edit_url")]
        public string EditURL;
        public JClip(object json)
            : base(json) { }
    }
}

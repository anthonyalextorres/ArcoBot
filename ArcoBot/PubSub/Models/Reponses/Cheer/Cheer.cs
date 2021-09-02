using System;
using Newtonsoft.Json;

namespace ArcoBot.PubSub.Models.Reponses.Cheer {
    public class Cheer : MessageData
    {
        [JsonProperty("user_name")]
        public string UserName { get; internal set; }

        [JsonProperty("channel_name")]
        public string ChannelName { get; internal set; }

        [JsonProperty("user_id")]

        public string UserId { get; internal set; }

        [JsonProperty("channel_id")]

        public string ChannelId { get; internal set; }

        [JsonProperty("time")]

        public DateTime Time { get; internal set; }

        [JsonProperty("chat_message")]

        public string ChatMessage { get; internal set; }

        [JsonProperty("bits_used")]

        public string BitsUsed { get; internal set; }

        [JsonProperty("total_bits_used")]

        public string TotalBitsUsed { get; internal set; }

        [JsonProperty("context")]

        public string Context { get; internal set; }

        [JsonProperty("badge_entitlement")]

        public BadgeEntitlement BadgeEntitlement { get; internal set; }

        public Cheer(string json)
        {
            JsonConvert.PopulateObject(json, this);
        }
    }
}
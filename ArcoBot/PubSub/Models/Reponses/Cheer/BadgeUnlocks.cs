using System;
using Newtonsoft.Json;
namespace ArcoBot.PubSub.Models.Reponses.Cheer
{
    public class BitsBadgeUnlocks : MessageData
    {
        [JsonProperty("user_name")]
        public string Username { get; internal set; }

        [JsonProperty("channel_id")]
        public string ChannelId { get; internal set; }

        [JsonProperty("channel_name")]
        public string ChannelName { get; internal set; }

        [JsonProperty("badge_tier")]
        public string BadgeTier { get; internal set; }

        [JsonProperty("chat_message")]
        public string ChatMessage { get; internal set; }

        [JsonProperty("time")]
        public DateTime Time { get; internal set; }

        public BitsBadgeUnlocks(string json)
        {
            JsonConvert.PopulateObject(json, this);
        }
    }
}
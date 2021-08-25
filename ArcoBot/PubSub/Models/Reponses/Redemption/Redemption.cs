using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcoBot.PubSub.Models.Reponses.Redemption
{
    public class Redemption
    {
        [JsonProperty("id")]
        public string Id { get; protected set; }
        [JsonProperty("user")]
        public User User { get; protected set; }
        [JsonProperty("channel_id")]
        public string ChannelId { get; protected set; }
        [JsonProperty("redeemed_at")]
        public DateTime RedeemedAt { get; protected set; }
        [JsonProperty("reward")]
        public Reward Reward { get; protected set; }
        [JsonProperty("user_input")]
        public string UserInput { get; protected set; }
        [JsonProperty("status")]
        public string Status { get; protected set; }
    }
}

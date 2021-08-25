using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;

namespace ArcoBot.PubSub.Models.Reponses.Subscription
{
    public class Subscribe : MessageData
    {
        [JsonProperty("user_name")]
        public string Username { get; set; }
        [JsonProperty("display_name")]
        public string DisplayName { get; set; }
        [JsonProperty("channel_name")]
        public string ChannelName { get; set; }
        [JsonProperty("user_id")]
        public string UserId { get; set; }
        [JsonProperty("channel_id")]
        public string ChannelId { get; set; }
        [JsonProperty("time")]
        public DateTime Time { get; set; }
        [JsonProperty("sub_plan")]
        public string SubPlan { get; set; }
        [JsonProperty("sub_plan_name")]
        public string SubPlanName { get; set; }
        [JsonProperty("cumulative_months")]
        public int CumulativeMonths { get; set; }
        [JsonProperty("streak_months")]
        public int StreakMonths { get; set; }
        [JsonProperty("context")]
        public string Context { get; set; }
        [JsonProperty("is_gift")]
        public bool IsGift { get; set; }
        [JsonProperty("sub_message")]
        public SubMessage SubMessage { get; set; }

        public Subscribe(string json)
        {
            JsonConvert.PopulateObject(json, this);
        }
    }
}

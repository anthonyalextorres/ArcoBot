using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcoBot.JsonObjects
{
    [JsonObject]
    public class JPollCreate
    {
       [JsonProperty("broadcaster_id")]
        public string BroadcasterID;
        [JsonProperty("title")]
        public string Title;
        [JsonProperty("choices")]
        public JPollChoice[] Choices;
        [JsonProperty("channel_points_voting_enabled")]
        public bool ChannelPointsVotingEnabled;
        [JsonProperty("channel_points_per_vote")]
        public int ChannelPointsPerVote;
        [JsonProperty("duration")]
        public int Duration;
    }
    [JsonObject]
    public class JPollChoice
    {
        [JsonProperty("title")]
        public string Title;
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcoBot.PubSub.Models.Reponses.Redemption
{
    public class ChannelPointsData
    {
        [JsonProperty(PropertyName = "timestamp")]
        public DateTime Timestamp { get; protected set; }
        [JsonProperty(PropertyName = "redemption")]
        public Redemption Redemption { get; protected set; }
    }
}

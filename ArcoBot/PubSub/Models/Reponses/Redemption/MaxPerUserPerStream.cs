using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcoBot.PubSub.Models.Reponses.Redemption
{
    public class MaxPerUserPerStream
    {
        [JsonProperty("is_enabled")]
        public string IsEnabled { get; protected set; }
        [JsonProperty("max_per_user_per_stream")]
        public int MaxPerUserPerStreamValue { get; protected set; }
    }
}

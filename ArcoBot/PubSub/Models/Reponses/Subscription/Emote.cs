using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;

namespace ArcoBot.PubSub.Models.Reponses.Subscription
{
    public class Emote
    {
        [JsonProperty("start")]
        public int Start { get; set; }

        [JsonProperty("end")]
        public int End { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }
    }
}

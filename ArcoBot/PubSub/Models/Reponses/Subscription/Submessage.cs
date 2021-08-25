using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcoBot.PubSub.Models.Reponses.Subscription
{
    public class SubMessage
    {
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("emotes")]
        public List<Emote> Emotes { get; set; }
    }

}

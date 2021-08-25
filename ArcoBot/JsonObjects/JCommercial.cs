using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcoBot.JsonObjects
{
    [JsonObject]
    public class JCommercial
    {
        [JsonProperty("broadcaster_id")]
        public string BroadcasterID;

        [JsonProperty("length")]
        public string Length;

        public JCommercial(string broadcasterID, string length) { BroadcasterID = broadcasterID; Length = length; }
    }
}

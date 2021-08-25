using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcoBot.JsonObjects
{
    [JsonObject]
    public class JClipCreation : JObject<JClipCreation>
    {
        [JsonProperty("id")]
        public string ID;

        [JsonProperty("edit_url")]
        public string EditURL;

        public JClipCreation(object json)
            : base(json) { }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcoBot.JsonObjects
{
    public class JResponse : JObject<JResponse>
    {
        [JsonProperty("error")]
        public string Error;

        [JsonProperty("status")]
        public string Status;

        [JsonProperty("message")]
        public string Message;


        public JResponse(object json)
            : base(json) { }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ArcoBot.JsonObjects
{
    [JsonObject("pagination")]
    public class JPagination : JObject<JPagination>
    {
        [JsonProperty("cursor")]
        public string Cursor;

        public JPagination(object json)
            : base(json) { }
    }
}

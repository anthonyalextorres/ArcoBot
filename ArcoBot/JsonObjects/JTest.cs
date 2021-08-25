using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcoBot.JsonObjects
{

    public class JViewerList : JObject<JViewerList>
    {
        [JsonProperty("_links")]
        public _Links Links { get; set; }
        [JsonProperty("chatter_count")]
        public int Count { get; set; }
        [JsonProperty("chatters")]
        public Chatters Chatters { get; set; }

        public JViewerList(object json)
            : base(json)
        {
        }
    }
    public class _Links
    {
    }

    public class Chatters
    {
        [JsonProperty("broadcaster")]
        public string[] Broadcaster;

        [JsonProperty("vips")]
        public string[] VIPs;

        [JsonProperty("moderators")]
        public string[] Moderators;

        [JsonProperty("staff")]
        public string[] Staff;

        [JsonProperty("admins")]
        public string[] Admins;

        [JsonProperty("global_mods")]
        public string[] GlobalMods;

        [JsonProperty("viewers")]
        public string[] Viewers;
    }
}
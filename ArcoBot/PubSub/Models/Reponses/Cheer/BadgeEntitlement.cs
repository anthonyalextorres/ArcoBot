using System;
using Newtonsoft.Json;

namespace ArcoBot.PubSub.Models.Reponses.Cheer
{
    //Set namespace
    public class BadgeEntitlement
    {
        [JsonProperty("new_version")]
        public string NewVersion { get; internal set; }

        [JsonProperty("previous_version")]
        public string PreviousVersion { get; internal set; }
    }
}
//"badge_entitlement\":{\"new_version\":25000,\"previous_version\":10000}
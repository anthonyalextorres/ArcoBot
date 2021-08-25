using Newtonsoft.Json;

namespace ArcoBot.PubSub.Models.Reponses.Redemption
{
    public class RedemptionImage
    {
        [JsonProperty("url_1x")]
        public string Url1x { get; protected set; }
        [JsonProperty("url_2x")]
        public string Url2x { get; protected set; }
        [JsonProperty("url_4x")]
        public string Url4x { get; protected set; }
    }
}

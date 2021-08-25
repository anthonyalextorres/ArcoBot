using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcoBot.PubSub.Models.Reponses
{
    public class User
    {
        [JsonProperty("id")]
        public string Id { get; protected set; }
        [JsonProperty("login")]
        public string Login { get; protected set; }
        [JsonProperty("display_name")]
        public string DisplayName { get; protected set; }
    }
}

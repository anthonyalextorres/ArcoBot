using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcoBot.PubSub.Models.Reponses.Redemption
{
    public class GlobalCooldown
    {
        [JsonProperty("is_enabled")]
        public string IsEnabled { get; protected set; }
        [JsonProperty("global_cooldown_seconds")]
        public int GlobalCooldownSeconds { get; protected set; }
    }
}

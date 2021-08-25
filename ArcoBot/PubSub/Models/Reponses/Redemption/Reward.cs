using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcoBot.PubSub.Models.Reponses.Redemption
{
    public class Reward
    {
        [JsonProperty("id")]
        public string Id { get; protected set; }
        [JsonProperty("channel_id")]
        public string ChannelId { get; protected set; }
        [JsonProperty("title")]
        public string Title { get; protected set; }
        [JsonProperty("prompt")]
        public string Prompt { get; protected set; }
        [JsonProperty("cost")]
        public int Cost { get; protected set; }
        [JsonProperty("is_user_input_required")]
        public bool IsUserInputRequired { get; protected set; }
        [JsonProperty("is_sub_only")]
        public bool IsSubOnly { get; protected set; }
        [JsonProperty("image")]
        public RedemptionImage Image { get; protected set; }
        [JsonProperty("default_image")]
        public RedemptionImage DefaultImage { get; protected set; }
        [JsonProperty("background_color")]
        public string BackgroundColor { get; protected set; }
        [JsonProperty("is_enabled")]
        public bool IsEnabled { get; protected set; }
        [JsonProperty("is_paused")]
        public bool IsPaused { get; protected set; }
        [JsonProperty("is_in_stock")]
        public bool IsInStock { get; protected set; }
        [JsonProperty("max_per_stream")]
        public MaxPerUserPerStream MaxPerStream { get; protected set; }
        [JsonProperty("should_redemptions_skip_request_queue")]
        public bool ShouldRedemptionsSkipRequestQueue { get; protected set; }
        [JsonProperty("template_id")]
        public string TemplateId { get; protected set; }
        [JsonProperty("updated_for_indicator_at")]
        public DateTime UpdatedForIndicatorAt { get; protected set; }
        [JsonProperty("max_per_user_per_stream")]
        public MaxPerUserPerStream MaxPerUserPerStream { get; protected set; }
        [JsonProperty("global_cooldown")]
        public GlobalCooldown GlobalCooldown { get; protected set; }
        [JsonProperty("cooldown_expires_at")]
        public DateTime? CooldownExpiresAt { get; protected set; }
    }
}
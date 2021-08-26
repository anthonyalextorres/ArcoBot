using System;
using Newtonsoft.Json;
//Set namespace
public class Cheer : MessageData
{
    [JsonProperty("user_name")]
    public string UserName { get; internal set; }

    [JsonProperty("channel_name")]
    public string ChannelName { get; internal set; }

    [JsonProperty("user_id")]

    public string UserId { get; internal set; }

    [JsonProperty("channel_id")]

    public string ChannelId { get; internal set; }

    [JsonProperty("time")]

    public DateTime Time { get; internal set; }

    [JsonProperty("chat_message")]

    public string ChatMessage { get; internal set; }

    [JsonProperty("bits_used")]

    public string BitsUsed { get; internal set; }

    [JsonProperty("total_bits_used")]

    public string TotalBitsUsed { get; internal set; }

    [JsonProperty("context")]

    public string Context { get; internal set; }

    [JsonProperty("badge_entitlement")]

    public BadgeEntitlement BadgeEntitlement { get; internal set; }

    public Cheer(string json)
	{
        JsonConverter.PopulateObject(this, json);
	}
}

//"message": "{\"data\":{\"user_name\":\"dallasnchains\",\"channel_name\":\"dallas\",\"user_id\":\"129454141\",\"channel_id\":\"44322889\",
//\"time\":\"2017-02-09T13:23:58.168Z\",\"chat_message\":\"cheer10000 New badge hype!\",\"bits_used\":10000,\"total_bits_used\":25000,\
//"context\":\"cheer\",\"badge_entitlement\":{\"new_version\":25000,\"previous_version\":10000}},\"version\":\"1.0\",\"message_type\":\"bits_event\",\
//"message_id\":\"8145728a4-35f0-4cf7-9dc0-f2ef24de1eb6\"}"

using ArcoBot.PubSub.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcoBot.PubSub.Models.Reponses.Redemption
{

    public class ChannelPointsChannel : MessageData
    {
        public ChannelPointsType Type { get; private set; }
        public ChannelPointsData Data { get; private set; }
        public ChannelPointsChannel(string jsonStr)
        {
            JToken json = JObject.Parse(jsonStr);
            switch (json.SelectToken("type").ToString())
            {
                case "reward-redeemed":
                    Type = ChannelPointsType.RewardRedeemed;
                    Data = JsonConvert.DeserializeObject<ChannelPointsData>(json.SelectToken("data").ToString());
                    break;
                default:
                    Type = ChannelPointsType.Unknown;
                    break;
            }
        }
    }

}

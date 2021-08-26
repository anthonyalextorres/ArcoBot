using ArcoBot.JsonObjects;
using ArcoBot.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using ArcoBot.PubSub.Models.Reponses.Redemption;
using ArcoBot.PubSub.Events;
using ArcoBot.PubSub.Models.Reponses;
using System.Threading;
using ArcoBot.PubSub.Models.Reponses.Subscription;

namespace ArcoBot
{
    public class PubSubManager
    {
        /// <summary>
        /// Triggers when PubSub receives incoming data about channel point redemption event.
        /// </summary>
        public event EventHandler<OnChannelPointsRedeemedArgs> OnPointsRedeemed;

        /// <summary>
        /// Triggers when PubSub receives incoming data about a new follow event.
        /// </summary>
        public event EventHandler<OnFollowArgs> OnFollow;

        /// <summary>
        /// Triggers when PubSub receives incoming data about a new subscription event.
        /// </summary>
        public event EventHandler<OnSubscribeArgs> OnSubscribe;

        /// <summary>
        /// Triggers when PubSub receives incoming data about a new cheer event.
        /// </summary>
        public event EventHandler<OnCheerArgs> OnCheer;


        /// <summary>
        /// Triggers when PubSub receives incoming data about a new bits badge unlock event.
        /// </summary>
        public event EventHandler<OnBitsBadgeUnlocksArgs> OnBitsBadgeUnlock;

        private readonly WebSocketClient client;
        private readonly Random nonceRandom;
        private readonly string channelID;
        private readonly string oauth;
        private readonly Timer pingTimer;
        private DateTime lastPong;
        //ping/pong timer;

        public PubSubManager(string _authToken, string _channelID)
        {
            client = new WebSocketClient();
            nonceRandom = new Random();
            channelID = _channelID;
            oauth = _authToken.Replace("oauth:", "");
            lastPong = new DateTime();
        }
        public void Initialize()
        {
            if (client.Open())
                 client.Listen();
            client.OnMessage += OnMessage;

             InitializeListener();
        }

        private void OnMessage(object sender, Network.Events.OnMessageEventArgs e)
            {
            var type = JObject.Parse(e.Message).SelectToken("type")?.ToString();
            switch(type?.ToLower())
            {
                case "response":break;

                case "message":
                    var message = new PubSub.Models.Reponses.Message(e.Message);
                    switch (message.Topic.Split('.')[0])
                    {
                        case "channel-points-channel-v1":
                            var redemptionData = (message.MessageData as ChannelPointsChannel).Data.Redemption;
                            OnPointsRedeemed?.Invoke(this, new OnChannelPointsRedeemedArgs { ChannelId = redemptionData.ChannelId, Redemption = redemptionData});
                            break;

                        case "following":
                            var followData = (Follow)message.MessageData;
                            OnFollow?.Invoke(this, new OnFollowArgs { DisplayName = followData.DisplayName, FollowedChannelId = followData.FollowedChannelId, UserId = followData.UserId, UserName = followData.Username });
                            break;
                        case "channel-subscribe-events-v1"://TODO fire separately for gifted subs.
                            var subData = (Subscribe)message.MessageData;
                            OnSubscribe?.Invoke(this, new OnSubscribeArgs(subData))
                            break;

                        case "channel-bits-events-v1":
                            var cheerData = (Cheer)message.MessageData;
                            OnCheer?.Invoke(this, new OnCheerArgs(cheerData));
                            break;

                        case "channel-bits-badge-unlocks":
                            var bitsBadgeUnlocksData = (BitsBadgeUnlocks)message.MessageData;
                            OnBitsBadgeUnlock?.Invoke(this, new OnBitsBadgeUnlocksArgs(bitsBadgeUnlocksData));
                            break;

                    }
                    break;
                
            }
        }
        /// <summary>
        /// Sends initial PING message to server follwed by listening data.
        /// TODO::::::: PING/PONG TIMER
        /// </summary>
        /// <returns></returns>
        private async Task InitializeListener()
        {
            JPubSub obj1 = new JPubSub();
            obj1.Type = "PING";
            await client.Send(obj1.ToString());

            JPubSub obj = new JPubSub();
            obj.DataWrap = new JPubSub.Data();
            obj.Type = "LISTEN";
            obj.Nonce = GenerateNonce();
            obj.DataWrap.Topics = new string[5] 
            { 
                $"channel-points-channel-v1.{channelID}",
                $"following.{channelID}", 
                $"channel-subscribe-events-v1.{channelID}",
                $"channel-bits-events-v1.{channelID}," +
                $"channel-bits-badge-unlocks.{channelID}
            };
            obj.DataWrap.AuthToken = oauth;
          
            await client.Send(obj.ToString());
        }
        private string GenerateNonce()
        {
            return new string(Enumerable.Repeat("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 8)
                .Select(s => s[nonceRandom.Next(s.Length)]).ToArray());
        }
    }
}
/*

 "message": "{\"data\":{\"user_name\":\"dallasnchains\",\"channel_name\":\"dallas\",\"user_id\":\"129454141\",\"channel_id\":\"44322889\",\"time\":\"2017-02-09T13:23:58.168Z\",\"chat_message\":\"cheer10000 New badge hype!\",\"bits_used\":10000,\"total_bits_used\":25000,\"context\":\"cheer\",\"badge_entitlement\":{\"new_version\":25000,\"previous_version\":10000}},\"version\":\"1.0\",\"message_type\":\"bits_event\",\"message_id\":\"8145728a4-35f0-4cf7-9dc0-f2ef24de1eb6\"}"


*/

using ArcoBot.PubSub.Models.Reponses.Subscription;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcoBot.PubSub.Events
{
    public class OnSubscribeArgs
    {
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string ChannelName { get; set; }
        public string UserId { get; set; }
        public string ChannelId { get; set; }
        public DateTime Time { get; set; }
        public string SubPlan { get; set; }
        public string SubPlanName { get; set; }
        public int CumulativeMonths { get; set; }
        public int StreakMonths { get; set; }
        public string Context { get; set; }
        public bool IsGift { get; set; }

        public OnSubscribeArgs(Subscribe data)
        {
            Username = data.Username;
            DisplayName = data.DisplayName;
            ChannelName = data.ChannelName;
            UserId = data.UserId;
            ChannelId = data.ChannelId;
            Time = data.Time;
            SubPlan = data.SubPlan;
            SubPlanName = data.SubPlanName;
            CumulativeMonths = data.CumulativeMonths;
            StreakMonths = data.StreakMonths;
            Context = data.Context;
            IsGift = data.IsGift;

        }
    }
}

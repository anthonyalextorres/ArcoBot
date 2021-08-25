using ArcoBot.PubSub.Models.Reponses.Redemption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcoBot.PubSub.Events
{
    public class OnChannelPointsRedeemedArgs : EventArgs
    {
        public string ChannelId { get; internal set; }
        public Redemption Redemption { get; internal set; }
    }
}

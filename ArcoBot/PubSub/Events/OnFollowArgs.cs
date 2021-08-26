using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcoBot.PubSub.Events
{
    public class OnFollowArgs : EventArgs
    {
        public string FollowedChannelId { get; internal set; }
        public string DisplayName { get; internal set; }
        public string UserId { get; internal set; }
        public string UserName { get; internal set; }
    }
}

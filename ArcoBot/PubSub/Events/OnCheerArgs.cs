using ArcoBot.PubSub.Models.Reponses.Cheer;
using System;

namespace ArcoBot.PubSub.Events
{
    public class OnCheerArgs : EventArgs
    {
        public string UserName { get; internal set; }

        public string ChannelName { get; internal set; }

        public string UserId { get; internal set; }

        public string ChannelId { get; internal set; }

        public DateTime Time { get; internal set; }

        public string ChatMessage { get; internal set; }

        public string BitsUsed { get; internal set; }

        public string TotalBitsUsed { get; internal set; }

        public string Context { get; internal set; }

        public OnCheerArgs(Cheer data)
        {
            UserName = data.UserName;
            ChannelName = data.ChannelName;
            UserId = data.UserId;
            ChannelId = data.ChannelId;
            Time = data.Time;
            ChatMessage = data.ChatMessage;
            BitsUsed = data.BitsUsed;
            TotalBitsUsed = data.TotalBitsUsed;
            Context = data.Context;
        }
    }
}
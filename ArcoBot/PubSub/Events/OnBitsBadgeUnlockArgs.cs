using System;

public class OnBitsBadgeUnlocksArgs
{
    public string Username { get; internal set; }

    public string ChannelId { get; internal set; }

    public string ChannelName { get; internal set; }

    public string BadgeTier { get; internal set; }

    public string ChatMessage { get; internal set; }

    public DateTime Time { get; internal set; }

    public OnBitsBadgeUnlocksArgs(BitsBadgeUnlocks data)
    {
        Username = data.Username;
        ChannelId = data.ChannelId;
        ChannelName = data.ChannelName;
        BadgeTier = data.BadgeTier;
        ChatMessage = data.ChatMessage;
        Time = data.Time;
    }
}

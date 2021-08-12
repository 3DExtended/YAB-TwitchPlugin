using YAB.Core.Events;

namespace TwitchBotPlugin.Events
{
    public class TwitchReSubscribedEvent : UserEventBase
    {
        public int Month { get; set; }
    }
}
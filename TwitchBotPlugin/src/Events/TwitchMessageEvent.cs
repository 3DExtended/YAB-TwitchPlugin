using TwitchBotPlugin.Contracts;

using YAB.Core.Annotations;
using YAB.Core.Events;

namespace TwitchBotPlugin.Events
{
    [ClassDescription("This event is raised for a twitch user, if they send a message into the chat of the joined channel.")]
    public class TwitchMessageEvent : MessageEventBase
    {
        [PropertyDescription(false, "Determines, whether this message was hightlighted.")]
        public bool IsHighlighted { get; set; }

        public TwitchUser User { get; set; }
    }
}

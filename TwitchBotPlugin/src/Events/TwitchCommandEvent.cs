using TwitchBotPlugin.Contracts;

using YAB.Core.Annotations;
using YAB.Core.Events;

namespace TwitchBotPlugin.Events
{
    [ClassDescription("This event is raised for a twitch user, if they send a command into the chat of the joined channel.")]
    public class TwitchCommandEvent : CommandEventBase
    {
        public TwitchUser User { get; set; }
    }
}

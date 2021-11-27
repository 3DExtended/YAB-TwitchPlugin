using YAB.Core.Events;
using YAB.Core.Annotations;

namespace TwitchBotPlugin.Events
{
    [ClassDescription("This event is raised for a twitch user, if they send a command into the chat of the joined channel.")]
    public class TwitchCommandEvent : CommandEventBase
    {
    }
}
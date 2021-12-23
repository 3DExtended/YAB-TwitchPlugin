using YAB.Core.Annotations;

namespace TwitchBotPlugin.Events
{
    [ClassDescription("This event is raised for a twitch user, if they followed the joined channel.")]
    public class TwitchUserFollowedEvent : TwitchUserEventBase
    {
    }
}

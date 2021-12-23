using YAB.Core.Annotations;

namespace TwitchBotPlugin.Events
{
    [ClassDescription("This event is raised for a twitch user, if they subscriped to the joined channel.")]
    public class TwitchNewSubscribedEvent : TwitchUserEventBase
    {
    }
}

using YAB.Core.Annotations;

namespace TwitchBotPlugin.Events
{
    [ClassDescription("This event is raised for a twitch user, if they resubscriped to the joined channel.")]
    public class TwitchReSubscribedEvent : TwitchUserEventBase
    {
        [PropertyDescription(false, "The number of month the user is already subed.")]
        public int Month { get; set; }
    }
}

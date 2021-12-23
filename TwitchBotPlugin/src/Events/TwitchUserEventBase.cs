using TwitchBotPlugin.Contracts;

using YAB.Core.Annotations;
using YAB.Core.Events;

namespace TwitchBotPlugin.Events
{
    public abstract class TwitchUserEventBase : EventBase
    {
        [PropertyDescription(false, "The user which has done something.")]
        public TwitchUser User { get; set; }
    }
}

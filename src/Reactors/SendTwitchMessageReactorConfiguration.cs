using YAB.Core.EventReactor;
using YAB.Core.Events;

namespace TwitchBotPlugin.Reactors
{
    public class SendTwitchMessageReactorConfiguration : IEventReactorConfiguration<SendTwitchMessageReactor, UserEventBase>
    {
        public string SomeRandomProperty { get; set; }
    }
}
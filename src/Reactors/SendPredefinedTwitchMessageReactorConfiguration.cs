using YAB.Core.EventReactor;
using YAB.Core.Events;

namespace TwitchBotPlugin.Reactors
{
    public class SendPredefinedTwitchMessageReactorConfiguration : IEventReactorConfiguration<SendPredefinedTwitchMessageReactor, UserEventBase>
    {
        /// <summary>
        /// This determines what the bot should respond if a pipeline uses the SendPredefinedTwitchMessageReactorConfiguration
        /// </summary>
        public string Answer { get; set; }
    }
}
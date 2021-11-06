using YAB.Core.EventReactor;
using YAB.Core.Events;
using YAB.Plugins.Injectables.Options;

namespace TwitchBotPlugin.Reactors
{
    [ReactorConfigurationDescription("Sends a message in the name of the bot into the joined channel chat.")]
    public class SendPredefinedTwitchMessageReactorConfiguration :
        IEventReactorConfiguration<SendPredefinedTwitchMessageReactor, CommandEventBase>
    {
        /// <summary>
        /// This determines what the bot should respond if a pipeline uses the SendPredefinedTwitchMessageReactorConfiguration
        /// </summary>
        [PropertyDescription(false, "The message that will be send.")]
        public string Answer { get; set; }
    }
}
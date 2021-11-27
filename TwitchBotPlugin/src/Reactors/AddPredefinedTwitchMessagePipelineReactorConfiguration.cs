using TwitchBotPlugin.Events;

using YAB.Core.Annotations;
using YAB.Core.EventReactor;
using YAB.Plugins.Injectables.Options;

namespace TwitchBotPlugin.Reactors
{
    [ClassDescription("Use this reactor to automatically let the joined twitch channel user create new bot responses. Use it like this: \"!addcom {commandName} {response}\"")]
    public class AddPredefinedTwitchMessagePipelineReactorConfiguration : IEventReactorConfiguration<AddPredefinedTwitchMessagePipelineReactor, TwitchCommandEvent>
    {
        [PropertyDescription(false, "The pipeline will be added after this amount of seconds")]
        public int? DelayTaskForSeconds { get ; set ; }
    }
}
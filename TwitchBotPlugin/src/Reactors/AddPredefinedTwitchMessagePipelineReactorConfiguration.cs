using TwitchBotPlugin.Events;

using YAB.Core.EventReactor;
using YAB.Plugins.Injectables.Options;

namespace TwitchBotPlugin.Reactors
{
    [ReactorConfigurationDescription("Use this reactor to automatically let the joined twitch channel user create new bot responses. Use it like this: \"!addcom {commandName} {response}\"")]
    public class AddPredefinedTwitchMessagePipelineReactorConfiguration : IEventReactorConfiguration<AddPredefinedTwitchMessagePipelineReactor, TwitchCommandEvent>
    {
    }
}
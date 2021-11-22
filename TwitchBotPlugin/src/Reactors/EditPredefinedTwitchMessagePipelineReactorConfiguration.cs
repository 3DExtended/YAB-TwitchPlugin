using TwitchBotPlugin.Events;

using YAB.Core.EventReactor;
using YAB.Plugins.Injectables.Options;

namespace TwitchBotPlugin.Reactors
{
    [ReactorConfigurationDescription("Use this reactor to update pipelines created with the AddPredefinedTwitchMessagePipelineReactorConfiguration. Use it like this: \"!editcom {commandName} {response}\"")]
    public class EditPredefinedTwitchMessagePipelineReactorConfiguration : IEventReactorConfiguration<EditPredefinedTwitchMessagePipelineReactor, TwitchCommandEvent>
    {
        [PropertyDescription(false, "The pipeline will be edited after this amount of seconds")]
        public int? DelayTaskForSeconds { get; set; }
    }
}
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using TwitchBotPlugin.Events;

using YAB.Core.EventReactor;
using YAB.Core.Pipelines.Filter;
using YAB.Plugins.Injectables;

namespace TwitchBotPlugin.Reactors
{
    public class EditPredefinedTwitchMessagePipelineReactor :
        IEventReactor<EditPredefinedTwitchMessagePipelineReactorConfiguration, TwitchCommandEvent>
    {
        private readonly ILogger _logger;
        private readonly IPipelineStore _pipelineStore;

        public EditPredefinedTwitchMessagePipelineReactor(ILogger logger, IPipelineStore pipelineStore)
        {
            _logger = logger;
            _pipelineStore = pipelineStore;
        }

        public async Task RunAsync(EditPredefinedTwitchMessagePipelineReactorConfiguration config, TwitchCommandEvent evt, CancellationToken cancellationToken)
        {
            // assert evt has at least two arguments: first the command itself and secondly the new response
            var commandName = evt.Arguments?.FirstOrDefault();
            if (commandName == null)
            {
                Module.TwitchClient.Value.SendMessage(Module.TwitchClient.Value.JoinedChannels[0], $"You tried to edit a pipeline but did not specify the name of the command you intended to edit nor the new response.");
                return;
            }

            if (evt.Arguments.Count <= 1)
            {
                Module.TwitchClient.Value.SendMessage(Module.TwitchClient.Value.JoinedChannels[0], $"You tried to edit a pipeline but did not specify the new response for command {commandName}.");
                return;
            }

            var newResponse = string.Join(" ", evt.Arguments.Where((v, index) => index != 0));

            // first, try finding pipeline for event message:
            var pipelinesToEdit = _pipelineStore.Pipelines
                .Where(p => p.EventFilter is Filter filter && filter.PropertyName == "Command" && filter.FilterValue == commandName && p.EventType.FullName == typeof(TwitchCommandEvent).FullName)
                .ToList();

            if (pipelinesToEdit.Count == 0)
            {
                Module.TwitchClient.Value.SendMessage(Module.TwitchClient.Value.JoinedChannels[0], $"You tried to edit a pipeline for command {commandName} but there is no pipeline registered yet. Consider using the addcom pipeline.");
                return;
            }

            if (pipelinesToEdit.Count > 1)
            {
                Module.TwitchClient.Value.SendMessage(Module.TwitchClient.Value.JoinedChannels[0], $"You tried to edit a pipeline for command {commandName} but there are multiple pipelines registered to this command. Please edit this pipeline through the frontend.");
                return;
            }

            var pipelineToEdit = pipelinesToEdit.Single();

            if (pipelineToEdit.PipelineHandlerConfigurations.Count != 1)
            {
                Module.TwitchClient.Value.SendMessage(Module.TwitchClient.Value.JoinedChannels[0], $"You tried to edit a pipeline for command {commandName} but this pipeline uses multiple actions. Please edit this pipeline through the frontend.");
                return;
            }

            var configToEdit = pipelineToEdit.PipelineHandlerConfigurations[0];
            if (configToEdit.GetType().FullName == typeof(SendPredefinedTwitchMessageReactorConfiguration).FullName)
            {
                // edit configuration
                var sendPredefinedTwitchMessageReactorConfiguration = (dynamic)configToEdit;
                sendPredefinedTwitchMessageReactorConfiguration.Answer = newResponse;
                await _pipelineStore.SavePipelinesAsync(cancellationToken).ConfigureAwait(false);
                Module.TwitchClient.Value.SendMessage(Module.TwitchClient.Value.JoinedChannels[0], $"Successfully updated command {commandName}.");

            }
            else
            {
                Module.TwitchClient.Value.SendMessage(Module.TwitchClient.Value.JoinedChannels[0], $"You tried to edit a pipeline for command {commandName} but this pipeline does not use the SendPredefinedTwitchMessageReactorConfiguration, which is the only configuration you can edit through this command.");
            }
        }
    }
}
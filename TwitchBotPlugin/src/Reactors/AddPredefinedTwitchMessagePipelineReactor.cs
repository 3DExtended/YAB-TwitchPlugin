using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using TwitchBotPlugin.Events;

using YAB.Core.EventReactor;
using YAB.Core.Pipeline;
using YAB.Core.Pipelines.Filter;
using YAB.Plugins.Injectables;

namespace TwitchBotPlugin.Reactors
{
    public class AddPredefinedTwitchMessagePipelineReactor :
        IEventReactor<AddPredefinedTwitchMessagePipelineReactorConfiguration, TwitchCommandEvent>
    {
        private readonly ILogger _logger;
        private readonly IPipelineStore _pipelineStore;

        public AddPredefinedTwitchMessagePipelineReactor(ILogger logger, IPipelineStore pipelineStore)
        {
            _logger = logger;
            _pipelineStore = pipelineStore;
        }

        public async Task RunAsync(AddPredefinedTwitchMessagePipelineReactorConfiguration config, TwitchCommandEvent evt, CancellationToken cancellationToken)
        {
            if (Module.TwitchClient.Value.JoinedChannels.Count == 0)
            {
                return;
            }

            // assert evt has at least two arguments: first the command itself and secondly the new response
            var commandName = evt.Arguments?.FirstOrDefault();
            if (commandName == null)
            {
                Module.TwitchClient.Value.SendMessage(Module.TwitchClient.Value.JoinedChannels[0], $"You tried to add a pipeline but did not specify the name of the command you intended to add nor its response.");
                return;
            }

            if (evt.Arguments.Count <= 1)
            {
                Module.TwitchClient.Value.SendMessage(Module.TwitchClient.Value.JoinedChannels[0], $"You tried to add a pipeline but did not specify the response for command {commandName}.");
                return;
            }

            var response = string.Join(" ", evt.Arguments.Where((v, index) => index != 0));

            // first, try finding pipeline for event message:
            var pipelinesToEdit = _pipelineStore.Pipelines
                .Where(p => p.EventFilter is Filter filter && filter.PropertyName == "Command" && filter.FilterValue == commandName && p.EventType.FullName == typeof(TwitchCommandEvent).FullName)
                .ToList();

            if (pipelinesToEdit.Count != 0)
            {
                Module.TwitchClient.Value.SendMessage(Module.TwitchClient.Value.JoinedChannels[0], $"You tried to add a pipeline for command {commandName} but there is already a pipline registered for this command. Please use editcom if you want to edit this pipeline.");
                return;
            }

            _pipelineStore.Pipelines.Add(Pipeline.CreateForEvent<TwitchCommandEvent>(
                new Filter
                {
                    FilterValue = commandName,
                    IgnoreValueCasing = true,
                    Operator = FilterOperator.Equals,
                    PropertyName = "Command"
                },
                new List<IEventReactorConfiguration>
                {
                    new SendPredefinedTwitchMessageReactorConfiguration
                    {
                        Answer = response
                    }
                }));

            await _pipelineStore.SavePipelinesAsync(cancellationToken).ConfigureAwait(false);
            Module.TwitchClient.Value.SendMessage(Module.TwitchClient.Value.JoinedChannels[0], $"Successfully added command {commandName}.");
        }
    }
}
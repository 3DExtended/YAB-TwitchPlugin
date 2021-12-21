using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using TwitchBotPlugin.Events;
using TwitchBotPlugin.FilterExtensions.UserGroups;

using YAB.Core.EventReactor;
using YAB.Core.Filters;
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
                .Where(p => p.EventFilter is FilterExtension filter && filter.CustomFilterConfiguration is EventPropertyFilterConfiguration epfc && epfc.PropertyName == "Command" && epfc.FilterValue == commandName && p.EventType.FullName == typeof(TwitchCommandEvent).FullName)
                .ToList();

            if (pipelinesToEdit.Count != 0)
            {
                Module.TwitchClient.Value.SendMessage(Module.TwitchClient.Value.JoinedChannels[0], $"You tried to add a pipeline for command {commandName} but there is already a pipline registered for this command. Please use editcom if you want to edit this pipeline.");
                return;
            }

            var filter = new FilterGroup
            {
                Operator = LogicalOperator.And,
                Filters = new[] {
                    new FilterExtension
                    {
                        CustomFilterConfiguration = new EventPropertyFilterConfiguration
                        {
                            FilterValue = commandName,
                            IgnoreValueCasing = true,
                            Operator = FilterOperator.Equals,
                            PropertyName = "Command"
                        }
                    }
                }
            };

            if (response.Contains("#streameronly"))
            {
                filter.Filters = filter.Filters.Append(new FilterExtension
                {
                    CustomFilterConfiguration = new UserIsTwitchStreamerFilterConfiguration()
                }).ToList();

                response = response.Replace("#streameronly", string.Empty).Trim();
            }
            else if (response.Contains("#modonly"))
            {
                filter.Filters = filter.Filters.Append(new FilterExtension
                {
                    CustomFilterConfiguration = new UserIsTwitchModeratorFilterConfiguration()
                }).ToList();

                response = response.Replace("#modonly", string.Empty).Trim();
            }
            else if (response.Contains("#viponly"))
            {
                filter.Filters = filter.Filters.Append(new FilterExtension
                {
                    CustomFilterConfiguration = new UserIsTwitchVIPFilterConfiguration()
                }).ToList();

                response = response.Replace("#viponly", string.Empty).Trim();
            }

            _pipelineStore.Pipelines.Add(Pipeline.CreateForEvent<TwitchCommandEvent>(
                "Twitch Message Pipeline - !" + commandName,
                $"This pipeline lets the bot answer after someone entered \"{commandName}\" into chat.",
                filter,
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

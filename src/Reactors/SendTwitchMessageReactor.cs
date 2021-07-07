using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using YAB.Core.EventReactor;
using YAB.Core.Events;

namespace TwitchBotPlugin.Reactors
{
    public class SendTwitchMessageReactor :
        IEventReactor<SendTwitchMessageReactorConfiguration, CommandEventBase>,
        IEventReactor<SendTwitchMessageReactorConfiguration, UserMessageEventBase>
    {
        private readonly ILogger _logger;

        public SendTwitchMessageReactor(ILogger logger)
        {
            _logger = logger;
        }

        public Task RunAsync(SendTwitchMessageReactorConfiguration config, CommandEventBase evt, CancellationToken cancellationToken)
        {
            Module.TwitchClient.Value.SendMessage(Module.TwitchClient.Value.JoinedChannels[0], $"You send \"{evt.Command}\".");
            return Task.CompletedTask;
        }

        public Task RunAsync(SendTwitchMessageReactorConfiguration config, UserMessageEventBase evt, CancellationToken cancellationToken)
        {
            Module.TwitchClient.Value.SendMessage(Module.TwitchClient.Value.JoinedChannels[0], $"You send \"{evt.Message}\".");
            return Task.CompletedTask;
        }
    }
}
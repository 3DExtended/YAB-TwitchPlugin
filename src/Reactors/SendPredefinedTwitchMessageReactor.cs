using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using YAB.Core.EventReactor;
using YAB.Core.Events;

namespace TwitchBotPlugin.Reactors
{
    public class SendPredefinedTwitchMessageReactor :
        IEventReactor<SendPredefinedTwitchMessageReactorConfiguration, CommandEventBase>,
        IEventReactor<SendPredefinedTwitchMessageReactorConfiguration, UserMessageEventBase>
    {
        private readonly ILogger _logger;

        public SendPredefinedTwitchMessageReactor(ILogger logger)
        {
            _logger = logger;
        }

        public Task RunAsync(SendPredefinedTwitchMessageReactorConfiguration config, CommandEventBase evt, CancellationToken cancellationToken)
        {
            if (evt.Arguments.Count > 0 && evt.Arguments.First().StartsWith("@"))
            {
                Module.TwitchClient.Value.SendMessage(Module.TwitchClient.Value.JoinedChannels[0], $"{evt.Arguments.First()}: {config.Answer}");
            }
            else { 
                Module.TwitchClient.Value.SendMessage(Module.TwitchClient.Value.JoinedChannels[0], $"{config.Answer}");
            }
            return Task.CompletedTask;
        }

        public Task RunAsync(SendPredefinedTwitchMessageReactorConfiguration config, UserMessageEventBase evt, CancellationToken cancellationToken)
        {
            Module.TwitchClient.Value.SendMessage(Module.TwitchClient.Value.JoinedChannels[0], $"{config.Answer}");
            return Task.CompletedTask;
        }
    }
}
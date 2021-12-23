using System.Threading;
using System.Threading.Tasks;

using TwitchBotPlugin.Events;

using YAB.Core.FilterExtension;

namespace TwitchBotPlugin.FilterExtensions.UserGroups
{
    public class UserIsTwitchStreamerFilter : IFilterExtension<UserIsTwitchStreamerFilterConfiguration, TwitchUserEventBase>

    {
        public Task<bool> RunAsync(UserIsTwitchStreamerFilterConfiguration config, TwitchUserEventBase evt, CancellationToken cancellationToken)
        {
            var twitchClient = Module.TwitchClient.Value;

            return Task.FromResult(string.Equals(evt.User.DisplayName,
                twitchClient.JoinedChannels[0].Channel, System.StringComparison.OrdinalIgnoreCase));
        }
    }
}

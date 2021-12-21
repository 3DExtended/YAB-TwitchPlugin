using System.Threading;
using System.Threading.Tasks;

using YAB.Core.Events;
using YAB.Core.FilterExtension;

namespace TwitchBotPlugin.FilterExtensions.UserGroups
{
    public class UserIsTwitchStreamerFilter : IFilterExtension<UserIsTwitchStreamerFilterConfiguration, UserEventBase>

    {
        public Task<bool> RunAsync(UserIsTwitchStreamerFilterConfiguration config, UserEventBase evt, CancellationToken cancellationToken)
        {
            var twitchClient = Module.TwitchClient.Value;

            return Task.FromResult(string.Equals(evt.User.DisplayName,
                twitchClient.JoinedChannels[0].Channel, System.StringComparison.OrdinalIgnoreCase));
        }
    }
}

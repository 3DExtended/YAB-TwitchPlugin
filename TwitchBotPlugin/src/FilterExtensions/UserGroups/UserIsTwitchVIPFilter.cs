using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using TwitchBotPlugin.Events;

using TwitchLib.Client.Extensions;

using YAB.Core.FilterExtension;

namespace TwitchBotPlugin.FilterExtensions.UserGroups
{
    public class UserIsTwitchVIPFilter : IFilterExtension<UserIsTwitchVIPFilterConfiguration, TwitchUserEventBase>

    {
        public Task<bool> RunAsync(UserIsTwitchVIPFilterConfiguration config, TwitchUserEventBase evt, CancellationToken cancellationToken)
        {
            if (Module.TwitchVIPs.Any(m => string.Equals(m, evt.User.DisplayName, System.StringComparison.OrdinalIgnoreCase)))
            {
                return Task.FromResult(true);
            }

            var twitchClient = Module.TwitchClient.Value;
            twitchClient.GetVIPs(twitchClient.JoinedChannels[0]);

            return Task.FromResult(false);
        }
    }
}

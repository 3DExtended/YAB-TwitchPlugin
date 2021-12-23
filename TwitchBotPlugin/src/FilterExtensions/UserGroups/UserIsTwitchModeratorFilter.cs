using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using TwitchBotPlugin.Events;

using TwitchLib.Client.Extensions;

using YAB.Core.FilterExtension;

namespace TwitchBotPlugin.FilterExtensions.UserGroups
{
    public class UserIsTwitchModeratorFilter : IFilterExtension<UserIsTwitchModeratorFilterConfiguration, TwitchUserEventBase>

    {
        public Task<bool> RunAsync(UserIsTwitchModeratorFilterConfiguration config, TwitchUserEventBase evt, CancellationToken cancellationToken)
        {
            if (Module.TwitchModerators.Any(m => string.Equals(m, evt.User.DisplayName, System.StringComparison.OrdinalIgnoreCase)))
            {
                return Task.FromResult(true);
            }

            var twitchClient = Module.TwitchClient.Value;
            twitchClient.GetChannelModerators(twitchClient.JoinedChannels[0]);

            return Task.FromResult(false);
        }
    }
}

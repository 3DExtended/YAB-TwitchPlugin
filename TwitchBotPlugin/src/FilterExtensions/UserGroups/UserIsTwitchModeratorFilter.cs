using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using TwitchLib.Client.Extensions;

using YAB.Core.Events;
using YAB.Core.FilterExtension;

namespace TwitchBotPlugin.FilterExtensions.UserGroups
{
    public class UserIsTwitchModeratorFilter : IFilterExtension<UserIsTwitchModeratorFilterConfiguration, UserEventBase>

    {
        public Task<bool> RunAsync(UserIsTwitchModeratorFilterConfiguration config, UserEventBase evt, CancellationToken cancellationToken)
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

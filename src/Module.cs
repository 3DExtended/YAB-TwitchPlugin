using System;
using System.Linq;

using TwitchLib.Api;
using TwitchLib.Api.Interfaces;
using TwitchLib.Api.Services;
using TwitchLib.Client.Interfaces;

using YAB.Core.EventReactor;
using YAB.Core.Events;
using YAB.Plugins;

namespace TwitchBotPlugin
{
    public class Module : IPluginModule
    {
        public static Lazy<ITwitchAPI> TwitchAPI { get; set; } = new Lazy<ITwitchAPI>(() => new TwitchAPI());

        public static Lazy<ITwitchClient> TwitchClient { get; set; }

        public static Lazy<FollowerService> TwitchFollowerService { get; set; } = new Lazy<FollowerService>(() => new FollowerService(TwitchAPI.Value));

        public void RegisterBackgroundTasks(Action<Type> registerer)
        {
            var types = typeof(Module).Assembly.GetTypes().Where(t => typeof(IBackgroundTask).IsAssignableFrom(t));
            foreach (var type in types)
            {
                registerer(type);
            }
        }

        public void RegisterEventReactors(Action<Type> registerer)
        {
            var types = typeof(Module).Assembly.GetTypes().Where(t => typeof(IEventReactor).IsAssignableFrom(t));
            foreach (var type in types)
            {
                registerer(type);
            }
        }

        public void RegisterPluginEvents(Action<Type> registerer)
        {
            var types = typeof(Module).Assembly.GetTypes().Where(t => typeof(EventBase).IsAssignableFrom(t));
            foreach (var type in types)
            {
                registerer(type);
            }
        }
    }
}
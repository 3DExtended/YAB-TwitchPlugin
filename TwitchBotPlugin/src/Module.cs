﻿using System;
using System.Collections.Generic;
using System.Linq;

using TwitchBotPlugin.Options;

using TwitchLib.Api.Interfaces;
using TwitchLib.Api.Services;
using TwitchLib.Client.Interfaces;

using YAB.Core.EventReactor;
using YAB.Core.Events;
using YAB.Core.FilterExtension;
using YAB.Plugins;
using YAB.Plugins.Injectables.Options;

namespace TwitchBotPlugin
{
    public class Module : IPluginModule
    {
        public static YAB.Plugins.Injectables.Lazy<ITwitchAPI> TwitchAPI { get; set; }

        public static YAB.Plugins.Injectables.Lazy<ITwitchClient> TwitchClient { get; set; }

        public static YAB.Plugins.Injectables.Lazy<FollowerService> TwitchFollowerService { get; set; }

        public static List<string> TwitchModerators { get; internal set; } = new List<string>();

        public static List<string> TwitchVIPs { get; internal set; } = new List<string>();

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

        public void RegisterFilterExtensions(Action<Type> registerer)
        {
            var types = typeof(Module).Assembly.GetTypes().Where(t => typeof(IFilterExtension).IsAssignableFrom(t));
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

        public void RegisterPluginOptions(Action<IOptions> registerer)
        {
            registerer(new TwitchOptions());
        }
    }
}

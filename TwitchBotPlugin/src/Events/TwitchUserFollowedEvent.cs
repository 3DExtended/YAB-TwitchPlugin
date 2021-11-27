﻿using YAB.Core.Annotations;
using YAB.Core.Events;
using YAB.Plugins.Injectables.Options;

namespace TwitchBotPlugin.Events
{
    [ClassDescription("This event is raised for a twitch user, if they followed the joined channel.")]
    public class TwitchUserFollowedEvent : UserEventBase
    {
    }
}
﻿using YAB.Core.Annotations;
using YAB.Core.Events;
using YAB.Plugins.Injectables.Options;

namespace TwitchBotPlugin.Events
{
    [ClassDescription("This event is raised for a twitch user, if they resubscriped to the joined channel.")]
    public class TwitchReSubscribedEvent : UserEventBase
    {
        [PropertyDescription(false, "The number of month the user is already subed.")]
        public int Month { get; set; }
    }
}
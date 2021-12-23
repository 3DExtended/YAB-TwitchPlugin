using YAB.Core.Annotations;

namespace TwitchBotPlugin.Contracts
{
    [ClassDescription("Wrapper around information about a twitch user.")]
    public class TwitchUser
    {
        [PropertyDescription(false, "Normally the user name shown in chats and feeds.")]
        public string DisplayName { get; set; }

        [PropertyDescription(false, "Unique identifier for a given user.")]
        public string Id { get; set; }

        [PropertyDescription(false, "Is this user the broadcaster the bot is looking at. Might be null when information is not available.")]
        public bool? IsBroadcaster { get; set; }

        [PropertyDescription(false, "Is this user a moderator of the chat the bot is looking at. Might be null when information is not available.")]
        public bool? IsModerator { get; set; }

        [PropertyDescription(false, "Is this user a partner of twitch. Might be null when information is not available.")]
        public bool? IsPartner { get; set; }

        [PropertyDescription(false, "Is this user a twitch staff member. Might be null when information is not available.")]
        public bool? IsStaff { get; set; }

        [PropertyDescription(false, "Is this user a subscriber of the stream the bot is looking at. Might be null when information is not available.")]
        public bool? IsSubscriber { get; set; }

        [PropertyDescription(false, "Is this user a turbo member of twitch. Might be null when information is not available.")]
        public bool? IsTurbo { get; set; }

        [PropertyDescription(false, "Is this user a VIP of the stream the bot is looking at. Might be null when information is not available.")]
        public bool? IsVip { get; set; }
    }
}

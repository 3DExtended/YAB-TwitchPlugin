using YAB.Plugins.Injectables.Options;

namespace TwitchBotPlugin.src.Options
{
    public class TwitchOptions : Options<TwitchOptions>
    {
        /// <summary>
        /// Navigate to https://dev.twitch.tv/console and register a new application for your bot
        /// </summary>
        [OptionPropertyDescription(true, "This is the Client Id of your registered application in Twitch. Please visit  https://dev.twitch.tv/console if you are unsure.")]
        public string TwitchBotClientId { get; set; }

        /// <summary>
        /// Navigate to https://dev.twitch.tv/console and register a new application for your bot
        /// </summary>
        [OptionPropertyDescription(true, "This is the Client Secret of your registered application in Twitch. Please visit  https://dev.twitch.tv/console if you are unsure.")]
        public string TwitchBotSecret { get; set; }

        [OptionPropertyDescription(true, "This is the Token for you bot. Please visit https://twitchtokengenerator.com/ to generate this custom scope token.")]
        public string TwitchBotToken { get; set; }

        [OptionPropertyDescription(false, "The displayname of the bot.")]
        public string TwitchBotUsername { get; set; }

        [OptionPropertyDescription(false, "The twitch channel you want to observe (its display name).")]
        public string TwitchChannelToJoin { get; set; }
    }
}
namespace DevCore.TfsNotificationRelay.Slack
{
    using System.Collections.Generic;
    using System.Linq;
    using Configuration;

    public class SlackConfigurationFactory : ISlackConfigurationFactory
    {
        public SlackConfiguration GetConfiguration(BotElement slackBot)
        {
            var webhookUrl = slackBot.GetSetting("webhookUrl");
            var channels = GetChannels(slackBot.GetSetting("channels"));
            var username = slackBot.GetSetting("username");
            var iconEmoji = slackBot.GetSetting("iconEmoji");
            var iconUrl = slackBot.GetSetting("iconUrl");
            var standardColor = slackBot.GetSetting("standardColor");
            var successColor = slackBot.GetSetting("successColor");
            var errorColor = slackBot.GetSetting("errorColor");

            return new SlackConfiguration
            {
                WebhookUrl = webhookUrl,
                Channels = channels,
                Username = username,
                IconEmoji = iconEmoji,
                IconUrl = iconUrl,
                StandardColor = standardColor,
                SuccessColor = successColor,
                ErrorColor = errorColor,
                Bot = slackBot
            };
        }

        private IEnumerable<string> GetChannels(string channelsSetting)
        {
            return channelsSetting.Split(',').Select(channel => channel.Trim());
        }
    }
}
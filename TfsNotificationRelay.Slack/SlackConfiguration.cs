namespace DevCore.TfsNotificationRelay.Slack
{
    using System.Collections.Generic;
    using Configuration;

    public class SlackConfiguration
    {
        public IEnumerable<string> Channels { get; set; }

        public BotElement Bot { get; set; }
        public string Username { get; set; }
        public string IconUrl { get; set; }
        public string StandardColor { get; set; }
        public string SuccessColor { get; set; }
        public string ErrorColor { get; set; }
        public string WebhookUrl { get; set; }
        public string IconEmoji { get; set; }
    }
}
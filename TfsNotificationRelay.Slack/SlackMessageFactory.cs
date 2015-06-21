namespace DevCore.TfsNotificationRelay.Slack
{
    using System.Linq;
    using Configuration;
    using Notifications;

    public class SlackMessageFactory : ISlackMessageFactory
    {
        public Message GetMessage(INotification notification, SlackConfiguration slackConfiguration, string channel)
        {
            return ToSlackMessage((dynamic) notification, slackConfiguration.Bot, channel);
        }

        private Message ToSlackMessage(INotification notification, BotElement bot, string channel)
        {
            var lines = notification.ToMessage(bot, s => s);

            return SlackHelper.CreateSlackMessage(lines, bot, channel, bot.GetSetting("standardColor"));
        }

        private Message ToSlackMessage(IBuildCompletionNotification notification, BotElement bot, string channel)
        {
            var lines = notification.ToMessage(bot, s => s);
            var color = notification.IsSuccessful ? bot.GetSetting("successColor") : bot.GetSetting("errorColor");

            return SlackHelper.CreateSlackMessage(lines, bot, channel, color);
        }

        private Message ToSlackMessage(IWorkItemChangedNotification notification, BotElement bot, string channel)
        {
            string header = notification.ToMessage(bot, s => s).First();
            var fields = new[] { 
                new AttachmentField(bot.Text.State, notification.State, true), 
                new AttachmentField(bot.Text.AssignedTo, notification.AssignedTo, true) 
            };

            return SlackHelper.CreateSlackMessage(header, fields, bot, channel, bot.GetSetting("standardColor"));
        }
    }
}
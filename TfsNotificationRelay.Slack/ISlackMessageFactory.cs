namespace DevCore.TfsNotificationRelay.Slack
{
    using Notifications;

    public interface ISlackMessageFactory
    {
        Message GetMessage(INotification notification, SlackConfiguration slackConfiguration, string channel);
    }
}
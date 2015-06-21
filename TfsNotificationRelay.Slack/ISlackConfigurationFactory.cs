namespace DevCore.TfsNotificationRelay.Slack
{
    using Configuration;

    public interface ISlackConfigurationFactory
    {
        SlackConfiguration GetConfiguration(BotElement slackBot);
    }
}
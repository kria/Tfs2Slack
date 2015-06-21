namespace DevCore.TfsNotificationRelay.Slack
{
    using System.Net.Http;
    using System.Threading.Tasks;

    public interface ISlackClient
    {
        Task<HttpResponseMessage> SendMessageAsync(Message message, string webhookUrl);
    }
}
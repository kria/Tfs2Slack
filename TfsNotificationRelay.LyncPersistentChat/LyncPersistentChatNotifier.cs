using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevCore.TfsNotificationRelay;
using DevCore.TfsNotificationRelay.Configuration;
using DevCore.TfsNotificationRelay.Notifications;
using Microsoft.TeamFoundation.Framework.Server;
using TfsNotificationRelay.LyncPersistentChat;
using Microsoft.TeamFoundation.Build.Server;

namespace DevCore.TfsNotificationRelay.LyncPersistentChat
{
    public class LyncPersistentChatNotifier : INotifier
    {
        public async Task NotifyAsync(TeamFoundationRequestContext requestContext, INotification notification, BotElement bot, EventRuleElement matchingRule)
        {
            string room = bot.GetSetting("room");
            string user = bot.GetSetting("user");
            string password = bot.GetSetting("password");
            var client = new LyncPersistentChatClient();
            await client.ConnectAsync(user, password);
            await client.JoinRoomAsync(room);
            if (notification.GetType() == typeof(BuildNotification))
            {
                var typednotification = notification as BuildNotification;
                var message = new LyncPersistentChatMessage(false,"Build Status Notification");
                if (!typednotification.IsSuccessful)
                {
                    message.AppendEmoticon(LyncChatEmoticon.Crying);
                    message.AppendText(String.Format("Build {0} Status {1}", typednotification.DisplayName,typednotification.BuildStatus));
                    message.AppendText(String.Format("Shame on {0}", typednotification.UserName));
                    message.AppendHyperLink("Details", new Uri(typednotification.BuildUrl));
                } else
                {
                    message.AppendEmoticon(LyncChatEmoticon.Cool);
                    message.AppendText(String.Format("Build {0} Status {1}", typednotification.DisplayName, typednotification.BuildStatus));
                    message.AppendHyperLink("Details", new Uri(typednotification.BuildUrl));
                }
                await client.SendCustomMessageAsync(message);
            }
            else
            {
                var lines = notification.ToMessage(bot, s => s);
                string message = "";
                if (lines != null)
                    message = String.Join("\r\n", lines);
                await client.SendSimpleMessageAsync(message);
            }
            client.Dispose();
        }
    }
}

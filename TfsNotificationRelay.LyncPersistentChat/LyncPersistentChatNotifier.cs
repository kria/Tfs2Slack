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

namespace DevCore.TfsNotificationRelay.LyncPersistentChat
{
    public class LyncPersistentChatNotifier : INotifier
    {
        public async Task NotifyAsync(TeamFoundationRequestContext requestContext, INotification notification, BotElement bot, EventRuleElement matchingRule)
        {
            string room = bot.GetSetting("room");
            string user = bot.GetSetting("user");
            string password = bot.GetSetting("password");
            var lines = notification.ToMessage(bot, s => s);
            string message = "";
            if (lines != null)
                foreach (string line in lines)
                    message += String.Format("{0}\r\n", line);
            var client = new LyncPersistentChatClient();
            await client.ConnectAsync(user, password);
            await client.JoinRoomAsync(room);
            await client.SendSimpleMessageAsync(message);
            client.Dispose();
        }
    }
}

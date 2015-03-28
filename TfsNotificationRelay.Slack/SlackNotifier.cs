﻿/*
 * TfsNotificationRelay - http://github.com/kria/TfsNotificationRelay
 * 
 * Copyright (C) 2014 Kristian Adrup
 * 
 * This file is part of TfsNotificationRelay.
 * 
 * TfsNotificationRelay is free software: you can redistribute it and/or 
 * modify it under the terms of the GNU General Public License as published 
 * by the Free Software Foundation, either version 3 of the License, or 
 * (at your option) any later version. See included file COPYING for details.
 */

using DevCore.TfsNotificationRelay.Notifications;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevCore.TfsNotificationRelay.Configuration;
using Microsoft.TeamFoundation.Framework.Server;

namespace DevCore.TfsNotificationRelay.Slack
{
    public class SlackNotifier : INotifier
    {
        public async Task NotifyAsync(TeamFoundationRequestContext requestContext, INotification notification, BotElement bot)
        {
            var channels = bot.GetSetting("channels").Split(',').Select(chan => chan.Trim());
            var tasks = new List<Task>();
            var slackClient = new SlackClient();

            foreach (string channel in channels)
            {
                Message slackMessage = ToSlackMessage((dynamic)notification, bot, channel);
                if (slackMessage != null)
                {
                    tasks.Add(slackClient.SendMessageAsync(slackMessage, bot.GetSetting("webhookUrl")).ContinueWith(t => t.Result.EnsureSuccessStatusCode()));
                }
            }

            await Task.WhenAll(tasks);
        }

        public Message ToSlackMessage(INotification notification, BotElement bot, string channel)
        {
            var lines = notification.ToMessage(bot, s => s);

            return SlackHelper.CreateSlackMessage(lines, bot, channel, bot.GetSetting("standardColor"));
        }

        public Message ToSlackMessage(BuildCompletionNotification notification, BotElement bot, string channel)
        {
            var lines = notification.ToMessage(bot, s => s);
            var color = notification.IsSuccessful ? bot.GetSetting("successColor") : bot.GetSetting("errorColor");

            return SlackHelper.CreateSlackMessage(lines, bot, channel, color);
        }

        public Message ToSlackMessage(WorkItemChangedNotification notification, BotElement bot, string channel)
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

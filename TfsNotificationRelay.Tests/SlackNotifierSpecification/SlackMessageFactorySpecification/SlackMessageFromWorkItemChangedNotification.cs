namespace TfsNotificationRelay.Tests.SlackNotifierSpecification.SlackMessageFactorySpecification
{
    using System;
    using System.Collections.Generic;
    using DevCore.TfsNotificationRelay.Configuration;
    using DevCore.TfsNotificationRelay.Notifications;
    using FakeItEasy;
    using NUnit.Framework;

    [TestFixture]
    class SlackMessageFromWorkItemChangedNotification : SlackMessageElementsForEveryNotification<IWorkItemChangedNotification>
    {
        protected override void ReturnFromNotification(params string[] notificationLines)
        {
            A.CallTo(() => Notification.ToMessage(A<BotElement>.Ignored, A<Func<string, string>>.Ignored))
                .Returns(new List<string>(notificationLines));
        }
    }
}

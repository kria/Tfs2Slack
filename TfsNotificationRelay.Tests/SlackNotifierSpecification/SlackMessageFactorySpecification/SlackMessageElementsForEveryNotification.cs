namespace TfsNotificationRelay.Tests.SlackNotifierSpecification.SlackMessageFactorySpecification
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DevCore.TfsNotificationRelay.Configuration;
    using DevCore.TfsNotificationRelay.Notifications;
    using DevCore.TfsNotificationRelay.Slack;
    using FakeItEasy;
    using NUnit.Framework;

    abstract class SlackMessageElementsForEveryNotification<T> where T : INotification
    {
        protected SlackConfiguration SlackConfiguration;
        protected T Notification;
        protected SlackMessageFactory SlackMessageFactory;

        [SetUp]
        public void Setup()
        {
            SlackMessageFactory = new SlackMessageFactory();
            var slackBot =
                TestConfigurationHelper.LoadSlackBot(@"SlackNotifierSpecification\slackNotifierTestConfig.config");
            SlackConfiguration = new SlackConfigurationFactory().GetConfiguration(slackBot);

            Notification = A.Fake<T>();
            ReturnFromNotification("some", "lines");
        }

        protected virtual void ReturnFromNotification(params string[] notificationLines)
        {
            A.CallTo(() => Notification.ToMessage(A<BotElement>.Ignored, A<Func<string, string>>.Ignored))
                .Returns(new List<string>(notificationLines));
        }

        [Test]
        public void ShouldAddUserNameToMessage()
        {
            //when
            var message = SlackMessageFactory.GetMessage(Notification, SlackConfiguration, "channel");

            //then
            Assert.That(message.Username, Is.EqualTo(SlackConfiguration.Username));
        }

        [Test]
        public void ShouldTreatFirstLineOfTheNotificationAsFallbackAndPretextForAttachments()
        {
            //given
            ReturnFromNotification("fallback", "other", "lines");
            const string expectedFallback = "fallback";

            //when
            var message = SlackMessageFactory.GetMessage(Notification, SlackConfiguration, "channel");

            //then
            var actualFallback = message.Attachments.First().Fallback;
            var actualPretext = message.Attachments.First().Pretext;

            Assert.That(actualFallback, Is.EqualTo(expectedFallback));
            Assert.That(actualPretext, Is.EqualTo(expectedFallback));
        }

        [Test]
        public void ShouldFillMessageChannel()
        {
            //given
            const string expectedChannel = "expectedChannel";

            //when
            var message = SlackMessageFactory.GetMessage(Notification, SlackConfiguration, expectedChannel);

            //then
            Assert.That(message.Channel, Is.EqualTo(expectedChannel));
        }
    }
}

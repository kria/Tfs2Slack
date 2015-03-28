﻿namespace TfsNotificationRelay.Tests.SlackNotifier
{
    using DevCore.TfsNotificationRelay.Slack;
    using NUnit.Framework;

    [TestFixture]
    class SlackNotifierConfigurationSpec
    {
        [Test]
        public void ShouldRecogniseLegacyNotificationSchemeWhenChannelsSettingIsNotEmpty()
        {
            //given
            var slackBot = TestConfigurationHelper.LoadSlackBot(@"SlackNotifier\legacyNotification.config");

            //when
            var config = new SlackConfiguration(slackBot);

            //then
            Assert.That(config.AllNotificationsShouldGoToAllChannels, Is.True);
        }

        [Test]
        public void ShouldRetrieveChannelListFromConfiguration()
        {
            //given
            var expectedChannels = new[]
            {
                "#general",
                "#b"
            };
            var slackBot = TestConfigurationHelper.LoadSlackBot(@"SlackNotifier\legacyNotification.config");
            var config = new SlackConfiguration(slackBot);

            //when
            var actualChannels = config.Channels;

            //then
            Assert.That(actualChannels, Is.EquivalentTo(expectedChannels));
        }
    }
}

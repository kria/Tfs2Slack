namespace TfsNotificationRelay.Tests.SlackNotifierSpecification
{
    using DevCore.TfsNotificationRelay.Configuration;
    using DevCore.TfsNotificationRelay.Slack;
    using FakeItEasy;
    using FakeItEasy.ExtensionSyntax.Full;
    using NUnit.Framework;

    [TestFixture]
    class SlackNotifierConfigurationFactorySpecification
    {
        private SlackConfiguration LoadTestConfig()
        {
            var factory = new SlackConfigurationFactory();
            var slackBot = TestConfigurationHelper.LoadSlackBot(@"SlackNotifierSpecification\slackNotifierTestConfig.config");
            return factory.GetConfiguration(slackBot);
        }

        [Test]
        public void ShouldRetrieveChannelListFromConfiguration()
        {
            //given
            var expected = new[]
            {
                "#general",
                "#b"
            };

            //when
            var config = LoadTestConfig();

            //then
            Assert.That(config.Channels, Is.EquivalentTo(expected));
        }

        [Test]
        public void ShouldStoreBotElementThatWasUsedToInitializeIt()
        {
            //given
            var slackBot = TestConfigurationHelper.LoadSlackBot(@"SlackNotifierSpecification\slackNotifierTestConfig.config");

            //when
            var config = new SlackConfigurationFactory().GetConfiguration(slackBot);

            //then
            Assert.That(config.Bot, Is.EqualTo(slackBot));
        }

        [Test]
        public void ShouldReadUsername()
        {
            //given
            const string expected = "tfsbot";

            //when
            var config = LoadTestConfig();

            //then
            Assert.That(config.Username, Is.EqualTo(expected));
        }

        [Test]
        public void ShouldReadIconUrl()
        {
            //given
            const string expected = "https://raw.githubusercontent.com/kria/TfsNotificationRelay/master/tfsicon.png";

            //when
            var config = LoadTestConfig();

            //then
            Assert.That(config.IconUrl, Is.EqualTo(expected));
        }

        [Test]
        public void ShouldReadColors()
        {
            //given
            const string expectedStandardColor = "#68217a";
            const string expectedSuccessColor = "#1cb841";
            const string expectedErrorColor = "#ca3c3c";

            //when
            var config = LoadTestConfig();

            //then
            Assert.That(config.StandardColor, Is.EqualTo(expectedStandardColor));
            Assert.That(config.SuccessColor, Is.EqualTo(expectedSuccessColor));
            Assert.That(config.ErrorColor, Is.EqualTo(expectedErrorColor));
        }

        [Test]
        public void ShouldReadWebhookUrl()
        {
            //given
            const string expected = "someTestWebhookUrl";

            //when
            var config = LoadTestConfig();

            //then
            Assert.That(config.WebhookUrl, Is.EqualTo(expected));
        }

        [Test]
        public void ShouldReadIconEmoji()
        {
            //given
            const string expected = "testEmoji";

            //when
            var config = LoadTestConfig();

            //then
            Assert.That(config.IconEmoji, Is.EqualTo(expected));
        }
    }
}

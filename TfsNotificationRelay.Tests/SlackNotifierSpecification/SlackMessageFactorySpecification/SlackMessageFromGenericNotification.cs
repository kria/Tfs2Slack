namespace TfsNotificationRelay.Tests.SlackNotifierSpecification.SlackMessageFactorySpecification
{
    using System.Linq;
    using DevCore.TfsNotificationRelay.Notifications;
    using NUnit.Framework;

    [TestFixture]
    class SlackMessageFromGenericNotification : SlackMessageElementsForEveryNotification<INotification>
    {
        [Test]
        public void ShouldUseStandardColorForGenericNotifications()
        {
            //when
            var message = SlackMessageFactory.GetMessage(Notification, SlackConfiguration, "anyChannel");

            //then
            Assert.That(message.Attachments.All(attachment => attachment.Color == SlackConfiguration.StandardColor));
        }

        [Test]
        public void ShouldAddNotificationLinesAsShortAttachmentFields()
        {
            //given
            ReturnFromNotification("some", "notification", "lines");
            var expectedFieldValues = new[] {"notification", "lines"};

            //when
            var message = SlackMessageFactory.GetMessage(Notification, SlackConfiguration, "channel");
            
            //then
            var attachmentFields = message.Attachments.First().Fields;
            var actualFieldValues = attachmentFields.Select(field => field.Value);

            Assert.That(actualFieldValues, Is.EquivalentTo(expectedFieldValues));
            Assert.That(!attachmentFields.Any(field => field.IsShort));
        }
    }
}
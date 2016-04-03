using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TfsNotificationRelay.LyncPersistentChat;
using System.Threading.Tasks;

namespace TfsNotificationRelay.Lync.Tests
{
    [TestClass]
    public class ChatClientTests
    {
        [TestMethod]
        public void Lync_Construction_ShouldSucceed()
        {
            var testObject = new LyncPersistentChatClient();
            Assert.IsNotNull(testObject, "Constructor return null");
        }

        [TestMethod]
        public void Lync_Connect_ShouldSucceed()
        {
            var testObject = new LyncPersistentChatClient();
            testObject.Connect(Properties.Settings.Default.username, Properties.Settings.Default.password);
        }

        [TestMethod]
        public async Task Lync_ConnectAsync_ShouldSucceed()
        {
            var testObject = new LyncPersistentChatClient();
            await testObject.ConnectAsync(Properties.Settings.Default.username, Properties.Settings.Default.password);
        }

        [TestMethod]
        public void Lync_JoinRoom_ShouldSucceed()
        {
            var testObject = new LyncPersistentChatClient();
            testObject.Connect(Properties.Settings.Default.username, Properties.Settings.Default.password);
            Assert.IsTrue(testObject.JoinRoom(Properties.Settings.Default.room), "Join Room failed");
        }

        [TestMethod]
        public async Task Lync_JoinRoomAsync_ShouldSucceed()
        {
            var testObject = new LyncPersistentChatClient();
            await testObject.ConnectAsync(Properties.Settings.Default.username, Properties.Settings.Default.password);
            Assert.IsTrue(await testObject.JoinRoomAsync(Properties.Settings.Default.room), "Join Room failed");
        }

        [TestMethod]
        public void Lync_SendSimpleMessage_ShouldSucceed()
        {
            var testObject = new LyncPersistentChatClient();
            testObject.Connect(Properties.Settings.Default.username, Properties.Settings.Default.password);
            Assert.IsTrue(testObject.JoinRoom(Properties.Settings.Default.room), "Join Room failed");
            testObject.SendSimpleMessage("Einfache Test-Nachricht vom Unit-Test.");
        }

        [TestMethod]
        public void Lync_SendStoryMessage_ShouldSucceed()
        {
            var testObject = new LyncPersistentChatClient();
            testObject.Connect(Properties.Settings.Default.username, Properties.Settings.Default.password);
            Assert.IsTrue(testObject.JoinRoom(Properties.Settings.Default.room), "Join Room failed");
            testObject.SendStoryMessage("Story-Message Title","Einfache Test-Nachricht vom Unit-Test.",true);
        }
    }
}

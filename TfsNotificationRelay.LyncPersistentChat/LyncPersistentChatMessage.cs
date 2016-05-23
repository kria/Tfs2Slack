using Microsoft.Rtc.Collaboration.PersistentChat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevCore.TfsNotificationRelay.LyncPersistentChat
{
    public enum LyncChatEmoticon
    {
        Smile = 0,
        Frown = 1,
        Cool = 2,
        Laughing = 3,
        Schocked = 4,
        Confused = 5,
        Crying = 6,
        Tounge = 7,
        Wink = 8,
        Angry = 9,
        Kissing = 10
    };

    public class LyncPersistentChatMessage
    {
        private FormattedOutboundChatMessage _message;
        public bool IsAlert {
            get { return _message.IsAlert; }
        }

        public LyncPersistentChatMessage()
        {
            _message = new FormattedOutboundChatMessage();
        }

        public LyncPersistentChatMessage(bool alert)
        {
            _message = new FormattedOutboundChatMessage(alert);
        }

        public LyncPersistentChatMessage(bool alert, string title)
        {
            _message = new FormattedOutboundChatMessage(alert, title);
        }

        public void AppendEmoticon(LyncChatEmoticon emoticon)
        {
            _message.AppendEmoticon((ChatEmoticon)emoticon);
        }

        public void AppendText(string message)
        {
            _message.AppendPlainText(message);
        }

        public void AppendHyperLink(string displaytext, Uri hyperlink)
        {
            _message.AppendHyperLink(displaytext, hyperlink);
        }

        internal FormattedOutboundChatMessage getLyncMessage()
        {
            return _message;
        }
    }
}

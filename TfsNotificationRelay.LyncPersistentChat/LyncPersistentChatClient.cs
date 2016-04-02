using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Rtc.Collaboration;
using Microsoft.Rtc.Collaboration.PersistentChat;
using Microsoft.Rtc.Collaboration.PersistentChat.Management;
using Microsoft.Rtc.Collaboration.Presence;
using Microsoft.Rtc.Signaling;
using System.Collections.ObjectModel;

namespace TfsNotificationRelay.LyncPersistentChat
{
    public class LyncPersistentChatClient
    {
        private Uri _persistentChatServerUri;
        private PersistentChatEndpoint _persistentChatEndpoint;
        private UserEndpoint _userEndpoint;
        private CollaborationPlatform _collaborationPlatform;
        private ProvisioningData _provisioningData;
        private ChatRoomSnapshot _room;
        private ChatRoomSession _session;

        public LyncPersistentChatClient()
        {
            ClientPlatformSettings platformSettings = new ClientPlatformSettings("PersistentChat.TfsNotificationRelay", SipTransportType.Tls);
            platformSettings.DefaultAudioVideoProviderEnabled = false;
            _collaborationPlatform = new CollaborationPlatform(platformSettings);
            _collaborationPlatform.AllowedAuthenticationProtocol = SipAuthenticationProtocols.Ntlm;
            _collaborationPlatform.EndStartup(_collaborationPlatform.BeginStartup(null, null));
        }

        public void Connect(string username, string password)
        {
            if (_userEndpoint == null)
            {
                UserEndpointSettings userEndpointSettings = new UserEndpointSettings("sip:" + username);
                userEndpointSettings.Credential = new System.Net.NetworkCredential(username, password);
                _userEndpoint = new UserEndpoint(_collaborationPlatform, userEndpointSettings);
                _userEndpoint.EndEstablish(_userEndpoint.BeginEstablish(null, null));
            }
            
            if (_provisioningData == null)
                _provisioningData = _userEndpoint.EndGetProvisioningData(_userEndpoint.BeginGetProvisioningData(null, null));

            if (_persistentChatServerUri == null)
            {
                _persistentChatServerUri = new Uri(_provisioningData.PersistentChatConfiguration.DefaultPersistentChatUri);
                _persistentChatEndpoint = new PersistentChatEndpoint(_persistentChatServerUri, _userEndpoint);
                _persistentChatEndpoint.EndEstablish(_persistentChatEndpoint.BeginEstablish(null, null));
            }
        }
        public bool JoinRoom(string roomname)
        {
            IAsyncResult asyncResult = _persistentChatEndpoint.PersistentChatServices.BeginBrowseChatRoomsByCriteria(roomname, false, true, true, null, null);
            ReadOnlyCollection<ChatRoomSnapshot> rooms = _persistentChatEndpoint.PersistentChatServices.EndBrowseChatRoomsByCriteria(asyncResult);
            if (rooms.Count == 0 || rooms.Count > 1)
                return false;
            _room = rooms.First();
            if (_session != null)
                _session.EndLeave(_session.BeginLeave(null, null));
            else
                _session = new ChatRoomSession(_persistentChatEndpoint);
            try
            {
                _session.EndJoin(_session.BeginJoin(_room, null, null));
            }
            catch (CommandFailedException e)
            {
                if (e.Message.Contains("to perform"))
                    return false;
                else
                    throw;
            }
            return true;
        }

        private bool IsSessionEstablished()
        {
            if (_session == null)
                return false;
            if (_session.State != ChatRoomSessionState.Established && _session.State != ChatRoomSessionState.Idle)
                return false;
            return true;
        }

        public void SendSimpleMessage(string message)
        {
            if (IsSessionEstablished())
                _session.EndSendChatMessage(_session.BeginSendChatMessage(message, null, null));
        }

        public void SendStoryMessage(string title, string message, bool alert)
        {
            if (IsSessionEstablished())
            {
                FormattedOutboundChatMessage chatSimpleStory = new FormattedOutboundChatMessage(alert, title);
                chatSimpleStory.AppendPlainText(message);
                _session.EndSendChatMessage(_session.BeginSendChatMessage(chatSimpleStory, null, null));
            }
        }

    }
}

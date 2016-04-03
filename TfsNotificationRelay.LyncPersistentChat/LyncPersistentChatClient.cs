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
using DevCore.TfsNotificationRelay.LyncPersistentChat;

namespace TfsNotificationRelay.LyncPersistentChat
{
    public class LyncPersistentChatClient : IDisposable
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
            Task t = Task.Factory.FromAsync(_collaborationPlatform.BeginStartup(null, null), _collaborationPlatform.EndStartup);
            t.Wait();
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
            } else
            {
                if (_persistentChatEndpoint.State == PersistentChatEndpointState.Terminated)
                    _persistentChatEndpoint.EndEstablish(_persistentChatEndpoint.BeginEstablish(null, null));
            }
        }

        public async Task ConnectAsync(string username, string password)
        {
            if (_userEndpoint == null)
            {
                UserEndpointSettings userEndpointSettings = new UserEndpointSettings("sip:" + username);
                userEndpointSettings.Credential = new System.Net.NetworkCredential(username, password);
                _userEndpoint = new UserEndpoint(_collaborationPlatform, userEndpointSettings);
                await Task<SipResponseData>.Factory.FromAsync(_userEndpoint.BeginEstablish(null, null), _userEndpoint.EndEstablish);
            }
            if (_provisioningData == null)
            {
                var task = Task<ProvisioningData>.Factory.FromAsync(_userEndpoint.BeginGetProvisioningData(null, null), _userEndpoint.EndGetProvisioningData);
                _provisioningData = await task;
            }

            if (_persistentChatServerUri == null)
            {
                _persistentChatServerUri = new Uri(_provisioningData.PersistentChatConfiguration.DefaultPersistentChatUri);
                _persistentChatEndpoint = new PersistentChatEndpoint(_persistentChatServerUri, _userEndpoint);
                await Task.Factory.FromAsync(_persistentChatEndpoint.BeginEstablish(null, null), _persistentChatEndpoint.EndEstablish);
            } else
            {
                if (_persistentChatEndpoint.State == PersistentChatEndpointState.Terminated)
                    await Task.Factory.FromAsync(_persistentChatEndpoint.BeginEstablish(null, null), _persistentChatEndpoint.EndEstablish);
            }
        }

        public bool JoinRoom(string roomname)
        {
            IAsyncResult asyncResult = _persistentChatEndpoint.PersistentChatServices.BeginBrowseChatRoomsByCriteria(roomname, false, true, true, null, null);
            ReadOnlyCollection<ChatRoomSnapshot> rooms = _persistentChatEndpoint.PersistentChatServices.EndBrowseChatRoomsByCriteria(asyncResult);
            if (!rooms.Count.Equals(1))
                return false;
            _room = rooms[0];
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

        public async Task<bool> JoinRoomAsync(string roomname)
        {
            IAsyncResult asyncResult = _persistentChatEndpoint.PersistentChatServices.BeginBrowseChatRoomsByCriteria(roomname, false, true, true, null, null);
            Task<ReadOnlyCollection<ChatRoomSnapshot>> task = Task<ReadOnlyCollection<ChatRoomSnapshot>>.Factory.FromAsync(asyncResult, _persistentChatEndpoint.PersistentChatServices.EndBrowseChatRoomsByCriteria);
            ReadOnlyCollection<ChatRoomSnapshot> rooms = await task;
            if (!rooms.Count.Equals(1))
                return false;
            _room = rooms[0];
            if (_session != null)
                await Task.Factory.FromAsync(_session.BeginLeave(null, null), _session.EndLeave);
            else
                _session = new ChatRoomSession(_persistentChatEndpoint);
            try
            {
                await Task.Factory.FromAsync(_session.BeginJoin(_room, null, null), _session.EndJoin);
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

        public bool IsUserEndpointEstablished()
        {
            if (_userEndpoint == null)
                return false;
            if (_userEndpoint.State != LocalEndpointState.Established && _userEndpoint.State != LocalEndpointState.Idle)
                return false;
            return true;
        }

        public bool IsPersistentChatEnpointEstablished()
        {
            if (_persistentChatEndpoint == null)
                return false;
            if (_persistentChatEndpoint.State != PersistentChatEndpointState.Established && _persistentChatEndpoint.State != PersistentChatEndpointState.Idle)
                return false;
            return true;
        }

        public bool IsSessionEstablished()
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

        public async Task SendSimpleMessageAsync(string message)
        {
            if (IsSessionEstablished())
                await Task.Factory.FromAsync(_session.BeginSendChatMessage(message, null, null), _session.EndSendChatMessage);
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

        public async Task SendStoryMessageAsync(string title, string message, bool alert)
        {
            if (IsSessionEstablished())
            {
                FormattedOutboundChatMessage chatSimpleStory = new FormattedOutboundChatMessage(alert, title);
                chatSimpleStory.AppendPlainText(message);
                await Task.Factory.FromAsync(_session.BeginSendChatMessage(chatSimpleStory, null, null), _session.EndSendChatMessage);
            }
        }

        public void SendCustomMessage(LyncPersistentChatMessage message)
        {
            if (IsSessionEstablished())
            {
                _session.EndSendChatMessage(_session.BeginSendChatMessage(message.getLyncMessage(), null, null));
            }
        }

        public async Task SendCustomMessageAsync(LyncPersistentChatMessage message)
        {
            if (IsSessionEstablished())
            {
                await Task.Factory.FromAsync(_session.BeginSendChatMessage(message.getLyncMessage(), null, null), _session.EndSendChatMessage);
            }
        }

        public void Dispose()
        {
            if (IsSessionEstablished())
            {
                var tEndSession = Task.Factory.FromAsync(_session.BeginLeave(null, null), _session.EndLeave);
                tEndSession.Wait();
            }
            _session = null;
            _room = null;
            if (IsPersistentChatEnpointEstablished())
            {
                var tEndChatEndpoint = Task.Factory.FromAsync(_persistentChatEndpoint.BeginTerminate(null, null), _persistentChatEndpoint.EndTerminate);
                tEndChatEndpoint.Wait();
            }
            _persistentChatEndpoint = null;
            _persistentChatServerUri = null;
            if (IsUserEndpointEstablished())
            {
                var tEndUserEndpoint = Task.Factory.FromAsync(_userEndpoint.BeginTerminate(null, null), _userEndpoint.EndTerminate);
                tEndUserEndpoint.Wait();
            }
            _userEndpoint = null;
            _provisioningData = null;
            var tEndPlatform = Task.Factory.FromAsync(_collaborationPlatform.BeginShutdown(null, null), _collaborationPlatform.EndShutdown);
            tEndPlatform.Wait();
            _collaborationPlatform = null;

        }
    }
}

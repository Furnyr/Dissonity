using System;
using UnityEngine;
using Dissonity.Models.Mock;
using System.Collections.Generic;
using Dissonity.Models;
using Dissonity.Events;

namespace Dissonity
{
    /// <summary>
    /// <b> Warning: </b> This script is self-destroyed outside of the Unity Editor
    /// </summary>
    public class DiscordMock : MonoBehaviour
    {
        #nullable enable annotations

        // Query
        public MockQueryData _query = new();

        // Activity
        public MockLocale _locale = MockLocale.en_US;
        public string _accessToken = "mock-access-token";
        public MockPlayer _currentPlayer = new();
        public List<MockPlayer> _otherPlayers = new();
        public List<MockChannel> _channels = new();

        // General events
        public LayoutModeType _layoutMode = LayoutModeType.Focused;
        public OrientationType _screenOrientation = OrientationType.Landscape;
        public ThermalStateType _thermalState = ThermalStateType.Nominal;

        // In-App Purchases
        public List<MockEntitlement> _entitlements = new();
        public List<MockSku> _skus = new();

        // Singleton
        public static DiscordMock Singleton { get; private set; }

        //# RUNTIME - - - - -
        void Awake()
        {
            if (!Api.isEditor)
            {
                Destroy(gameObject);
                return;
            }

            if (Singleton != null && Singleton != this) 
            {
                Destroy(gameObject); 
            }
            else 
            {
                Singleton = this; 
            }

            DontDestroyOnLoad(gameObject);
        }

        internal Participant[] GetParticipants()
        {
            List<Participant> participants = new();

            participants.Add(_currentPlayer.Participant.ToParticipant());

            // Add each player participant
            foreach (MockPlayer player in _otherPlayers)
            {
                participants.Add(player.Participant.ToParticipant());
            }

            return participants.ToArray();
        }

        internal Sku[] GetSkus()
        {
            List<Sku> skuList = new();

            foreach (var dict in _skus)
            {
                skuList.Add(dict.ToSku());
            }

            return skuList.ToArray();
        }

        internal Entitlement[] GetEntitlements()
        {
            List<Entitlement> entitlementList = new();

            foreach (var ent in _entitlements)
            {
                entitlementList.Add(ent.ToEntitlement());
            }

            return entitlementList.ToArray();
        }

        internal UserVoiceState[] GetUserVoiceStates()
        {
            List<UserVoiceState> userVoiceStates = new();

            var myVoiceState = _currentPlayer.UserVoiceState.ToUserVoiceState();
            myVoiceState.Nickname = _currentPlayer.Participant.Nickname;
            myVoiceState.User = _currentPlayer.Participant.ToUser();

            userVoiceStates.Add(myVoiceState);

            // Add each player voice state
            foreach (MockPlayer player in _otherPlayers)
            {
                var voiceState = player.UserVoiceState.ToUserVoiceState();
                voiceState.Nickname = player.Participant.Nickname;
                voiceState.User = player.Participant.ToUser();

                userVoiceStates.Add(voiceState);
            }

            return userVoiceStates.ToArray();
        }

        internal GuildMemberRpc GetGuildMemberRpc()
        {
            GuildMemberRpc data = _currentPlayer.GuildMemberRpc.ToGuildMemberRpc();

            data.Nickname = _currentPlayer.Participant.Nickname;
            data.UserId = _currentPlayer.Participant.Id;

            return data;
        }

        /// <summary>
        /// Dispatch a mock Activity Instance Participants Update.
        /// </summary>
        public void ActivityInstanceParticipantsUpdate()
        {
            ActivityInstanceParticipantsUpdateData data = new()
            {
                Participants = GetParticipants()
            };

            Api.bridge!.MockDiscordDispatch(DiscordEventType.ActivityInstanceParticipantsUpdate, data);
        }


        /// <summary>
        /// Dispatch a mock Activity Layout Mode Update.
        /// </summary>
        public void ActivityLayoutModeUpdate()
        {
            ActivityLayoutModeUpdateData data = new()
            {
                LayoutMode = _layoutMode
            };

            Api.bridge!.MockDiscordDispatch(DiscordEventType.ActivityLayoutModeUpdate, data);
        }


        /// <summary>
        /// Dispatch a mock Orientation Update.
        /// </summary>
        public void OrientationUpdate()
        {
            OrientationUpdateData data = new()
            {
                ScreenOrientation = _screenOrientation
            };

            Api.bridge!.MockDiscordDispatch(DiscordEventType.OrientationUpdate, data);
        }


        /// <summary>
        /// Dispatch a mock Thermal State Update.
        /// </summary>
        public void ThermalStateUpdate()
        {
            ThermalStateUpdateData data = new()
            {
                ThermalState = _thermalState
            };

            Api.bridge!.MockDiscordDispatch(DiscordEventType.ThermalStateUpdate, data);
        }


        /// <summary>
        /// Dispatch a mock Current Guild Member Update.
        /// </summary>
        public void CurrentGuildMemberUpdate()
        {
            GuildMemberRpc data = _currentPlayer.GuildMemberRpc.ToGuildMemberRpc();

            data.Nickname = _currentPlayer.Participant.Nickname;
            data.UserId = _currentPlayer.Participant.Id;

            Api.bridge!.MockDiscordDispatch(DiscordEventType.CurrentGuildMemberUpdate, data); 
        }


        /// <summary>
        /// Dispatch a mock Current User Update.
        /// </summary>
        public void CurrentUserUpdate()
        {
            User data = _currentPlayer.Participant.ToUser();

            Api.bridge!.MockDiscordDispatch(DiscordEventType.CurrentUserUpdate, data);
        }


        /// <summary>
        /// Dispatch a mock Voice State Update. <br/> <br/>
        /// <c> playerIndex </c> -1 for current player.
        /// </summary>
        public void VoiceStateUpdate(int playerIndex = -1)
        {
            //\ Dispatch
            UserVoiceState data;

            //? Current player
            if (playerIndex == -1) data = _currentPlayer.UserVoiceState;
            else data = _otherPlayers[playerIndex].UserVoiceState;

            Api.bridge!.MockDiscordDispatch(DiscordEventType.VoiceStateUpdate, data);
        }


        /// <summary>
        /// Dispatch a mock Speaking Start. <br/> <br/>
        /// <c> playerIndex </c> -1 for current player.
        /// </summary>
        public void SpeakingStart(int playerIndex = -1)
        {
            //\ Dispatch
            long userId;

            //? Current player
            if (playerIndex == -1) userId = _currentPlayer.Participant.Id;
            else userId = _otherPlayers[playerIndex].Participant.Id;

            SpeakingData data = new()
            {
                ChannelId = _query.ChannelId,
                UserId = userId
            };
            
            Api.bridge!.MockDiscordDispatch(DiscordEventType.SpeakingStart, data);
        }


        /// <summary>
        /// Dispatch a mock Speaking Stop. <br/> <br/>
        /// <c> playerIndex </c> -1 for current player.
        /// </summary>
        public void SpeakingStop(int playerIndex = -1)
        {
            //\ Dispatch
            long userId;

            //? Current player
            if (playerIndex == -1) userId = _currentPlayer.Participant.Id;
            else userId = _otherPlayers[playerIndex].Participant.Id;

            SpeakingData data = new()
            {
                ChannelId = _query.ChannelId,
                UserId = userId
            };
            
            Api.bridge!.MockDiscordDispatch(DiscordEventType.SpeakingStop, data);
        }
    

        /// <summary>
        /// Dispatch a mock Entitlement Create.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public void EntitlementCreate(long mockEntitlementId)
        {
            MockEntitlement? mockEnt = _entitlements.Find(e => e.Id == mockEntitlementId);

            if (mockEnt == null) throw new ArgumentException("You must pass a valid mock entitlement id");

            var ent = mockEnt.ToEntitlement();
            ent.UserId = _currentPlayer.Participant.Id;

            EntitlementCreateData data = new()
            {
                Entitlement = ent
            };

            Api.bridge!.MockDiscordDispatch(DiscordEventType.EntitlementCreate, data);
        }
    }
}
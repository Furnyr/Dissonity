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
        public MockQueryData query = new();

        // Activity
        public MockLocale locale = MockLocale.en_US;
        public MockPlayer currentPlayer = new();
        public List<MockPlayer> otherPlayers = new();
        public List<MockChannel> channels = new();

        // General events
        public LayoutModeType layoutMode = LayoutModeType.Focused;
        public OrientationType screenOrientation = OrientationType.Landscape;
        public ThermalStateType thermalState = ThermalStateType.Nominal;
        public string mockEntitlementId = "";

        // In-App Purchases
        public List<MockEntitlement> entitlements = new();
        public List<MockSku> skus = new();

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

            GameObject.DontDestroyOnLoad(gameObject);

            Api.MockOn();
        }

        internal Participant[] GetParticipants()
        {
            List<Participant> participants = new();

            participants.Add(currentPlayer.Participant.ToParticipant());

            // Add each player participant
            foreach (MockPlayer player in otherPlayers)
            {
                participants.Add(player.Participant.ToParticipant());
            }

            return participants.ToArray();
        }

        internal Sku[] GetSkus()
        {
            List<Sku> skuList = new();

            foreach (var dict in skus)
            {
                skuList.Add(dict.ToSku());
            }

            return skuList.ToArray();
        }

        internal Entitlement[] GetEntitlements()
        {
            List<Entitlement> entitlementList = new();

            foreach (var ent in entitlements)
            {
                entitlementList.Add(ent.ToEntitlement());
            }

            return entitlementList.ToArray();
        }

        internal UserVoiceState[] GetUserVoiceStates()
        {
            List<UserVoiceState> userVoiceStates = new();

            var myVoiceState = currentPlayer.UserVoiceState.ToUserVoiceState();
            myVoiceState.Nickname = currentPlayer.Participant.Nickname;
            myVoiceState.User = currentPlayer.Participant;

            userVoiceStates.Add(myVoiceState);

            // Add each player voice state
            foreach (MockPlayer player in otherPlayers)
            {
                var voiceState = player.UserVoiceState.ToUserVoiceState();
                voiceState.Nickname = player.Participant.Nickname;
                voiceState.User = player.Participant;

                userVoiceStates.Add(voiceState);
            }

            return userVoiceStates.ToArray();
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

            Api.bridge!.MockDispatch(DiscordEventType.ActivityInstanceParticipantsUpdate, data);
        }


        /// <summary>
        /// Dispatch a mock Activity Layout Mode Update.
        /// </summary>
        public void ActivityLayoutModeUpdate()
        {
            ActivityLayoutModeUpdateData data = new()
            {
                LayoutMode = layoutMode
            };

            Api.bridge!.MockDispatch(DiscordEventType.ActivityLayoutModeUpdate, data);
        }


        /// <summary>
        /// Dispatch a mock Orientation Update.
        /// </summary>
        public void OrientationUpdate()
        {
            OrientationUpdateData data = new()
            {
                ScreenOrientation = screenOrientation
            };

            Api.bridge!.MockDispatch(DiscordEventType.OrientationUpdate, data);
        }


        /// <summary>
        /// Dispatch a mock Thermal State Update.
        /// </summary>
        public void ThermalStateUpdate()
        {
            ThermalStateUpdateData data = new()
            {
                ThermalState = thermalState
            };

            Api.bridge!.MockDispatch(DiscordEventType.ThermalStateUpdate, data);
        }


        /// <summary>
        /// Dispatch a mock Current Guild Member Update.
        /// </summary>
        public void CurrentGuildMemberUpdate()
        {
            GuildMemberRpc data = currentPlayer.GuildMemberRpc.ToGuildMemberRpc();

            data.Nickname = currentPlayer.Participant.Nickname;
            data.UserId = currentPlayer.Participant.Id;

            Api.bridge!.MockDispatch(DiscordEventType.CurrentGuildMemberUpdate, data); 
        }


        /// <summary>
        /// Dispatch a mock Current User Update.
        /// </summary>
        public void CurrentUserUpdate()
        {
            User data = currentPlayer.Participant.ToUser();

            Api.bridge!.MockDispatch(DiscordEventType.CurrentUserUpdate, data);
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
            if (playerIndex == -1) data = currentPlayer.UserVoiceState;
            else data = otherPlayers[playerIndex].UserVoiceState;

            Api.bridge!.MockDispatch(DiscordEventType.VoiceStateUpdate, data);
        }


        /// <summary>
        /// Dispatch a mock Speaking Start. <br/> <br/>
        /// <c> playerIndex </c> -1 for current player.
        /// </summary>
        public void SpeakingStart(int playerIndex = -1)
        {
            //\ Dispatch
            string userId;

            //? Current player
            if (playerIndex == -1) userId = currentPlayer.Participant.Id;
            else userId = otherPlayers[playerIndex].Participant.Id;

            SpeakingStartData data = new()
            {
                ChannelId = query.ChannelId,
                UserId = userId
            };
            
            Api.bridge!.MockDispatch(DiscordEventType.SpeakingStart, data);
        }


        /// <summary>
        /// Dispatch a mock Speaking Stop. <br/> <br/>
        /// <c> playerIndex </c> -1 for current player.
        /// </summary>
        public void SpeakingStop(int playerIndex = -1)
        {
            //\ Dispatch
            string userId;

            //? Current player
            if (playerIndex == -1) userId = currentPlayer.Participant.Id;
            else userId = otherPlayers[playerIndex].Participant.Id;

            SpeakingStopData data = new()
            {
                ChannelId = query.ChannelId,
                UserId = userId
            };
            
            Api.bridge!.MockDispatch(DiscordEventType.SpeakingStop, data);
        }
    

        /// <summary>
        /// Dispatch a mock Entitlement Create.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public void EntitlementCreate(string mockEntitlementId)
        {
            MockEntitlement? ent = entitlements.Find(e => e.Id == mockEntitlementId);

            if (ent == null) throw new ArgumentException("You must pass a valid mock entitlement id");

            EntitlementCreateData data = new()
            {
                Entitlement = ent.ToEntitlement()
            };

            Api.bridge!.MockDispatch(DiscordEventType.EntitlementCreate, data);
        }
    }
}
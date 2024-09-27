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
        public MockLocale locale = MockLocale.en_US;

        public MockPlayer currentPlayer = new();

        public List<MockPlayer> otherPlayers = new();

        public List<MockDictionary<MockChannel>> channels = new();

        public MockQueryData query = new();

        // General events
        public LayoutModeType layoutMode = LayoutModeType.Focused;
        public OrientationType screenOrientation = OrientationType.Landscape;
        public ThermalStateType thermalState = ThermalStateType.Nominal;

        // Singleton
        public static DiscordMock Singleton { get; private set; }

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

            Api.EnableMock();
        }

        public Participant[] GetParticipants()
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

        public void ActivityInstanceParticipantsUpdate()
        {
            ActivityInstanceParticipantsUpdateData data = new()
            {
                Participants = GetParticipants()
            };

            Api.Bridge.GetComponent<DissonityBridge>().MockDispatch(DiscordEventType.ActivityInstanceParticipantsUpdate, data);
        }

        public void ActivityLayoutModeUpdate()
        {
            ActivityLayoutModeUpdateData data = new()
            {
                LayoutMode = layoutMode
            };

            Api.Bridge.GetComponent<DissonityBridge>().MockDispatch(DiscordEventType.ActivityLayoutModeUpdate, data);
        }

        public void OrientationUpdate()
        {
            OrientationUpdateData data = new()
            {
                ScreenOrientation = screenOrientation
            };

            Api.Bridge.GetComponent<DissonityBridge>().MockDispatch(DiscordEventType.OrientationUpdate, data);
        }

        public void ThermalStateUpdate()
        {
            ThermalStateUpdateData data = new()
            {
                ThermalState = thermalState
            };

            Api.Bridge.GetComponent<DissonityBridge>().MockDispatch(DiscordEventType.ThermalStateUpdate, data);
        }

        public void CurrentGuildMemberUpdate()
        {
            GuildMemberRpc data = currentPlayer.GuildMemberRpc;

            Api.Bridge.GetComponent<DissonityBridge>().MockDispatch(DiscordEventType.CurrentGuildMemberUpdate, data); 
        }

        public void CurrentUserUpdate()
        {
            User data = currentPlayer.Participant;

            Api.Bridge.GetComponent<DissonityBridge>().MockDispatch(DiscordEventType.CurrentUserUpdate, data);
        }

        public void VoiceStateUpdate(int playerIndex = -1)
        {
            //\ Dispatch
            UserVoiceState data;

            //? Current player
            if (playerIndex == -1) data = currentPlayer.UserVoiceState;
            else data = otherPlayers[playerIndex].UserVoiceState;

            Api.Bridge.GetComponent<DissonityBridge>().MockDispatch(DiscordEventType.VoiceStateUpdate, data);
        }

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
            
            Api.Bridge.GetComponent<DissonityBridge>().MockDispatch(DiscordEventType.SpeakingStart, data);
        }

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
            
            Api.Bridge.GetComponent<DissonityBridge>().MockDispatch(DiscordEventType.SpeakingStop, data);
        }
    }
}
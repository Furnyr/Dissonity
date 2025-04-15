using System;
using System.Collections.Generic;
using System.Linq;
using Dissonity.Models;

namespace Dissonity.Events
{
    internal static class EventUtility
    {
        #nullable enable annotations

        internal static Dictionary<string, Type> EventMap = new ()
        {
            { DiscordEventType.Ready, typeof(ReadyEvent) },
            { DiscordEventType.Error, typeof(ErrorEvent) },
            { DiscordEventType.VoiceStateUpdate, typeof(VoiceStateUpdate) },
            { DiscordEventType.SpeakingStart, typeof(SpeakingStart) },
            { DiscordEventType.SpeakingStop, typeof(SpeakingStop) },
            { DiscordEventType.ActivityInstanceParticipantsUpdate, typeof(ActivityInstanceParticipantsUpdate) },
            { DiscordEventType.ActivityLayoutModeUpdate, typeof(ActivityLayoutModeUpdate) },
            { DiscordEventType.CurrentUserUpdate, typeof(CurrentUserUpdate) },
            { DiscordEventType.CurrentGuildMemberUpdate, typeof(CurrentGuildMemberUpdate) },
            { DiscordEventType.OrientationUpdate, typeof(OrientationUpdate) },
            { DiscordEventType.ThermalStateUpdate, typeof(ThermalStateUpdate) },
            { DiscordEventType.EntitlementCreate, typeof(EntitlementCreate) },
        };

        // Don't confuse "event data" with "event return data".
        // Event data is the value of the "data" property sent by Discord,
        // while event return data is the useful data returned to the user.
        // E.g.: ActivityInstanceParticipantsUpdateData vs Participant[]

        internal static Dictionary<string, Type> EventDataMap = new ()
        {
            { DiscordEventType.Ready, typeof(ReadyEventData) },
            { DiscordEventType.Error, typeof(ErrorEventData) },
            { DiscordEventType.VoiceStateUpdate, typeof(UserVoiceState) },
            { DiscordEventType.SpeakingStart, typeof(SpeakingData) },
            { DiscordEventType.SpeakingStop, typeof(SpeakingData) },
            { DiscordEventType.ActivityInstanceParticipantsUpdate, typeof(ActivityInstanceParticipantsUpdateData) },
            { DiscordEventType.ActivityLayoutModeUpdate, typeof(ActivityLayoutModeUpdateData) },
            { DiscordEventType.CurrentUserUpdate, typeof(User) },
            { DiscordEventType.CurrentGuildMemberUpdate, typeof(GuildMemberRpc) },
            { DiscordEventType.OrientationUpdate, typeof(OrientationUpdateData) },
            { DiscordEventType.ThermalStateUpdate, typeof(ThermalStateUpdateData) },
            { DiscordEventType.EntitlementCreate, typeof(EntitlementCreateData) },
        };

        internal static Dictionary<string, Type> EventReturnDataMap = new ()
        {
            { DiscordEventType.Ready, typeof(ReadyEventData) },
            { DiscordEventType.Error, typeof(ErrorEventData) },
            { DiscordEventType.VoiceStateUpdate, typeof(UserVoiceState) },
            { DiscordEventType.SpeakingStart, typeof(SpeakingData) },
            { DiscordEventType.SpeakingStop, typeof(SpeakingData) },
            { DiscordEventType.ActivityInstanceParticipantsUpdate, typeof(Participant[]) },
            { DiscordEventType.ActivityLayoutModeUpdate, typeof(LayoutModeType) },
            { DiscordEventType.CurrentUserUpdate, typeof(User) },
            { DiscordEventType.CurrentGuildMemberUpdate, typeof(GuildMemberRpc) },
            { DiscordEventType.OrientationUpdate, typeof(OrientationType) },
            { DiscordEventType.ThermalStateUpdate, typeof(ThermalStateType) },
            { DiscordEventType.EntitlementCreate, typeof(Entitlement) },
        };

        internal static Type GetTypeFromString(string eventString)
        {
            if (!EventMap.ContainsKey(eventString)) throw new Exception($"Couldn't find event type for '{eventString}'");

            Type eventType = EventMap[eventString];

            return eventType;
        }

        internal static string GetStringFromType(Type eventType)
        {
            if (!EventMap.ContainsValue(eventType)) throw new Exception($"Couldn't find event string for '{eventType}'");

            string eventString = EventMap.First(x => x.Value == eventType).Key;

            return eventString;
        }
    
        internal static Type GetDataTypeFromString(string eventString)
        {
            if (!EventDataMap.ContainsKey(eventString)) throw new Exception($"Couldn't find response data type for '{eventString}'");

            Type dataType = EventDataMap[eventString];

            return dataType;
        }

        internal static string GetStringFromDataType(Type dataType)
        {
            if (!EventDataMap.ContainsValue(dataType)) throw new Exception($"Couldn't find event string for '{dataType}'");

            string eventString = EventDataMap.First(x => x.Value == dataType).Key;

            return eventString;
        }

        internal static string GetStringFromReturnDataType(Type returnDataType)
        {
            if (!EventReturnDataMap.ContainsValue(returnDataType)) throw new Exception($"Couldn't find event string for '{returnDataType}'");

            string eventString = EventReturnDataMap.First(x => x.Value == returnDataType).Key;

            return eventString;
        }
    }
}
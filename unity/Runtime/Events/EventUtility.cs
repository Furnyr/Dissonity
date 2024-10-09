using System;
using System.Collections.Generic;
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

        internal static Dictionary<Type, string> EventStringMap = new ()
        {
            { typeof(ReadyEvent), DiscordEventType.Ready },
            { typeof(ErrorEvent), DiscordEventType.Error },
            { typeof(VoiceStateUpdate), DiscordEventType.VoiceStateUpdate },
            { typeof(SpeakingStart), DiscordEventType.SpeakingStart },
            { typeof(SpeakingStop), DiscordEventType.SpeakingStop },
            { typeof(ActivityInstanceParticipantsUpdate), DiscordEventType.ActivityInstanceParticipantsUpdate },
            { typeof(ActivityLayoutModeUpdate), DiscordEventType.ActivityLayoutModeUpdate },
            { typeof(CurrentUserUpdate), DiscordEventType.CurrentUserUpdate },
            { typeof(CurrentGuildMemberUpdate), DiscordEventType.CurrentGuildMemberUpdate },
            { typeof(OrientationUpdate), DiscordEventType.OrientationUpdate },
            { typeof(ThermalStateUpdate), DiscordEventType.ThermalStateUpdate },
            { typeof(EntitlementCreate), DiscordEventType.EntitlementCreate }
        };

        internal static Dictionary<string, Type> EventDataMap = new ()
        {
            { DiscordEventType.Ready, typeof(ReadyEventData) },
            { DiscordEventType.Error, typeof(ErrorEventData) },
            { DiscordEventType.VoiceStateUpdate, typeof(UserVoiceState) },
            { DiscordEventType.SpeakingStart, typeof(SpeakingStartData) },
            { DiscordEventType.SpeakingStop, typeof(SpeakingStopData) },
            { DiscordEventType.ActivityInstanceParticipantsUpdate, typeof(ActivityInstanceParticipantsUpdateData) },
            { DiscordEventType.ActivityLayoutModeUpdate, typeof(ActivityLayoutModeUpdateData) },
            { DiscordEventType.CurrentUserUpdate, typeof(User) },
            { DiscordEventType.CurrentGuildMemberUpdate, typeof(GuildMemberRpc) },
            { DiscordEventType.OrientationUpdate, typeof(OrientationUpdateData) },
            { DiscordEventType.ThermalStateUpdate, typeof(ThermalStateUpdateData) },
            { DiscordEventType.EntitlementCreate, typeof(EntitlementCreateData) },
        };

        internal static Type GetTypeFromString(string eventString)
        {
            if (!EventMap.ContainsKey(eventString)) throw new Exception($"Couldn't find event type for '{eventString}'");

            Type eventType = EventMap[eventString];

            return eventType;
        }

        internal static string GetStringFromType(Type eventType)
        {
            if (!EventStringMap.ContainsKey(eventType)) throw new Exception($"Couldn't find event string for '{eventType}'");

            string eventString = EventStringMap[eventType];

            return eventString;
        }
    
        internal static Type GetDataTypeFromString(string eventString)
        {
            if (!EventDataMap.ContainsKey(eventString)) throw new Exception($"Couldn't find response data type for '{eventString}'");

            Type dataType = EventDataMap[eventString];

            return dataType;
        }
    }
}
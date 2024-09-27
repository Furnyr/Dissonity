using System;
using Dissonity.Models;

namespace Dissonity.Events
{
    internal static class EventUtility
    {
        internal static Type GetTypeFromString(string eventString)
        {
            switch (eventString)
            {
                case DiscordEventType.Ready:
                    return typeof(ReadyEvent);
                case DiscordEventType.Error:
                    return typeof(ErrorEvent);
                case DiscordEventType.VoiceStateUpdate:
                    return typeof(VoiceStateUpdate);
                case DiscordEventType.SpeakingStart:
                    return typeof(SpeakingStart);
                case DiscordEventType.SpeakingStop:
                    return typeof(SpeakingStop);
                case DiscordEventType.ActivityInstanceParticipantsUpdate:
                    return typeof(ActivityInstanceParticipantsUpdate);
                case DiscordEventType.ActivityLayoutModeUpdate:
                    return typeof(ActivityLayoutModeUpdate);
                case DiscordEventType.CurrentUserUpdate:
                    return typeof(CurrentUserUpdate);
                case DiscordEventType.CurrentGuildMemberUpdate:
                    return typeof(CurrentGuildMemberUpdate);
                case DiscordEventType.OrientationUpdate:
                    return typeof(OrientationUpdate);
                case DiscordEventType.ThermalStateUpdate:
                    return typeof(ThermalStateUpdate);
                case DiscordEventType.EntitlementCreate:
                    return typeof(EntitlementCreate);
                default:
                    throw new Exception($"Couldn't find event type for '{eventString}'");
            }
        }

        internal static string GetStringFromType(Type eventType)
        {
            if (eventType == typeof(ReadyEvent))
                return DiscordEventType.Ready;
            if (eventType == typeof(ErrorEvent))
                return DiscordEventType.Error;
            if (eventType == typeof(VoiceStateUpdate))
                return DiscordEventType.VoiceStateUpdate;
            if (eventType == typeof(SpeakingStart))
                return DiscordEventType.SpeakingStart;
            if (eventType == typeof(SpeakingStop))
                return DiscordEventType.SpeakingStop;
            if (eventType == typeof(ActivityInstanceParticipantsUpdate))
                return DiscordEventType.ActivityInstanceParticipantsUpdate;
            if (eventType == typeof(ActivityLayoutModeUpdate))
                return DiscordEventType.ActivityLayoutModeUpdate;
            if (eventType == typeof(CurrentUserUpdate))
                return DiscordEventType.CurrentUserUpdate;
            if (eventType == typeof(CurrentGuildMemberUpdate))
                return DiscordEventType.CurrentGuildMemberUpdate;
            if (eventType == typeof(OrientationUpdate))
                return DiscordEventType.OrientationUpdate;
            if (eventType == typeof(ThermalStateUpdate))
                return DiscordEventType.ThermalStateUpdate;
            if (eventType == typeof(EntitlementCreate))
                return DiscordEventType.EntitlementCreate;
            
            throw new Exception($"Couldn't find event string for '{eventType}'");
        }
    
        internal static Type GetDataTypeFromString(string eventString)
        {
            switch (eventString)
            {
                case DiscordEventType.Ready:
                    return typeof(ReadyEventData);
                case DiscordEventType.Error:
                    return typeof(ErrorEventData);
                case DiscordEventType.VoiceStateUpdate:
                    return typeof(UserVoiceState);
                case DiscordEventType.SpeakingStart:
                    return typeof(SpeakingStartData);
                case DiscordEventType.SpeakingStop:
                    return typeof(SpeakingStopData);
                case DiscordEventType.ActivityInstanceParticipantsUpdate:
                    return typeof(ActivityInstanceParticipantsUpdateData);
                case DiscordEventType.ActivityLayoutModeUpdate:
                    return typeof(ActivityLayoutModeUpdateData);
                case DiscordEventType.CurrentUserUpdate:
                    return typeof(User);
                case DiscordEventType.CurrentGuildMemberUpdate:
                    return typeof(GuildMemberRpc);
                case DiscordEventType.OrientationUpdate:
                    return typeof(OrientationUpdateData);
                case DiscordEventType.ThermalStateUpdate:
                    return typeof(ThermalStateUpdateData);
                case DiscordEventType.EntitlementCreate:
                    return typeof(EntitlementCreateData);
                default:
                    throw new Exception($"Couldn't find response data type for '{eventString}'");
            }
        }

    }
}
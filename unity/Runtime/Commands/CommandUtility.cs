using System;
using Dissonity.Commands.Responses;
using Dissonity.Models;

namespace Dissonity.Commands
{
    internal static class CommandUtility
    {
        internal static Type GetResponseFromString(string commandString)
        {
            switch (commandString)
            {
                case DiscordCommandType.Authorize:
                    return typeof(AuthorizeResponse);
                case DiscordCommandType.Authenticate:
                    return typeof(AuthenticateResponse);
                case DiscordCommandType.EncourageHwAcceleration:
                    return typeof(EncourageHardwareAccelerationResponse);
                case DiscordCommandType.GetChannelPermissions:
                    return typeof(GetChannelPermissionsResponse);
                case DiscordCommandType.GetChannel:
                    return typeof(GetChannelResponse);
                case DiscordCommandType.GetActivityInstanceConnectedParticipants:
                    return typeof(GetInstanceConnectedParticipantsResponse);
                case DiscordCommandType.GetPlatformBehaviors:
                    return typeof(GetPlatformBehaviorsResponse);
                case DiscordCommandType.InitiateImageUpload:
                    return typeof(InitiateImageUploadResponse);
                case DiscordCommandType.SetActivity:
                    return typeof(SetActivityResponse);
                case DiscordCommandType.SetConfig:
                    return typeof(SetConfigResponse);
                case DiscordCommandType.Subscribe:
                case DiscordCommandType.Unsubscribe:
                    return typeof(SubscribeResponse);
                case DiscordCommandType.UserSettingsGetLocale:
                    return typeof(UserSettingsGetLocaleResponse);
                
                default:
                    return typeof(NoResponse);
            }
        }
  
        internal static Type GetDataTypeFromString(string commandString)
        {
            switch (commandString)
            {
                case DiscordCommandType.Authorize:
                    return typeof(AuthorizeData);
                case DiscordCommandType.Authenticate:
                    return typeof(AuthenticateData);
                case DiscordCommandType.EncourageHwAcceleration:
                    return typeof(EncourageHardwareAccelerationData);
                case DiscordCommandType.GetChannelPermissions:
                    return typeof(GetChannelPermissionsData);
                case DiscordCommandType.GetChannel:
                    return typeof(GetChannelData);
                case DiscordCommandType.GetActivityInstanceConnectedParticipants:
                    return typeof(GetInstanceConnectedParticipantsData);
                case DiscordCommandType.GetPlatformBehaviors:
                    return typeof(GetPlatformBehaviorsData);
                case DiscordCommandType.InitiateImageUpload:
                    return typeof(InitiateImageUploadData);
                case DiscordCommandType.SetActivity:
                    return typeof(Activity);
                case DiscordCommandType.SetConfig:
                    return typeof(SetConfigData);
                case DiscordCommandType.Subscribe:
                case DiscordCommandType.Unsubscribe:
                    return typeof(SubscribeData);
                case DiscordCommandType.UserSettingsGetLocale:
                    return typeof(UserSettingsGetLocaleData);
                
                default:
                    return typeof(NoResponse);
            }
        }
    }
}
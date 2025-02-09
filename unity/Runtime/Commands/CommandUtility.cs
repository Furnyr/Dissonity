using System;
using System.Collections.Generic;
using Dissonity.Commands.Responses;
using Dissonity.Models;

namespace Dissonity.Commands
{
    internal static class CommandUtility
    {
        #nullable enable

        // If the command isn't in the map, it's a NoResponse

        internal static Dictionary<string, Type> CommandResponseMap = new ()
        {
            { DiscordCommandType.Authenticate, typeof(AuthenticateResponse) },
            { DiscordCommandType.Authorize, typeof(AuthorizeResponse) },
            { DiscordCommandType.EncourageHwAcceleration, typeof(EncourageHardwareAccelerationResponse) },
            { DiscordCommandType.GetChannel, typeof(GetChannelResponse) },
            { DiscordCommandType.GetChannelPermissions, typeof(GetChannelPermissionsResponse) },
            { DiscordCommandType.GetEntitlementsEmbedded, typeof(GetEntitlementsResponse) },
            { DiscordCommandType.GetActivityInstanceConnectedParticipants, typeof(GetInstanceConnectedParticipantsResponse) },
            { DiscordCommandType.GetPlatformBehaviors, typeof(GetPlatformBehaviorsResponse) },
            { DiscordCommandType.GetSkusEmbedded, typeof(GetSkusResponse) },
            { DiscordCommandType.InitiateImageUpload, typeof(InitiateImageUploadResponse) },
            { DiscordCommandType.OpenExternalLink, typeof(OpenExternalLinkResponse) },
            { DiscordCommandType.SetActivity, typeof(SetActivityResponse) },
            { DiscordCommandType.SetConfig, typeof(SetConfigResponse) },
            { DiscordCommandType.StartPurchase, typeof(StartPurchaseResponse) },
            { DiscordCommandType.Subscribe, typeof(SubscribeResponse) },
            { DiscordCommandType.Unsubscribe, typeof(SubscribeResponse) },
            { DiscordCommandType.UserSettingsGetLocale, typeof(UserSettingsGetLocaleResponse) },
            { DiscordCommandType.ShareLink, typeof(ShareLinkResponse) },
        };

        internal static Dictionary<string, Type> CommandDataMap = new ()
        {
            { DiscordCommandType.Authenticate, typeof(AuthenticateData) },
            { DiscordCommandType.Authorize, typeof(AuthorizeData) },
            { DiscordCommandType.EncourageHwAcceleration, typeof(EncourageHardwareAccelerationData) },
            { DiscordCommandType.GetChannel, typeof(ChannelRpc) },
            { DiscordCommandType.GetChannelPermissions, typeof(GetChannelPermissionsData) },
            { DiscordCommandType.GetEntitlementsEmbedded, typeof(GetEntitlementsData) },
            { DiscordCommandType.GetActivityInstanceConnectedParticipants, typeof(GetInstanceConnectedParticipantsData) },
            { DiscordCommandType.GetPlatformBehaviors, typeof(GetPlatformBehaviorsData) },
            { DiscordCommandType.GetSkusEmbedded, typeof(GetSkusData) },
            { DiscordCommandType.InitiateImageUpload, typeof(InitiateImageUploadData) },
            { DiscordCommandType.OpenExternalLink, typeof(OpenExternalLinkData) },
            { DiscordCommandType.SetActivity, typeof(Activity) },
            { DiscordCommandType.SetConfig, typeof(SetConfigData) },
            { DiscordCommandType.StartPurchase, typeof(Entitlement[]) },
            { DiscordCommandType.Subscribe, typeof(SubscribeData) },
            { DiscordCommandType.Unsubscribe, typeof(SubscribeData) },
            { DiscordCommandType.UserSettingsGetLocale, typeof(UserSettingsGetLocaleData) },
            { DiscordCommandType.ShareLink, typeof(ShareLinkData) },
        };

        internal static Type GetResponseFromString(string commandString)
        {
            if (!CommandResponseMap.ContainsKey(commandString)) return typeof(NoResponse);

            Type responseType = CommandResponseMap[commandString];

            return responseType;
        }
  
        internal static Type GetDataTypeFromString(string commandString)
        {
            if (!CommandDataMap.ContainsKey(commandString)) return typeof(NoResponse);

            Type dataType = CommandDataMap[commandString];

            return dataType;
        }
    }
}
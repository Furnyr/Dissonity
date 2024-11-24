using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Dissonity.Events;
using Dissonity.Bus;
using Dissonity.Models.Interop;
using Dissonity.Commands;
using Dissonity.Commands.Responses;
using Dissonity.Models;
using Dissonity.Models.Builders;
using Dissonity.Models.Mock;
using System.Net;
using System.Linq;
using System.Globalization;

//todo main tasks
// Test patch url mappings
// Test real functionality
// Test that everything users don't need to see is internal
// Documentation
// Logo and brand
// GitHub repository stuff

namespace Dissonity
{
    public static class Api
    {
        #nullable enable

        //# FIELDS - - - - -
        internal static long? _clientId;
        internal static string? _instanceId;
        internal static string? _accessToken;
        internal static string? _platform;
        internal static long? _guildId;
        internal static long? _channelId;
        internal static long? _userId = null;
        internal static string? _frameId;
        internal static string? _mobileAppVersion = null;
        internal static ISdkConfiguration? _configuration;
        
        // Utility
        internal static User? _user = null;
        internal static GuildMemberRpc? _guildMemberRpc = null;

        // Messages
        internal static Dictionary<string, object> pendingCommands = new(); // TaskCompletionSource<TResponse>
        internal static MessageBus messageBus = new();
        internal static GameObject? bridgeObject = null;
        internal static DissonityBridge? bridge = null;
        internal static HashSet<string> subscribedRpcSet = new();

        // Shortcut
        internal static bool isEditor = UnityEngine.Application.isEditor;

        // Initialization
        private static bool _initialized = false;
        private static bool _ready = false;
        private static bool _mock = false;
        private static bool disableMock = false;

        // RpcVersion and RpcEncoding (handshake, overall) is handled in the RpcBridge
        // HANDSHAKE_SDK_VERSION_MINIUM_MOBILE_VERSION is in the BridgeLib


        //# PROPERTIES - - - - -
        /// <summary>
        /// Embedded App SDK version that Dissonity is mirroring. Note that some patch versions may not apply here.
        /// </summary>
        public const string SdkVersion = "1.6.1";
        public const string ProxyDomain = "discordsays.com";

        /// <summary>
        /// <c> ❄️ </c> Your app's client id.
        /// </summary>
        public static long ClientId
        {
            get
            {
                if (!_ready) throw new InvalidOperationException("You can't access this property before waiting for Api.Initialize");

                return (long) _clientId!;
            }
        }
        
        /// <summary>
        /// Unique string id for each activity instance.
        /// </summary>
        public static string InstanceId
        { 
            get
            {
                if (!_ready) throw new InvalidOperationException("You can't access this property before waiting for Api.Initialize");

                if (_mock) return GameObject.FindAnyObjectByType<DiscordMock>()._query.InstanceId;

                return _instanceId!;
            }
        }
        
        /// <summary>
        /// Your client access token
        /// </summary>
        public static string? AccessToken
        {
            get
            {
                if (!_ready) throw new InvalidOperationException("You can't access this property before waiting for Api.Initialize");

                return _accessToken;
            }
        }
        
        /// <summary>
        /// The platform on which the activity is running. It's a value of <c> Models.Platform </c>.
        /// </summary>
        public static string Platform
        {
           get
            {
                if (!_ready) throw new InvalidOperationException("You can't access this property before waiting for Api.Initialize");

                if (_mock) return MockUtils.ToPlatformString(GameObject.FindAnyObjectByType<DiscordMock>()._query.Platform);

                return _platform!;
            } 
        }  
        
        /// <summary>
        /// <c> ❄️ </c> The id of the guild on which the activity is running.
        /// </summary>
        public static long GuildId
        {
            get
            {
                if (!_ready) throw new InvalidOperationException("You can't access this property before waiting for Api.Initialize");

                if (_mock) return GameObject.FindAnyObjectByType<DiscordMock>()._query.GuildId;

                return (long) _guildId!;
            }
        }
        
        /// <summary>
        /// <c> ❄️ </c> The id of the channel on which the activity is running.
        /// </summary>
        public static long ChannelId
        {
            get
            {
                if (!_ready) throw new InvalidOperationException("You can't access this property before waiting for Api.Initialize");

                if (_mock) return GameObject.FindAnyObjectByType<DiscordMock>()._query.ChannelId;

                return (long) _channelId!;
            }
        }
        
        /// <summary>
        /// The activity frame id.
        /// </summary>
        public static string FrameId
        {
            get
            {
                if (!_ready) throw new InvalidOperationException("You can't access this property before waiting for Api.Initialize");

                if (_mock) return GameObject.FindAnyObjectByType<DiscordMock>()._query.FrameId;

                return _frameId!;
            }
        }
        
        /// <summary>
        /// The mobile client version. Returns null in desktop.
        /// </summary>
        public static string? MobileAppVersion
        {
            get
            {
                if (!_ready) throw new InvalidOperationException("You can't access this property before waiting for Api.Initialize");

                if (_mock)
                {
                    var mock = GameObject.FindAnyObjectByType<DiscordMock>();

                    if (mock._query.Platform == MockPlatform.Desktop) return null;
                    else return mock._query.MobileAppVersion;
                }

                return _mobileAppVersion;
            }
        }
        
        /// <summary>
        /// <c> ❄️ </c> The current user id.
        /// </summary>
        public static long? UserId
        {
            get
            {
                if (!_ready) throw new InvalidOperationException("You can't access this property before waiting for Api.Initialize");

                if (_mock) {

                    if (!_configuration!.OauthScopes.Contains(OauthScope.Identify))
                    {
                        if (!_configuration!.DisableDissonityInfoLogs) Utils.DissonityLogWarning("Tried to access the current user id without the 'identify' scope. You will receive null");

                        return null;
                    }

                    return GameObject.FindAnyObjectByType<DiscordMock>()._currentPlayer.Participant.Id;
                }

                if (!_configuration!.DisableDissonityInfoLogs && _userId == null)
                {
                    Utils.DissonityLogWarning("Tried to access the current user id without the 'identify' scope. You will receive null");
                }

                return _userId;
            }
        }
        
        /// <summary>
        /// True after the first <c> Api.Initialize </c> call, regardless of success.
        /// </summary>
        public static bool Initialized
        {
            get
            {
                return _initialized;
            }
        }
        
        /// <summary>
        /// True if the <c> Api.Initialize </c> call was successful.
        /// </summary>
        public static bool Ready
        {
            get
            {
                return _ready;
            }
        }
        
        /// <summary>
        /// If <c> SynchronizeUser </c> is enabled in the config, returns the current user object.
        /// </summary>
        public static User? SyncedUser
        {
            get
            {
                if (!_configuration!.SynchronizeUser) throw new InvalidOperationException("To access this property you need to enable 'SynchronizeUser'");

                if (!_ready) throw new InvalidOperationException("You can't access this property before waiting for Api.Initialize");

                if (_mock) {

                    if (!_configuration!.OauthScopes.Contains(OauthScope.Identify))
                    {
                        if (!_configuration!.DisableDissonityInfoLogs) Utils.DissonityLogWarning("Tried to access SyncedUser without the 'identify' scope. You will receive null");

                        return null;
                    }

                    return GameObject.FindAnyObjectByType<DiscordMock>()._currentPlayer.Participant.ToUser();
                }

                if (!_configuration!.DisableDissonityInfoLogs && _user == null)
                {
                    Utils.DissonityLogWarning("Tried to access SyncedUser without the 'identify' scope. You will receive null");
                }

                return _user;
            }
        }
        
        /// <summary>
        /// If <c> SynchronizeGuildMemberRpc </c> is enabled in the config, returns the current guild member RPC object.
        /// </summary>
        public static GuildMemberRpc? SyncedGuildMemberRpc
        {
            get
            {
                if (!_configuration!.SynchronizeGuildMemberRpc) throw new InvalidOperationException("To access this property you need to enable 'SynchronizeGuildMemberRpc'");

                if (!_ready) throw new InvalidOperationException("You can't access this property before waiting for Api.Initialize");

                if (_mock) {

                    if (!_configuration!.OauthScopes.Contains(OauthScope.Identify) || !_configuration!.OauthScopes.Contains(OauthScope.GuildsMembersRead))
                    {
                        if (!_configuration!.DisableDissonityInfoLogs) Utils.DissonityLogWarning("Tried to access SyncedGuildMemberRpc without the 'identify' and 'guilds.members.read' scopes. You will receive null");

                        return null;
                    }

                    return GameObject.FindAnyObjectByType<DiscordMock>().GetGuildMemberRpc();
                }

                if (!_configuration!.DisableDissonityInfoLogs && _guildMemberRpc == null)
                {
                    Utils.DissonityLogWarning("Tried to access SyncedGuildMemberRpc without the 'identify' and 'guilds.members.read' scopes. You will receive null");
                }

                return _guildMemberRpc;
            }
        }

        public static ISdkConfiguration Configuration
        {
            get
            {
                if (!_ready) throw new InvalidOperationException("You can't access this property before waiting for Api.Initialize");

                return _configuration!;
            }
        }
        public static bool IsMock => _mock;


        //# JAVASCRIPT - - - - -
#if UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern void LoadInterface();

        [DllImport("__Internal")]
        private static extern void StopListening();

        [DllImport("__Internal")]
        private static extern void Send(string stringifiedMessage);
#endif

#if !UNITY_WEBGL
        private static void LoadInterface() {}
        private static void StopListening() {}
        private static void Send(string _) {}
#endif


        //# COMMANDS - - - - -
        public static class Commands
        {
            /// <summary>
            /// Forward logs to your own logger. <br/> <br/>
            /// No scopes required. <br/> <br/>
            /// ---------------------- <br/>
            /// ✅ | Web <br/>
            /// ✅ | iOS <br/>
            /// ✅ | Android <br/>
            /// ---------------------- <br/>
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            /// <exception cref="CommandException"></exception>
            public static async Task CaptureLog(string consoleLevel, string message)
            {
                if (!_ready) throw new InvalidOperationException("Tried to use a command without being ready");

                if (_mock) return;

                await SendCommand<CaptureLog, NoResponse>(new CaptureLog(consoleLevel, message));
            }


            /// <summary>
            /// Presents a modal dialog to allow enabling of hardware acceleration. <br/> <br/>
            /// No scopes required. <br/> <br/>
            /// ---------------------- <br/>
            /// ✅ | Web <br/>
            /// ⛔️ | iOS <br/>
            /// ⛔️ | Android <br/>
            /// ---------------------- <br/>
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            /// <exception cref="CommandException"></exception>
            public static async Task<EncourageHardwareAccelerationData> EncourageHardwareAcceleration()
            {
                if (!_ready) throw new InvalidOperationException("Tried to use a command without being ready");

                if (_mock)
                {
                    var mockResponse = await MockSendCommand<EncourageHardwareAccelerationResponse>();

                    if (Platform == Models.Platform.Mobile)
                    {
                        mockResponse.Data.Enabled = false;

                        if (!_configuration!.DisableDissonityInfoLogs) Utils.DissonityLogWarning("Platform is mobile, not possible to encourage hardware acceleration");
                    }

                    return mockResponse.Data;
                }

                var response = await SendCommand<EncourageHardwareAcceleration, EncourageHardwareAccelerationResponse>(new());

                return response.Data;                
            }


            /// <summary>
            /// Returns information about the channel for a provided channel ID. <br/> <br/>
            /// Scopes required: <c> guilds </c> (+ <c> dm_channels.read </c> for (G)DM channels.) <br/> <br/>
            /// <c> dm_channels.read </c> requires approval from Discord. <br/>
            /// ---------------------- <br/>
            /// ✅ | Web <br/>
            /// ✅ | iOS <br/>
            /// ✅ | Android <br/>
            /// ---------------------- <br/>
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            /// <exception cref="CommandException"></exception>
            public static async Task<ChannelRpc> GetChannel(long channelId)
            {
                if (!_ready) throw new InvalidOperationException("Tried to use a command without being ready");

                if (_mock)
                {
                    var mockResponse = await MockSendCommand<GetChannelResponse>(channelId);

                    if (mockResponse.Data.Type == ChannelType.Dm || mockResponse.Data.Type == ChannelType.GroupDm)
                    {
                        //? Invalid scopes
                        if (!_configuration!.OauthScopes.Contains(OauthScope.Guilds) || !_configuration!.OauthScopes.Contains(OauthScope.DmChannelsRead))
                        {
                            if (!_configuration!.DisableDissonityInfoLogs) Utils.DissonityLogWarning("Channel is DM and oauth scopes don't include 'guilds' and 'dm_channels.read'. Can't get the channel");
                        
                            throw new CommandException("Invalid oauth scopes inside mock", (int) RpcErrorCode.InvalidPermissions);
                        }
                    }

                    else
                    {
                        //? Invalid scopes
                        if (!_configuration!.OauthScopes.Contains(OauthScope.Guilds))
                        {
                            if (!_configuration!.DisableDissonityInfoLogs) Utils.DissonityLogWarning("Oauth scopes don't include 'guilds'. Can't get the channel");

                            throw new CommandException("Invalid oauth scopes inside mock", (int) RpcErrorCode.InvalidPermissions);
                        }
                    }

                    return mockResponse.Data;
                }

                var response = await SendCommand<GetChannel, GetChannelResponse>(new (channelId.ToString()));

                return response.Data;
            }


            /// <summary>
            /// Returns permissions for the current user in the currently connected channel. <br/> <br/>
            /// Scopes required: <c> guilds.members.read </c> <br/>
            /// ---------------------- <br/>
            /// ✅ | Web <br/>
            /// ✅ | iOS <br/>
            /// ✅ | Android <br/>
            /// ---------------------- <br/>
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            /// <exception cref="CommandException"></exception>
            public static async Task<GetChannelPermissionsData> GetChannelPermissions()
            {
                if (!_ready) throw new InvalidOperationException("Tried to use a command without being ready");

                if (_mock)
                {
                    //? Invalid scopes
                    if (!_configuration!.OauthScopes.Contains(OauthScope.GuildsMembersRead))
                    {
                        if (!_configuration!.DisableDissonityInfoLogs) Utils.DissonityLogWarning("Oauth scopes don't include 'guilds.members.read'. Can't get channel permissions");
                    
                        throw new CommandException("Invalid oauth scopes inside mock", (int) RpcErrorCode.InvalidPermissions);
                    }

                    var mockResponse = await MockSendCommand<GetChannelPermissionsResponse>();

                    return mockResponse.Data;
                }

                var response = await SendCommand<GetChannelPermissions, GetChannelPermissionsResponse>(new ());

                return response.Data;
            }


            /// <summary>
            /// Returns a list of entitlements for the current user. <br/> <br/>
            /// No scopes required. <br/> <br/>
            /// ---------------------- <br/>
            /// ✅ | Web <br/>
            /// ✅ | iOS <br/>
            /// ✅ | Android <br/>
            /// ---------------------- <br/>
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            /// <exception cref="CommandException"></exception>
            public static async Task<Entitlement[]> GetEntitlements()
            {
                if (!_ready) throw new InvalidOperationException("Tried to use a command without being ready");

                if (_mock)
                {
                    var mockResponse = await MockSendCommand<GetEntitlementsResponse>();

                    return mockResponse.Data.Entitlements;
                }

                var response = await SendCommand<GetEntitlements, GetEntitlementsResponse>(new ());

                return response.Data.Entitlements;
            }


            /// <summary>
            /// Returns a list of SKU objects. SKUs without prices are automatically filtered out. <br/> <br/>
            /// No scopes required. <br/> <br/>
            /// ---------------------- <br/>
            /// ✅ | Web <br/>
            /// ✅ | iOS <br/>
            /// ✅ | Android <br/>
            /// ---------------------- <br/>
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            /// <exception cref="CommandException"></exception>
            public static async Task<Sku[]> GetSkus()
            {
                if (!_ready) throw new InvalidOperationException("Tried to use a command without being ready");

                if (_mock)
                {
                    var mockResponse = await MockSendCommand<GetSkusResponse>();

                    return mockResponse.Data.Skus;
                }

                var response = await SendCommand<GetSkus, GetSkusResponse>(new ());

                return response.Data.Skus;
            }


            /// <summary>
            /// Launches the purchase flow for a specific SKU ID. <br/> <br/>
            /// No scopes required. <br/> <br/>
            /// ---------------------- <br/>
            /// ✅ | Web <br/>
            /// ⛔️ | iOS <br/>
            /// ⛔️ | Android <br/>
            /// ---------------------- <br/>
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            /// <exception cref="CommandException"></exception>
            public static async Task<Entitlement[]> StartPurchase(long skuId)
            {
                if (!_ready) throw new InvalidOperationException("Tried to use a command without being ready");

                if (_mock)
                {
                    var mockResponse = await MockSendCommand<StartPurchaseResponse>(skuId);

                    if (Platform == Models.Platform.Mobile)
                    {
                        mockResponse.Data = new Entitlement[] {};

                        if (!_configuration!.DisableDissonityInfoLogs) Utils.DissonityLogWarning("Platform is mobile, not possible to start purchase");
                    }

                    if (mockResponse.Data == null) return new Entitlement[] {};

                    return mockResponse.Data;
                }

                var response = await SendCommand<StartPurchase, StartPurchaseResponse>(new (skuId.ToString()));

                if (response.Data == null) return new Entitlement[] {};

                return response.Data;                
            }


            /// <summary>
            /// Returns information about supported platform behaviors. <br/> <br/>
            /// No scopes required. <br/>
            /// ---------------------- <br/>
            /// ✅ | Web <br/>
            /// ✅ | iOS <br/>
            /// ✅ | Android <br/>
            /// ---------------------- <br/>
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            /// <exception cref="CommandException"></exception>
            public static async Task<GetPlatformBehaviorsData> GetPlatformBehaviors()
            {
                if (!_ready) throw new InvalidOperationException("Tried to use a command without being ready");

                if (_mock)
                {
                    var mockResponse = await MockSendCommand<GetPlatformBehaviorsResponse>();

                    return mockResponse.Data;
                }

                var response = await SendCommand<GetPlatformBehaviors, GetPlatformBehaviorsResponse>(new ());

                return response.Data;
            }


            /// <summary>
            /// Allows for opening an external link from within the Discord client. <br/> <br/>
            /// No scopes required. <br/>
            /// ---------------------- <br/>
            /// ✅ | Web <br/>
            /// ✅ | iOS <br/>
            /// ✅ | Android <br/>
            /// ---------------------- <br/>
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            /// <exception cref="CommandException"></exception>
            public static async Task OpenExternalLink(string url)
            {
                if (!_ready) throw new InvalidOperationException("Tried to use a command without being ready");

                if (_mock && !_configuration!.DisableDissonityInfoLogs)
                {
                    Utils.DissonityLog($"Open external link ({url}) sent");
                    return;
                }

                await SendCommand<OpenExternalLink, NoResponse>(new (url));
            }


            /// <summary>
            /// Presents a modal dialog with Channel Invite UI. <br/> <br/>
            /// No scopes required. <br/>
            /// ---------------------- <br/>
            /// ✅ | Web <br/>
            /// ✅ | iOS <br/>
            /// ✅ | Android <br/>
            /// ---------------------- <br/>
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            /// <exception cref="CommandException"></exception>
            public static async Task OpenInviteDialog()
            {
                if (!_ready) throw new InvalidOperationException("Tried to use a command without being ready");

                if (_mock && !_configuration!.DisableDissonityInfoLogs)
                {
                    Utils.DissonityLog("Invite dialog sent");
                    return;
                }

                await SendCommand<OpenInviteDialog, NoResponse>(new ());
            }


            /// <summary>
            /// Presents a modal dialog to share media to a channel or direct message. <br/> <br/>
            /// No scopes required. <br/>
            /// ---------------------- <br/>
            /// ✅ | Web <br/>
            /// ⛔️ | iOS <br/>
            /// ⛔️ | Android <br/>
            /// ---------------------- <br/>
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            /// <exception cref="CommandException"></exception>
            public static async Task OpenShareMomentDialog(string mediaUrl)
            {
                if (!_ready) throw new InvalidOperationException("Tried to use a command without being ready");

                if (_mock && !_configuration!.DisableDissonityInfoLogs)
                {
                    if (Platform == Models.Platform.Desktop) Utils.DissonityLog($"Share moment dialog with ({mediaUrl}) sent");
                    else Utils.DissonityLogWarning("Platform is mobile, not possible to open a share moment dialog");
                    
                    return;
                }

                await SendCommand<OpenShareMomentDialog, NoResponse>(new (mediaUrl));
            }


            /// <summary>
            /// Modifies how your activity's rich presence is displayed in the Discord client. <br/> <br/>
            /// Scopes required: <c> rpc.activities.write </c> <br/>
            /// ---------------------- <br/>
            /// ✅ | Web <br/>
            /// ✅ | iOS <br/>
            /// ✅ | Android <br/>
            /// ---------------------- <br/>
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            /// <exception cref="CommandException"></exception>
            public static async Task<Activity> SetActivity(ActivityBuilder activity)
            {
                if (!_ready) throw new InvalidOperationException("Tried to use a command without being ready");
                
                if (_mock)
                {
                    //? Invalid scopes
                    if (!_configuration!.OauthScopes.Contains(OauthScope.RpcActivitiesWrite))
                    {
                        if (!_configuration!.DisableDissonityInfoLogs) Utils.DissonityLogWarning("Oauth scopes don't include 'rpc.activities.write'. Can't set the activity");
                    
                        throw new CommandException("Invalid oauth scopes inside mock", (int) RpcErrorCode.InvalidPermissions);
                    }

                    if (!_configuration!.DisableDissonityInfoLogs) Utils.DissonityLog("Set activity sent");

                    var mockResponse = await MockSendCommand<SetActivityResponse>(activity);

                    return mockResponse.Data;
                }

                var response = await SendCommand<SetActivity, SetActivityResponse>(new (activity));

                return response.Data;
            }


            /// <summary>
            /// Set whether or not the PIP (picture-in-picture) is interactive. <br/> <br/>
            /// No scopes required. <br/>
            /// ---------------------- <br/>
            /// ✅ | Web <br/>
            /// ⛔ | iOS <br/>
            /// ⛔ | Android <br/>
            /// ---------------------- <br/>
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            /// <exception cref="CommandException"></exception>
            public static async Task<SetConfigData> SetConfig(bool useInteractivePip)
            {
                if (!_ready) throw new InvalidOperationException("Tried to use a command without being ready");

                if (_mock)
                {
                    var mockResponse = await MockSendCommand<SetConfigResponse>();

                    if (Platform == Models.Platform.Mobile)
                    {
                        mockResponse.Data.UseInteractivePip = true;

                        if (!_configuration!.DisableDissonityInfoLogs) Utils.DissonityLogWarning("Platform is mobile, not possible to set PIP");
                    }

                    return mockResponse.Data;
                }

                var response = await SendCommand<SetConfig, SetConfigResponse>(new (useInteractivePip));

                return response.Data;
            }


            /// <summary>
            /// Locks the application to specific orientations in each of the supported layout modes. <br/> <br/>
            /// No scopes required. <br/>
            /// ---------------------- <br/>
            /// ⛔️ | Web <br/>
            /// ✅ | iOS <br/>
            /// ✅ | Android <br/>
            /// ---------------------- <br/>
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            /// <exception cref="CommandException"></exception>
            public static async Task SetOrientationLockState(OrientationLockStateType lockState)
            {
                if (!_ready) throw new InvalidOperationException("Tried to use a command without being ready");

                if (_mock && !_configuration!.DisableDissonityInfoLogs)
                {
                    if (Platform == Models.Platform.Mobile) Utils.DissonityLog($"Set orientation lock state to ({lockState})");
                    else Utils.DissonityLogWarning("Platform is desktop, not possible to set orientation lock state");
                    
                    return;
                }

                await SendCommand<SetOrientationLockState, NoResponse>(new (lockState));
            }


            /// <summary>
            /// Returns the current user's locale. <br/> <br/>
            /// Scopes required: <c> identify </c> <br/>
            /// ---------------------- <br/>
            /// ✅ | Web <br/>
            /// ✅ | iOS <br/>
            /// ✅ | Android <br/>
            /// ---------------------- <br/>
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            /// <exception cref="CommandException"></exception>
            public static async Task<UserSettingsGetLocaleData> UserSettingsGetLocale()
            {
                if (!_ready) throw new InvalidOperationException("Tried to use a command without being ready");

                if (_mock)
                {
                    //? Invalid scopes
                    if (!_configuration!.OauthScopes.Contains(OauthScope.Identify))
                    {
                        if (!_configuration!.DisableDissonityInfoLogs) Utils.DissonityLogWarning("Oauth scopes don't include 'identify'. Can't get user locale");
                    
                        throw new CommandException("Invalid oauth scopes inside mock", (int) RpcErrorCode.InvalidPermissions);
                    }

                    var mockResponse = await MockSendCommand<UserSettingsGetLocaleResponse>();

                    return mockResponse.Data;
                }

                var response = await SendCommand<UserSettingsGetLocale, UserSettingsGetLocaleResponse>(new ());

                return response.Data;
            }


            /// <summary>
            /// Presents the file upload flow in the Discord client. <br/> <br/>
            /// No scopes required. <br/>
            /// ---------------------- <br/>
            /// ✅ | Web <br/>
            /// ✅ | iOS <br/>
            /// ✅ | Android <br/>
            /// ---------------------- <br/>
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            /// <exception cref="CommandException"></exception>
            public static async Task<InitiateImageUploadData> InitiateImageUpload()
            {
                if (!_ready) throw new InvalidOperationException("Tried to use a command without being ready");

                if (_mock)
                {
                    var mockResponse = await MockSendCommand<InitiateImageUploadResponse>();

                    return mockResponse.Data;
                }

                var response = await SendCommand<InitiateImageUpload, InitiateImageUploadResponse>(new ());

                return response.Data;
            }


            /// <summary>
            /// Returns all participants connected to the instance. <br/> <br/>
            /// No scopes required. <br/>
            /// ---------------------- <br/>
            /// ✅ | Web <br/>
            /// ✅ | iOS <br/>
            /// ✅ | Android <br/>
            /// ---------------------- <br/>
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            /// <exception cref="CommandException"></exception>
            public static async Task<Participant[]> GetInstanceConnectedParticipants()
            {
                if (!_ready) throw new InvalidOperationException("Tried to use a command without being ready");

                if (_mock)
                {
                    var mockResponse = await MockSendCommand<GetInstanceConnectedParticipantsResponse>();

                    return mockResponse.Data.Participants;
                }

                var response = await SendCommand<GetInstanceConnectedParticipants, GetInstanceConnectedParticipantsResponse>(new ());

                return response.Data.Participants;
            }
        }

        //# PROXY - - - - -
        public static class Proxy
        {
            // POST - - - - -
            /// <summary>
            /// Sends an HTTPS post request with a JSON payload to the Discord proxy. <br/> <br/>
            /// You can pass a relative <c> /cat </c> or absolute <c> https://... </c> path. <br/> <br/>
            /// <c> ⚠️ </c> If you use an absolute path, you'll need to patch the url mappings so the request goes through the proxy.
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            /// <exception cref="OutsideDiscordException"></exception>
            /// <exception cref="WebException"></exception>
            /// <exception cref="JsonException"></exception>
            public static Task<TJsonResponse> HttpsPostRequest<TJsonRequest, TJsonResponse>(string path, TJsonRequest payload, Dictionary<string, string>? headers = null)
            {
                if (!_ready) throw new InvalidOperationException("Tried to make a proxy request without being ready");

                if (isEditor && !path.ToLower().StartsWith("http")) throw new OutsideDiscordException("You can't make relative requests to the Discord proxy while inside Unity");

                string uri = GetFormattedUri(path);

                var tcs = new TaskCompletionSource<TJsonResponse>();
                
                bridge!.StartCoroutine( SendPostRequest(uri, payload, tcs, headers) );

                return tcs.Task;
            }


            // GET - - - - -
            /// <summary>
            /// Sends an HTTPS get request to the Discord proxy. <br/> <br/>
            /// You can pass a relative <c> /cat </c> or absolute <c> https://... </c> path. <br/> <br/>
            /// <c> ⚠️ </c> If you use an absolute path, you'll need to patch the url mappings so the request goes through the proxy.
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            /// <exception cref="OutsideDiscordException"></exception>
            /// <exception cref="WebException"></exception>
            /// <exception cref="JsonException"></exception>
            public static Task<TJsonResponse> HttpsGetRequest<TJsonResponse>(string path, Dictionary<string, string>? headers = null)
            {
                if (!_ready) throw new InvalidOperationException("Tried to make a proxy request without being ready");

                if (isEditor && !path.ToLower().StartsWith("http")) throw new OutsideDiscordException("You can't make relative requests to the Discord proxy while inside Unity");

                string uri = GetFormattedUri(path);

                var tcs = new TaskCompletionSource<TJsonResponse>();
                
                bridge!.StartCoroutine( SendGetRequest(uri, tcs, headers) );

                return tcs.Task;
            }


            // PATCH - - - - -
            /// <summary>
            /// Sends an HTTPS patch request with a JSON payload to the Discord proxy. <br/> <br/>
            /// You can pass a relative <c> /cat </c> or absolute <c> https://... </c> path. <br/> <br/>
            /// <c> ⚠️ </c> If you use an absolute path, you'll need to patch the url mappings so the request goes through the proxy.
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            /// <exception cref="OutsideDiscordException"></exception>
            /// <exception cref="WebException"></exception>
            /// <exception cref="JsonException"></exception>
            public static Task<TJsonResponse> HttpsPatchRequest<TJsonRequest, TJsonResponse>(string path, TJsonRequest payload, Dictionary<string, string>? headers = null)
            {
                if (!_ready) throw new InvalidOperationException("Tried to make a proxy request without being ready");

                if (isEditor && !path.ToLower().StartsWith("http")) throw new OutsideDiscordException("You can't make relative requests to the Discord proxy while inside Unity");

                string uri = GetFormattedUri(path);

                var tcs = new TaskCompletionSource<TJsonResponse>();
                
                bridge!.StartCoroutine( SendPatchRequest(uri, payload, tcs, headers) );

                return tcs.Task;
            }


            // PUT - - - - -
            /// <summary>
            /// Sends an HTTPS put request with a JSON payload to the Discord proxy. <br/> <br/>
            /// You can pass a relative <c> /cat </c> or absolute <c> https://... </c> path. <br/> <br/>
            /// <c> ⚠️ </c> If you use an absolute path, you'll need to patch the url mappings so the request goes through the proxy.
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            /// <exception cref="OutsideDiscordException"></exception>
            /// <exception cref="WebException"></exception>
            /// <exception cref="JsonException"></exception>
            public static Task<TJsonResponse> HttpsPutRequest<TJsonRequest, TJsonResponse>(string path, TJsonRequest payload, Dictionary<string, string>? headers = null)
            {
                if (!_ready) throw new InvalidOperationException("Tried to make a proxy request without being ready");

                if (isEditor && !path.ToLower().StartsWith("http")) throw new OutsideDiscordException("You can't make relative requests to the Discord proxy while inside Unity");

                string uri = GetFormattedUri(path);

                var tcs = new TaskCompletionSource<TJsonResponse>();
                
                bridge!.StartCoroutine( SendPutRequest(uri, payload, tcs, headers) );

                return tcs.Task;
            }


            // DELETE - - - - -
            /// <summary>
            /// Sends an HTTPS delete request to the Discord proxy. <br/> <br/>
            /// You can pass a relative <c> /cat </c> or absolute <c> https://... </c> path. <br/> <br/>
            /// <c> ⚠️ </c> If you use an absolute path, you'll need to patch the url mappings so the request goes through the proxy.
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            /// <exception cref="OutsideDiscordException"></exception>
            /// <exception cref="WebException"></exception>
            /// <exception cref="JsonException"></exception>
            public static Task<TJsonResponse> HttpsDeleteRequest<TJsonResponse>(string path, Dictionary<string, string>? headers = null)
            {
                if (!_ready) throw new InvalidOperationException("Tried to make a proxy request without being ready");

                if (isEditor && !path.ToLower().StartsWith("http")) throw new OutsideDiscordException("You can't make relative requests to the Discord proxy while inside Unity");

                string uri = GetFormattedUri(path);

                var tcs = new TaskCompletionSource<TJsonResponse>();
                
                bridge!.StartCoroutine( SendDeleteRequest(uri, tcs, headers) );

                return tcs.Task;
            }

            private static string GetFormattedUri(string path)
            {
                string uri;

                if (!path.ToLower().StartsWith("http"))
                {
                    path = path.StartsWith("/")
                        ? path
                        : $"/{path}";

                    uri = path.StartsWith("/.proxy/")
                        ? $"https://{_clientId}.{ProxyDomain}{path}"
                        : $"https://{_clientId}.{ProxyDomain}/.proxy{path}";
                }
                else
                {
                    uri = path;
                }

                return uri;
            }

            private static IEnumerator SendPostRequest<TJsonRequest, TJsonResponse>(string uri, TJsonRequest payload, TaskCompletionSource<TJsonResponse> tcs, Dictionary<string, string>? headers = null)
            {
                UnityWebRequest request = UnityWebRequest.Post(uri, JsonConvert.SerializeObject(payload), "application/json");

                if (headers != null)
                {
                    foreach (var pair in headers)
                    {
                        request.SetRequestHeader(pair.Key, pair.Value);
                    }
                }

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError || request.result == UnityWebRequest.Result.DataProcessingError)
                {
                    tcs.TrySetException(new WebException("An error occurred in a request to the proxy"));
                }

                else
                {
                    string stringifiedData = request.downloadHandler.text;

                    var data = JsonConvert.DeserializeObject<TJsonResponse>(stringifiedData);

                    if (data == null) throw new JsonException("Couldn't parse server response as JSON");

                    tcs.TrySetResult(data);
                }
            }
        
            private static IEnumerator SendGetRequest<TJsonResponse>(string uri, TaskCompletionSource<TJsonResponse> tcs, Dictionary<string, string>? headers = null)
            {
                UnityWebRequest request = UnityWebRequest.Get(uri);

                if (headers != null)
                {
                    foreach (var pair in headers)
                    {
                        request.SetRequestHeader(pair.Key, pair.Value);
                    }
                }

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError || request.result == UnityWebRequest.Result.DataProcessingError)
                {
                    tcs.TrySetException(new WebException("An error occurred in a request to the proxy"));
                }

                else
                {
                    string stringifiedData = request.downloadHandler.text;

                    var data = JsonConvert.DeserializeObject<TJsonResponse>(stringifiedData);

                    if (data == null) throw new JsonException("Couldn't parse server response as JSON");

                    tcs.TrySetResult(data);
                }
            }
        
            private static IEnumerator SendPatchRequest<TJsonRequest, TJsonResponse>(string uri, TJsonRequest payload, TaskCompletionSource<TJsonResponse> tcs, Dictionary<string, string>? headers = null)
            {
                UnityWebRequest request = new UnityWebRequest(uri, "PATCH");

                request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload)));
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                if (headers != null)
                {
                    foreach (var pair in headers)
                    {
                        request.SetRequestHeader(pair.Key, pair.Value);
                    }
                }

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError || request.result == UnityWebRequest.Result.DataProcessingError)
                {
                    tcs.TrySetException(new WebException("An error occurred in a request to the proxy"));
                }

                else
                {
                    string stringifiedData = request.downloadHandler.text;

                    var data = JsonConvert.DeserializeObject<TJsonResponse>(stringifiedData);

                    if (data == null) throw new JsonException("Couldn't parse server response as JSON");

                    tcs.TrySetResult(data);
                }
            }
        
            private static IEnumerator SendPutRequest<TJsonRequest, TJsonResponse>(string uri, TJsonRequest payload, TaskCompletionSource<TJsonResponse> tcs, Dictionary<string, string>? headers = null)
            {
                UnityWebRequest request = new UnityWebRequest(uri, "PUT");

                request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload)));
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                if (headers != null)
                {
                    foreach (var pair in headers)
                    {
                        request.SetRequestHeader(pair.Key, pair.Value);
                    }
                }

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError || request.result == UnityWebRequest.Result.DataProcessingError)
                {
                    tcs.TrySetException(new WebException("An error occurred in a request to the proxy"));
                }

                else
                {
                    string stringifiedData = request.downloadHandler.text;

                    var data = JsonConvert.DeserializeObject<TJsonResponse>(stringifiedData);

                    if (data == null) throw new JsonException("Couldn't parse server response as JSON");

                    tcs.TrySetResult(data);
                }
            }
        
            private static IEnumerator SendDeleteRequest<TJsonResponse>(string uri, TaskCompletionSource<TJsonResponse> tcs, Dictionary<string, string>? headers = null)
            {
                UnityWebRequest request = new UnityWebRequest(uri, "DELETE");
                request.downloadHandler = new DownloadHandlerBuffer();

                if (headers != null)
                {
                    foreach (var pair in headers)
                    {
                        request.SetRequestHeader(pair.Key, pair.Value);
                    }
                }

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError || request.result == UnityWebRequest.Result.DataProcessingError)
                {
                    tcs.TrySetException(new WebException("An error occurred in a request to the proxy"));
                }

                else
                {
                    string stringifiedData = request.downloadHandler.text;

                    var data = JsonConvert.DeserializeObject<TJsonResponse>(stringifiedData);

                    if (data == null) throw new JsonException("Couldn't parse server response as JSON");

                    tcs.TrySetResult(data);
                }
            }
        }

        //# SUBSCRIBE - - - -
        public static class Subscribe
        {
            //* SubscribeCommandFactory handles mock mode

            /// <summary>
            /// Received when the number of instance participants changes. <br/> <br/>
            /// No scopes required.
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            /// <exception cref="CommandException"></exception>
            public static async Task<SubscriptionReference> ActivityInstanceParticipantsUpdate(Action<Participant[]> listener)
            {
                if (!_ready) throw new InvalidOperationException("Tried to subscribe without being ready");

                var reference = await SubscribeCommandFactory<ActivityInstanceParticipantsUpdate>(new MessageBusReaderIndefinite(listener, (discordEvent) =>
                {
                    if (discordEvent is not ActivityInstanceParticipantsUpdate _) return;

                    var data = (ActivityInstanceParticipantsUpdateData) discordEvent.Data;

                    listener(data.Participants);
                }));

                return reference;
            }


            /// <summary>
            /// Received when a user changes the layout mode in the Discord client. <br/> <br/>
            /// No scopes required.
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            /// <exception cref="CommandException"></exception>
            public static async Task<SubscriptionReference> ActivityLayoutModeUpdate(Action<LayoutModeType> listener)
            {
                if (!_ready) throw new InvalidOperationException("Tried to subscribe without being ready");

                var reference = await SubscribeCommandFactory<ActivityLayoutModeUpdate>(new MessageBusReaderIndefinite(listener, (discordEvent) =>
                {
                    if (discordEvent is not ActivityLayoutModeUpdate _) return;

                    var data = (ActivityLayoutModeUpdateData) discordEvent.Data;

                    listener(data.LayoutMode);
                }));

                return reference;
            }


            /// <summary>
            /// Received when the current user object changes. <br/> <br/>
            /// Scopes required: <c> identify </c> <br/>
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            /// <exception cref="CommandException"></exception>
            public static async Task<SubscriptionReference> CurrentUserUpdate(Action<User> listener)
            {
                if (!_ready) throw new InvalidOperationException("Tried to subscribe without being ready");

                if (_mock)
                {
                    //? Invalid scopes
                    if (!_configuration!.OauthScopes.Contains(OauthScope.Identify))
                    {
                        if (!_configuration!.DisableDissonityInfoLogs) Utils.DissonityLogWarning("Oauth scopes don't include 'identify'. Can't subscribe to Current User Update");
                    
                        throw new CommandException("Invalid oauth scopes inside mock", (int) RpcErrorCode.InvalidPermissions);
                    }
                }

                var reference = await SubscribeCommandFactory<CurrentUserUpdate, User>(listener);

                return reference;
            }


            //todo Currently not documented so scopes might be wrong... That's gonna take a while to get merged https://github.com/discord/discord-api-docs/pull/7130
            /// <summary>
            /// Received when the current guild member object changes. <br/> <br/>
            /// Scopes required: <c> identify </c>, <c> guilds.members.read </c> <br/>
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            /// <exception cref="CommandException"></exception>
            public static async Task<SubscriptionReference> CurrentGuildMemberUpdate(long guildId, Action<GuildMemberRpc> listener)
            {
                if (!_ready) throw new InvalidOperationException("Tried to subscribe without being ready");

                if (_mock)
                {
                    //? Invalid scopes
                    if (!_configuration!.OauthScopes.Contains(OauthScope.Identify) || !_configuration!.OauthScopes.Contains(OauthScope.GuildsMembersRead))
                    {
                        if (!_configuration!.DisableDissonityInfoLogs) Utils.DissonityLogWarning("Oauth scopes don't include 'identify' and 'guilds.members.read'. Can't subscribe to Current Guild Member Update");
                    
                        throw new CommandException("Invalid oauth scopes inside mock", (int) RpcErrorCode.InvalidPermissions);
                    }
                }

                var reference = await SubscribeCommandFactory<CurrentGuildMemberUpdate, GuildMemberRpc>(listener, new EventArguments{ GuildId = guildId.ToString() });

                return reference;
            }


            //todo Also not documented yet
            /// <summary>
            /// Received when an entitlement is created for a SKU <br/> <br/>
            /// No scopes required
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            /// <exception cref="CommandException"></exception>
            public static async Task<SubscriptionReference> EntitlementCreate(Action<Entitlement> listener)
            {
                if (!_ready) throw new InvalidOperationException("Tried to subscribe without being ready");

                var reference = await SubscribeCommandFactory<EntitlementCreate>(new MessageBusReaderIndefinite(listener, (discordEvent) =>
                {
                    if (discordEvent is not EntitlementCreate _) return;

                    var data = (EntitlementCreateData) discordEvent.Data;

                    listener(data.Entitlement);
                }));

                return reference;
            }


            /// <summary>
            /// Non-subscription event sent when there is an error, including command responses. <br/> <br/>
            /// No scopes required.
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            /// <exception cref="CommandException"></exception>
            public static SubscriptionReference Error(Action<ErrorEventData> listener)
            {
                if (!_ready) throw new InvalidOperationException("Tried to subscribe without being ready");

                //\ Create reader
                var reader = new MessageBusReaderIndefinite(listener, (discordEvent) =>
                {
                    if (discordEvent is not ErrorEvent _) return;

                    listener((ErrorEventData) discordEvent.Data!);
                });

                //\ Create reference
                var reference = new SubscriptionReference();
                reference.SaveSubscriptionData(reader, DiscordEventType.Error);

                //\ Save reader in message bus
                messageBus.AddReader(DiscordEventType.Error, reader);

                return reference;
            }


            /// <summary>
            /// Received when screen orientation changes. <br/> <br/>
            /// No scopes required.
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            /// <exception cref="CommandException"></exception>
            public static async Task<SubscriptionReference> OrientationUpdate(Action<OrientationType> listener)
            {
                if (!_ready) throw new InvalidOperationException("Tried to subscribe without being ready");

                var reference = await SubscribeCommandFactory<OrientationUpdate>(new MessageBusReaderIndefinite(listener, (discordEvent) =>
                {
                    if (discordEvent is not OrientationUpdate _) return;

                    var data = (OrientationUpdateData) discordEvent.Data;

                    listener(data.ScreenOrientation);
                }));

                return reference;
            }


            /// <summary>
            /// Received when a user in a subscribed voice channel speaks. <br/> <br/>
            /// Scopes required: <c> rpc.voice.read </c> <br/>
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            /// <exception cref="CommandException"></exception>
            public static async Task<SubscriptionReference> SpeakingStart(long channelId, Action<SpeakingData> listener)
            {
                if (!_ready) throw new InvalidOperationException("Tried to subscribe without being ready");

                if (_mock)
                {
                    //? Invalid scopes
                    if (!_configuration!.OauthScopes.Contains(OauthScope.RpcVoiceRead))
                    {
                        if (!_configuration!.DisableDissonityInfoLogs) Utils.DissonityLogWarning("Oauth scopes don't include 'rpc.voice.read'. Can't subscribe to Speaking Start");
                    
                        throw new CommandException("Invalid oauth scopes inside mock", (int) RpcErrorCode.InvalidPermissions);
                    }
                }

                var reference = await SubscribeCommandFactory<SpeakingStart, SpeakingData>(listener, new EventArguments{ ChannelId = channelId.ToString() });

                return reference;
            }


            /// <summary>
            /// Received when a user in a subscribed voice channel stops speaking. <br/> <br/>
            /// Scopes required: <c> rpc.voice.read </c> <br/>
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            /// <exception cref="CommandException"></exception>
            public static async Task<SubscriptionReference> SpeakingStop(long channelId, Action<SpeakingData> listener)
            {
                if (!_ready) throw new InvalidOperationException("Tried to subscribe without being ready");

                if (_mock)
                {
                    //? Invalid scopes
                    if (!_configuration!.OauthScopes.Contains(OauthScope.RpcVoiceRead))
                    {
                        if (!_configuration!.DisableDissonityInfoLogs) Utils.DissonityLogWarning("Oauth scopes don't include 'rpc.voice.read'. Can't subscribe to Speaking Stop");
                    
                        throw new CommandException("Invalid oauth scopes inside mock", (int) RpcErrorCode.InvalidPermissions);
                    }
                }

                var reference = await SubscribeCommandFactory<SpeakingStop, SpeakingData>(listener, new EventArguments{ ChannelId = channelId.ToString() });

                return reference;
            }


            /// <summary>
            /// Received when Android or iOS thermal states are surfaced to the Discord mobile app. <br/> <br/>
            /// No scopes required
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            /// <exception cref="CommandException"></exception>
            public static async Task<SubscriptionReference> ThermalStateUpdate(Action<ThermalStateType> listener)
            {
                if (!_ready) throw new InvalidOperationException("Tried to subscribe without being ready");

                var reference = await SubscribeCommandFactory<ThermalStateUpdate>(new MessageBusReaderIndefinite(listener, (discordEvent) =>
                {
                    if (discordEvent is not ThermalStateUpdate _) return;

                    var data = (ThermalStateUpdateData) discordEvent.Data;

                    listener(data.ThermalState);
                }));

                return reference;
            }


            /// <summary>
            /// Received when a user's voice state changes in a subscribed voice channel (mute, volume, etc). <br/> <br/>
            /// Scopes required: <c> rpc.voice.read </c> <br/>
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            /// <exception cref="CommandException"></exception>
            public static async Task<SubscriptionReference> VoiceStateUpdate(long channelId, Action<UserVoiceState> listener)
            {
                if (!_ready) throw new InvalidOperationException("Tried to subscribe without being ready");

                if (_mock)
                {
                    //? Invalid scopes
                    if (!_configuration!.OauthScopes.Contains(OauthScope.RpcVoiceRead))
                    {
                        if (!_configuration!.DisableDissonityInfoLogs) Utils.DissonityLogWarning("Oauth scopes don't include 'rpc.voice.read'. Can't subscribe to Speaking Stop");
                    
                        throw new CommandException("Invalid oauth scopes inside mock", (int) RpcErrorCode.InvalidPermissions);
                    }
                }

                var reference = await SubscribeCommandFactory<VoiceStateUpdate, UserVoiceState>(listener, new EventArguments{ ChannelId = channelId.ToString() });

                return reference;
            }

            // Method used to simplify the subscription methods
            internal static async Task<SubscriptionReference> SubscribeCommandFactory<TEvent, TEventData>(Action<TEventData> listener, object? args = null, bool isInternal = false, bool once = false) where TEvent : DiscordEvent
            {
                string eventString = EventUtility.GetStringFromType(typeof(TEvent));

                //\ Create reader
                MessageBusReader reader;

                //? Once
                if (once)
                {
                    reader = new MessageBusReaderOnce(listener, (discordEvent) =>
                    {
                        if (discordEvent is not TEvent _) return;

                        listener((TEventData) discordEvent.Data!);
                    });
                }

                //? Indefinite
                else
                {
                    reader = new MessageBusReaderIndefinite(listener, (discordEvent) =>
                    {
                        if (discordEvent is not TEvent _) return;

                        listener((TEventData) discordEvent.Data!);
                    });
                }

                //\ Create reference
                var reference = new SubscriptionReference();
                reference.SaveSubscriptionData(reader, eventString);

                //\ Save reader in message bus
                messageBus.AddReader(eventString, reader, isInternal);

                //? Not subscribed to RPC
                if (!_mock && !subscribedRpcSet.Contains(eventString))
                {
                    subscribedRpcSet.Add(eventString);

                    //? Using args
                    if (args != null)
                    {
                        await SendCommand<Dissonity.Commands.Subscribe, SubscribeResponse>(new (eventString) { Args = args });
                    }

                    else
                    {
                        await SendCommand<Dissonity.Commands.Subscribe, SubscribeResponse>(new (eventString));
                    }

                    // Awaiting the subscribe command so that if it errors, the exception is raised properly
                }

                return reference;
            }
        
            // SubcribeCommandFactory but with a manual reader. Used to return <Event>.Data.<UniqueProperty>
            internal static async Task<SubscriptionReference> SubscribeCommandFactory<TEvent>(MessageBusReader reader, object? args = null, bool isInternal = false) where TEvent : DiscordEvent
            {
                string eventString = EventUtility.GetStringFromType(typeof(TEvent));

                //\ Create reference
                var reference = new SubscriptionReference();
                reference.SaveSubscriptionData(reader, eventString);

                //\ Save reader in message bus
                messageBus.AddReader(eventString, reader, isInternal);

                //? Not subscribed to RPC
                if (!_mock && !subscribedRpcSet.Contains(eventString))
                {
                    subscribedRpcSet.Add(eventString);

                    //? Using args
                    if (args != null)
                    {
                        await SendCommand<Dissonity.Commands.Subscribe, SubscribeResponse>(new (eventString) { Args = args });
                    }

                    else
                    {
                        await SendCommand<Dissonity.Commands.Subscribe, SubscribeResponse>(new (eventString));
                    }

                    // Awaiting the subscribe command so that if it errors, the exception is raised properly
                }

                return reference;
            }
        }


        //# METHODS - - - - -
        /// <summary>
        /// Initializes Dissonity. You must call and await this method once before doing anything else.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="OutsideDiscordException"></exception>
        /// <exception cref="JsonException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static Task<MultiEvent> Initialize()
        {
            if (_initialized) throw new InvalidOperationException("Already attempted to initialize");
            _initialized = true;

            _configuration = DissonityConfigAttribute.GetUserConfig();
            _clientId = _configuration.ClientId;

            //\ Create bridge instance
            bridgeObject = new GameObject("_DissonityBridge");
            bridge = bridgeObject.AddComponent<DissonityBridge>();
            GameObject.DontDestroyOnLoad(bridgeObject);

            //\ Prepare initialization task
            var tcs = new TaskCompletionSource<MultiEvent>();

            //? Not running inside Discord
            if (isEditor)
            {
                if (_mock)
                {
                    //\ Query data
                    var mock = GameObject.FindAnyObjectByType<DiscordMock>();

                    //? No mock
                    if (!mock)
                    {
                        throw new InvalidOperationException("Mock mode is enabled but there's no DiscordMock object. Make sure to create one to access mock data.");
                    }

                    _ready = true;

                    return Task.FromResult(new MockMultiEvent().ToMultiEvent());
                }

                else if (!_configuration.DisableDissonityInfoLogs)
                {
                    Utils.DissonityLogWarning("Running inside the Unity editor. You may debug using the Discord Mock object (Right click hierarchy > Dissonity > Discord Mock).");
                }

                throw new OutsideDiscordException();
            }

            // Mock is invalid here
            _mock = false;
            disableMock = true;

            UnityEngine.Application.runInBackground = true;

            LoadInterface();

            async void HandleInteraction()
            {
                string query = await bridge!.ExeQuery();

                InitializeQuery(tcs, query);
            }

            HandleInteraction();

            return tcs.Task;
        }


        /// <summary>
        /// Use this method to easily access external resources.
        /// If you need to use it before initialization, consider using <c> Mappings </c> and <c> PatchUrlMappingsConfig </c> from the <c> DissonityConfig </c> instead. <br/> <br/>
        /// https://discord.com/developers/docs/activities/development-guides#using-external-resources
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public static Task PatchUrlMappings(MappingBuilder[] mappings, PatchUrlMappingsConfigBuilder? config = null)
        {
            if (!_ready) throw new InvalidOperationException("Tried to use a method without being ready");

            if (config == null)
            {
                config = _configuration!.PatchUrlMappingsConfig;
            }

            var tcs = new TaskCompletionSource<bool>();

            if (_mock)
            {
                if (!_configuration!.DisableDissonityInfoLogs) Utils.DissonityLog("Mock patch url mappings called via API method");

                tcs.TrySetResult(true);
                return tcs.Task;
            }

            async void HandleInteraction()
            {
                var payload = new PatchUrlMappings()
                {
                    Mappings = mappings,
                    Config = config
                };

                await bridge!.ExePatchUrlMappings(payload);

                tcs.TrySetResult(true);
            }

            HandleInteraction();

            return tcs.Task;
        }


        /// <summary>
        /// Use this method to easily render SKU prices. When called in the Unity Editor, uses a generic currency symbol <c> ¤ </c>.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public static Task<string> FormatPrice(SkuPrice price, string locale = Locale.UsEnglish)
        {
            if (!_ready) throw new InvalidOperationException("Tried to use a method without being ready");

            var tcs = new TaskCompletionSource<string>();

            if (_mock)
            {
                CultureInfo culture = new CultureInfo(locale);
                NumberFormatInfo numberFormat = culture.NumberFormat;

                numberFormat.CurrencySymbol = "¤";

                double number = price.Amount / 100;
                string result = number.ToString("C", numberFormat);

                //\ Complete task
                tcs.TrySetResult(result);

                return tcs.Task;
            } 

            async void HandleInteraction()
            {
                var payload = new FormatPrice()
                {
                    Amount = price.Amount,
                    Currency = price.Currency,
                    Locale = locale
                };

                string result = await bridge!.ExeFormatPrice(payload);

                tcs.TrySetResult(result);
            }

            HandleInteraction();

            return tcs.Task;
        }


        //# UNSUBSCRIBE - - - - -
        /// <summary>
        /// Remove a subscription via a SubscriptionReference instance (returned by subscription methods).
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static void Unsubscribe(SubscriptionReference reference)
        {
            if (!_ready) throw new InvalidOperationException("Tried to unsubscribe without being ready");
            
            if (!messageBus.ReaderSetDictionary.ContainsKey(reference.EventString)) throw new ArgumentException($"Tried to unsubscribe from a SubscriptionReference that no longer exists ({reference.EventString})");

            var readerSet = messageBus.ReaderSetDictionary[reference.EventString];

            //? Reader set has reader
            if (!readerSet.Contains(reference.Reader)) throw new ArgumentException("Tried to unsubscribe from a SubscriptionReference that no longer exists");
        
            bool setIsGone = messageBus.RemoveReader(reference.EventString, reference.Reader);

            //? Should unsubscribe from RPC
            if (setIsGone && !_mock)
            {
                subscribedRpcSet.Remove(reference.EventString);
                SendCommand<Unsubscribe, SubscribeResponse>(new (reference.EventString));
            }
        }


        /// <summary>
        /// Remove a subscription via the event return data type and a method.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static void Unsubscribe<T>(Action<T> listener)
        {
            string eventString = EventUtility.GetStringFromReturnDataType(typeof(T));
            
            if (!messageBus.ReaderSetDictionary.ContainsKey(eventString)) throw new ArgumentException($"Tried to unsubscribe from an event you're not subscribed to ({eventString})");

            var readerSet = messageBus.ReaderSetDictionary[eventString];

            //? Reader set has reader
            var reader = readerSet.FirstOrDefault(r => (Action<T>) r.UserListener == listener);
            if (reader == null) throw new ArgumentException($"Tried to unsubscribe from a listener that no longer exists");
        
            bool setIsGone = messageBus.RemoveReader(eventString, reader);

            //? Should unsubscribe from RPC
            if (setIsGone && !_mock)
            {
                subscribedRpcSet.Remove(eventString);
                SendCommand<Unsubscribe, SubscribeResponse>(new (eventString));
            }
        }


        /// <summary>
        /// Remove all subscriptions related to a single event.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static void UnsubscribeFromEvent(string eventString)
        {
            if (!_ready) throw new InvalidOperationException("Tried to unsubscribe without being ready");
            
            if (!messageBus.ReaderSetDictionary.ContainsKey(eventString)) throw new ArgumentException($"Tried to unsubscribe from an event that had no subscriptions ({eventString})");

            messageBus.ReaderSetDictionary.Remove(eventString);

            //\ Unsubscribe from RPC
            if (_mock || messageBus.ReaderSetExists(eventString)) return;

            subscribedRpcSet.Remove(eventString);
            SendCommand<Unsubscribe, SubscribeResponse>(new (eventString));
        }


        /// <summary>
        /// Remove all subscriptions from every event.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public static void ClearAllSubscriptions()
        {
            if (!_ready) throw new InvalidOperationException("Tried to unsubscribe without being ready");

            messageBus.ReaderSetDictionary.Clear();

            if (_mock) return;

            foreach (string key in subscribedRpcSet)
            {
                if (messageBus.ReaderSetExists(key)) continue;

                subscribedRpcSet.Remove(key);

                SendCommand<Unsubscribe, SubscribeResponse>(new (key));
            }
        }


        //# CLOSE - - - - -
        /// <summary>
        /// Close the app with a specified code and reason.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public static void Close(RpcCloseCode code, string message = "")
        {
            if (!_ready) throw new InvalidOperationException("Tried to close the app without being ready");

            if (!_mock)
            {
                SendToBridge<Close, NoResponse>(new Close(code, message));
                StopListening();
            }

            messageBus.ReaderSetDictionary.Clear();
            subscribedRpcSet.Clear();
            GameObject.Destroy(bridge);
            _ready = false;

            if (_mock && !_configuration!.DisableDissonityInfoLogs)
            {
                Utils.DissonityLog($"Embedded app closed with code {code} and message '{message}'");
            }
        }


        //# PRIVATE METHODS - - - - -
        private static void InitializeQuery(TaskCompletionSource<MultiEvent> tcs, string stringifiedQuery)
        {
            var query = JsonConvert.DeserializeObject<QueryData>(stringifiedQuery);

            if (query == null)
            {
                tcs.TrySetException(new JsonException("Something went wrong attempting to read the query"));
                return;
            }

            //# QUERY PARAMS - - - - -
            // Frame id
            if (query.FrameId == null)
            {
                // RpcBridge state should only be OutsideDiscord here

                tcs.TrySetException(new OutsideDiscordException("'frame_id' query param is not defined - Running outside of Discord or too nested to access query. Initialization is canceled."));
                return;
            }
            _frameId = query.FrameId;

            // Instance id
            if (query.InstanceId == null)
            {
                tcs.TrySetException(new OutsideDiscordException("instance_id query param is not defined"));
                return;
            }
            _instanceId = query.InstanceId;

            // Platform
            if (query.Platform == null)
            {
                tcs.TrySetException(new OutsideDiscordException("platform query param is not defined"));
                return;
            }
            else if (query.Platform != Models.Platform.Desktop && query.Platform != Models.Platform.Mobile)
            {
                tcs.TrySetException(new ArgumentException($"Invalid query param 'platform' of '{query.Platform}'. Valid values are '{Models.Platform.Desktop}' or '{Models.Platform.Mobile}'"));
                return;
            }
            _platform = query.Platform;

            // Guild id
            if (query.GuildId != null)
            {
                _guildId = query.GuildId;
            }
            
            // Channel id
            if (query.ChannelId != null)
            {
                _channelId = query.ChannelId;
            }

            // Mobile app version
            if (query.MobileAppVersion != null)
            {
                _mobileAppVersion = query.MobileAppVersion;
            }

            //\ Bridge interactions
            async void HandleState()
            {
                var code = await bridge!.ExeState();

                if (!_configuration!.DisableDissonityInfoLogs)
                {
                    //? Problem
                    if (code == BridgeStateCode.Errored || code == BridgeStateCode.OutsideDiscord)
                    {
                        Utils.DissonityLogError($"Bridge returned unexpected state code: {code}");
                    }

                    else Utils.DissonityLog($"Bridge returned state code: {code}");
                }
                
                //? OutsideDiscord
                if (code == BridgeStateCode.OutsideDiscord)
                {
                    tcs.TrySetException(new OutsideDiscordException("RpcBridge returned OutsideDiscord state"));
                }
            }

            async void HandleMulti()
            {
                var multiEvent = await bridge!.ExeMultiEvent();

                // OverrideConsoleLogging is done in the BridgeLib
                _userId = multiEvent.AuthenticateData.User.Id;
                _accessToken = multiEvent.AuthenticateData.AccessToken;
                
                //? Synchronize user
                if (_configuration!.SynchronizeUser)
                {

                    //? Invalid scopes
                    if (!_configuration!.OauthScopes.Contains(OauthScope.Identify))
                    {
                        if (!_configuration!.DisableDissonityInfoLogs) Utils.DissonityLogError("SynchronizeUser is enabled but there's no 'identify' scope");
                    }

                    else
                    {
                        var userTcs = new TaskCompletionSource<object?>();

                        // First event
                        await Subscribe.SubscribeCommandFactory<CurrentUserUpdate, User>(data =>
                        {
                            _user = data;
                            userTcs.SetResult(null);
                        }, null, true, true);

                        // Indefinite
                        await Subscribe.SubscribeCommandFactory<CurrentUserUpdate, User>(data =>
                        {
                            _user = data;
                        }, null, true);

                        await userTcs.Task;
                    }
                }

                //? Synchronize guild member RPC
                if (_configuration!.SynchronizeGuildMemberRpc)
                {

                    //? Invalid scopes
                    if (!_configuration!.OauthScopes.Contains(OauthScope.Identify) || !_configuration!.OauthScopes.Contains(OauthScope.GuildsMembersRead))
                    {
                        if (!_configuration!.DisableDissonityInfoLogs) Utils.DissonityLogError("SynchronizeGuildMemberRpc is enabled but there's no 'identify' or 'guilds.members.read' scope");
                    }

                    else
                    {
                        var memberTcs = new TaskCompletionSource<object?>();

                        // First event
                        await Subscribe.SubscribeCommandFactory<CurrentGuildMemberUpdate, GuildMemberRpc>(data =>
                        {
                            _guildMemberRpc = data;
                            memberTcs.SetResult(null);
                        }, new EventArguments { GuildId = _guildId.ToString() }, true, true);

                        // Indefinite
                        await Subscribe.SubscribeCommandFactory<CurrentGuildMemberUpdate, GuildMemberRpc>(data =>
                        {
                            _guildMemberRpc = data;
                        }, null, true);

                        await memberTcs.Task;
                    }
                }
            
                _ready = true;
                tcs.TrySetResult(multiEvent);
            }

            // After requesting the state, the RpcBridge will send the multi event (once ready) through <DissonityBridge>.ReceiveMultiEvent
            HandleMulti();
            HandleState();

            // The handshake is handled by the RpcBridge even before the Unity build loads
        }

        private static Task<TResponse> SendCommand<TCommand, TResponse>(TCommand command) where TCommand : DiscordCommand where TResponse : DiscordEvent
        {
            if (!_initialized)
            {
                throw new InvalidOperationException("Tried to send a command without being ready");
            }

            var tcs = new TaskCompletionSource<TResponse>();

            // Only the handshake command lacks a nonce
            string commandNonce = command.Guid.ToString();


            if (typeof(TResponse) == typeof(NoResponse))
            {
                // Seems hacky, but I don't think there's other way around
                // if this method is intended to serve the command response.
                ((TaskCompletionSource<NoResponse>) (object) tcs).TrySetResult(new NoResponse());
            }

            else
            {
                pendingCommands.Add(commandNonce, tcs);
            }

            SendToBridge<TCommand, TResponse>(command);

            return tcs.Task;
        }

        private static void SendToBridge<TCommand, TResponse>(TCommand command) where TCommand : DiscordCommand where TResponse : DiscordEvent
        {
            if (!_initialized)
            {
                throw new InvalidOperationException("Tried to send to bridge without being ready");
            }

            object payload;

            if (command is FrameCommand frameCommand)
            {
                payload = new SerializableFrameCommand(frameCommand);
            }
            else
            {
                payload = command;
            }

            string stringifiedMessage = JsonConvert.SerializeObject(new object[2] { command.Opcode, payload });

            if (!isEditor)
            {
                //\ Send data to bridge
                Send(stringifiedMessage);
            }
        }
        
        // This method takes longs directly, unlike SendCommand, that takes the stringified long
        private static Task<TResponse> MockSendCommand<TResponse>(object? arg = null) where TResponse : DiscordEvent
        {
            if (!_mock) throw new InvalidOperationException("This method can only be called in mock mode");
            if (!_initialized)
            {
                throw new InvalidOperationException("Tried to send a command without being ready");
            }

            var tcs = new TaskCompletionSource<TResponse>();
            DiscordMock mock = GameObject.FindAnyObjectByType<DiscordMock>();

            if (typeof(TResponse) == typeof(NoResponse))
            {
                ((TaskCompletionSource<NoResponse>) (object) tcs).TrySetResult(new NoResponse());
            }

            else
            {
                // EncourageHardwareAccelerationResponse
                if (typeof(TResponse) == typeof(EncourageHardwareAccelerationResponse))
                {
                    var response = new EncourageHardwareAccelerationResponse();

                    response.Data = new() 
                    {
                        Enabled = true
                    };

                    ((TaskCompletionSource<EncourageHardwareAccelerationResponse>) (object) tcs).TrySetResult(response);
                }

                // GetChannelPermissionsResponse
                else if (typeof(TResponse) == typeof(GetChannelPermissionsResponse))
                {
                    var response = new GetChannelPermissionsResponse();

                    response.Data = new();

                    //? Query channel id is an actual mock channel
                    long id = ChannelId!;
                    MockChannel? mockChannel = mock._channels.Find(c => c.Id == id);

                    if (mockChannel != null)
                    {
                        response.Data.Permissions = mockChannel.ChannelPermissions.Permissions;
                    }

                    ((TaskCompletionSource<GetChannelPermissionsResponse>) (object) tcs).TrySetResult(response);
                }

                // GetEntitlementsResponse
                else if (typeof(TResponse) == typeof(GetEntitlementsResponse))
                {
                    var response = new GetEntitlementsResponse();

                    response.Data = new()
                    {
                        Entitlements = mock.GetEntitlements()
                    };

                    ((TaskCompletionSource<GetEntitlementsResponse>) (object) tcs).TrySetResult(response);
                }

                // GetChannelResponse
                else if (typeof(TResponse) == typeof(GetChannelResponse))
                {
                    var response = new GetChannelResponse();

                    response.Data = new();

                    //? Channel id is an actual mock channel
                    MockChannel? mockChannel = mock._channels.Find(c => c.Id == (long) arg!);

                    if (mockChannel != null)
                    {
                        response.Data = mockChannel.ToChannelRpc();
                    }

                    else if (!_configuration!.DisableDissonityInfoLogs) Utils.DissonityLogWarning("You can get mock channel data by calling Api.Commands.GetChannel with a mock channel id");

                    //? Channel id is query channel id
                    if (mock._query.ChannelId == (long) arg!)
                    {
                        // I believe it makes more sense to only add these if the channel
                        // is the same that's sent in the query
                        response.Data.VoiceStates = mock.GetUserVoiceStates();
                    }

                    ((TaskCompletionSource<GetChannelResponse>) (object) tcs).TrySetResult(response);
                }

                // GetInstanceConnectedParticipantsResponse
                else if (typeof(TResponse) == typeof(GetInstanceConnectedParticipantsResponse))
                {
                    var response = new GetInstanceConnectedParticipantsResponse();

                    response.Data = new()
                    {
                        Participants = mock.GetParticipants()
                    };

                    ((TaskCompletionSource<GetInstanceConnectedParticipantsResponse>) (object) tcs).TrySetResult(response);
                }

                // GetPlatformBehaviorsResponse
                else if (typeof(TResponse) == typeof(GetPlatformBehaviorsResponse))
                {
                    var response = new GetPlatformBehaviorsResponse();

                    response.Data = new()
                    {
                        IosKeyboardResizesView = true
                    };

                    ((TaskCompletionSource<GetPlatformBehaviorsResponse>) (object) tcs).TrySetResult(response);
                }

                // GetSkusResponse
                else if (typeof(TResponse) == typeof(GetSkusResponse))
                {
                    var response = new GetSkusResponse();

                    response.Data = new()
                    {
                        Skus = mock.GetSkus()
                    };

                    ((TaskCompletionSource<GetSkusResponse>) (object) tcs).TrySetResult(response);
                }

                // InitiateImageUploadResponse
                else if (typeof(TResponse) == typeof(InitiateImageUploadResponse))
                {
                    var response = new InitiateImageUploadResponse();

                    response.Data = new()
                    {
                        ImageUrl = "https://placehold.co/100x100"
                    };

                    ((TaskCompletionSource<InitiateImageUploadResponse>) (object) tcs).TrySetResult(response);
                }

                // SetActivityResponse
                else if (typeof(TResponse) == typeof(SetActivityResponse))
                {
                    var response = new SetActivityResponse();

                    response.Data = ((ActivityBuilder) arg!).ToActivity();

                    ((TaskCompletionSource<SetActivityResponse>) (object) tcs).TrySetResult(response);
                }

                // SetConfigResponse
                else if (typeof(TResponse) == typeof(SetConfigResponse))
                {
                    var response = new SetConfigResponse();

                    response.Data = new()
                    {
                        UseInteractivePip = true
                    };

                    ((TaskCompletionSource<SetConfigResponse>) (object) tcs).TrySetResult(response);
                }

                // StartPurchaseResponse
                else if (typeof(TResponse) == typeof(StartPurchaseResponse))
                {
                    var response = new StartPurchaseResponse();

                    //? Sku id is an actual mock sku
                    MockSku? mockSku = mock._skus.Find(s => s.Id == (long) arg!);

                    if (mockSku != null)
                    {
                        //? Add SKU if it's not added yet
                        MockEntitlement? mockEntitlement = mock._entitlements.Find(e => e.SkuId == (long) arg!);

                        if (mockEntitlement == null)
                        {
                            long id = Utils.GetMockSnowflake();

                            mock._entitlements.Add(new MockEntitlement()
                            {
                                UserId = mock._currentPlayer.Participant.Id,
                                SkuId = (long) arg!,
                                Id = id,
                                __mock__name = $"{mockSku.Name} Entitlement"
                            });

                            mock.EntitlementCreate(id);
                        }
                    }

                    else if (!_configuration!.DisableDissonityInfoLogs) Utils.DissonityLogWarning("You can test a purchase by calling Api.Commands.StartPurchase with a mock sku id");
                    
                    response.Data = mock.GetEntitlements();

                    ((TaskCompletionSource<StartPurchaseResponse>) (object) tcs).TrySetResult(response);
                }

                // UserSettingsGetLocaleResponse
                else if (typeof(TResponse) == typeof(UserSettingsGetLocaleResponse))
                {
                    var response = new UserSettingsGetLocaleResponse();

                    response.Data = new()
                    {
                        Locale = MockUtils.ToLocaleString(mock._locale)
                    };

                    ((TaskCompletionSource<UserSettingsGetLocaleResponse>) (object) tcs).TrySetResult(response);
                }

                else
                    ((TaskCompletionSource<NoResponse>) (object) tcs).TrySetResult(new NoResponse());
            }

            return tcs.Task;
        }

        internal static void MockOn()
        {
            if (disableMock) return;
            _mock = true;
        }

        internal static void MockOff()
        {
            _mock = false;
        }
    }
}
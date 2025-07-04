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

namespace Dissonity
{
    public static class Api
    {
        #nullable enable

        //# FIELDS - - - - -
        internal static long? _clientId;
        internal static string? _instanceId;
        internal static string? _platform;
        internal static long? _guildId = null;
        internal static long? _channelId;
        internal static long? _userId = null;
        internal static string? _accessToken = null;
        internal static string? _appHash = null;
        internal static string? _frameId;
        internal static string? _mobileAppVersion = null;
        internal static string? _customId = null;
        internal static string? _referrerId = null;
        internal static string? _locationId = null;
        internal static ISdkConfiguration? _configuration;
        
        // Utility
        internal static User? _user = null;
        internal static GuildMemberRpc? _guildMemberRpc = null;

        // Messages
        internal static Dictionary<string, object> pendingCommands = new(); // TaskCompletionSource<TResponse>
        internal static DiscordMessageBus discordMessageBus = new();
        internal static HiRpcMessageBus hiRpcMessageBus = new();
        internal static GameObject? bridgeObject = null;
        internal static DissonityBridge? bridge = null;
        internal static HashSet<string> subscribedRpcSet = new();

        // Shortcut
        internal static bool isEditor = UnityEngine.Application.isEditor;

        // Initialization
        private static bool _initialized = false; // Called Initialize
        private static bool _hiRpcReady = false; // hiRPC available
        private static bool _ready = false; // RPC available
        private static bool _mock = false;
        private static bool disableMock = false;
        private static TaskCompletionSource<bool> readyTask = new TaskCompletionSource<bool>(null);

        // Handshake, authorization and authentication are handled in the app loader level.


        //# PROPERTIES - - - - -
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
        /// <c> ❄️ </c> The id of the guild on which the activity is running. Returns null if the activity runs in a DM.
        /// </summary>
        public static long? GuildId
        {
            get
            {
                if (!_ready) throw new InvalidOperationException("You can't access this property before waiting for Api.Initialize");

                if (_mock) return GameObject.FindAnyObjectByType<DiscordMock>()._query.GuildId;

                return _guildId;
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
        /// Query custom id. Used when a user joins the activity via a shared link.
        /// </summary>
        public static string CustomId
        {
            get
            {
                if (!_ready) throw new InvalidOperationException("You can't access this property before waiting for Api.Initialize");

                if (_mock) return GameObject.FindAnyObjectByType<DiscordMock>()._query.CustomId;

                return _customId!;
            }
        }

        /// <summary>
        /// Query referrer id. Used when a user joins the activity via a shared link.
        /// </summary>
        public static string ReferrerId
        {
            get
            {
                if (!_ready) throw new InvalidOperationException("You can't access this property before waiting for Api.Initialize");

                if (_mock) return GameObject.FindAnyObjectByType<DiscordMock>()._query.ReferrerId;

                return _referrerId!;
            }
        }

        /// <summary>
        /// Query location id.
        /// </summary>
        public static string LocationId
        {
            get
            {
                if (!_ready) throw new InvalidOperationException("You can't access this property before waiting for Api.Initialize");

                if (_mock) return GameObject.FindAnyObjectByType<DiscordMock>()._query.LocationId;

                return _locationId!;
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
        /// Your client access token. <br/> <br/>
        /// <c> ⚠️ </c> This is <b>highly-sensitive</b> data. Handle this token securely and don't expose it. Use encryption if you need it outside of Unity.
        /// </summary>
        public static string AccessToken
        {
            get
            {
                if (!_ready) throw new InvalidOperationException("You can't access this property before waiting for Api.Initialize");

                if (_mock) {

                    return GameObject.FindAnyObjectByType<DiscordMock>()._accessToken;
                }

                return _accessToken!;
            }
        }

        /// <summary>
        /// Your hiRPC application access hash. <br/> <br/>
        /// <c> ⚠️ </c> This allows access to restricted hiRPC functionality. Handle it securely.
        /// </summary>
        public static string ApplicationAccessHash
        {
            get
            {
                if (!_hiRpcReady) throw new InvalidOperationException("You can't access this property before waiting for Api.Initialize");

                if (_mock) {

                    return GameObject.FindAnyObjectByType<JavascriptMock>()._hiRpcAppHash;
                }

                return _appHash!;
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


        //# HIRPC INTERFACE - - - - -
#if UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern void DsoOpenDownwardFlow();

        [DllImport("__Internal")]
        private static extern void DsoCloseDownwardFlow(string stringifiedMessage);

        [DllImport("__Internal")]
        private static extern void DsoExpandCanvas();

        [DllImport("__Internal")]
        private static extern void DsoSendToRpc(string stringifiedMessage);

        [DllImport("__Internal")]
        private static extern void DsoSendToJs(string stringifiedMessage);

        [DllImport("__Internal")]
        private static extern void DsoLocalStorageSetItem(string stringifiedMessage);

        [DllImport("__Internal")]
        private static extern void DsoLocalStorageClear();
#endif

#if !UNITY_WEBGL
        private static void DsoOpenDownwardFlow() {}
        private static void DsoCloseDownwardFlow(string stringifiedMessage) {}
        private static void DsoExpandCanvas() {}
        private static void DsoSendToRpc(string _) {}
        private static void DsoSendToJs(string _) {}
        private static void DsoLocalStorageSetItem(string _) {}
        private static void DsoLocalStorageClear() {}
#endif


        //# COMMANDS - - - - -
        /// <summary>
        /// Subclass that allows sending RPC commands.
        /// </summary>
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
            public static async Task<ChannelRpc?> GetChannel(long channelId)
            {
                if (!_ready) throw new InvalidOperationException("Tried to use a command without being ready");

                if (_mock)
                {
                    var mockResponse = await MockSendCommand<GetChannelResponse>(channelId);

                    if (mockResponse.Data!.Type == ChannelType.Dm || mockResponse.Data.Type == ChannelType.GroupDm)
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
            public static async Task<OpenExternalLinkData> OpenExternalLink(string url)
            {
                if (!_ready) throw new InvalidOperationException("Tried to use a command without being ready");

                if (_mock)
                {
                    var mockResponse = await MockSendCommand<OpenExternalLinkResponse>();

                    return mockResponse.Data;
                }

                var response = await SendCommand<OpenExternalLink, OpenExternalLinkResponse>(new (url));

                return response.Data;
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

                if (_mock)
                {
                    if (!_configuration!.DisableDissonityInfoLogs)
                    {
                        Utils.DissonityLog("Invite dialog sent");
                    }

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

                if (_mock)
                {
                    if (!_configuration!.DisableDissonityInfoLogs)
                    {
                        if (Platform == Models.Platform.Desktop) Utils.DissonityLog($"Share moment dialog with ({mediaUrl}) sent");
                        else Utils.DissonityLogWarning("Platform is mobile, not possible to open a share moment dialog");
                    }

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
            public static async Task SetOrientationLockState(OrientationLockStateType lockState, OrientationLockStateType? pipLockState = null, OrientationLockStateType? gridLockState = null)
            {
                if (!_ready) throw new InvalidOperationException("Tried to use a command without being ready");

                if (_mock)
                {
                    if (!_configuration!.DisableDissonityInfoLogs)
                    {
                        if (Platform == Models.Platform.Mobile) Utils.DissonityLog($"Set orientation lock state to ({lockState})");
                        else Utils.DissonityLogWarning("Platform is desktop, not possible to set orientation lock state");
                    }

                    return;
                }

                await SendCommand<SetOrientationLockState, NoResponse>(new (lockState, pipLockState, gridLockState));
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

                        throw new CommandException("Invalid oauth scopes inside mock", (int)RpcErrorCode.InvalidPermissions);
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

            /// <summary>
            /// Presents a modal for the user to share a link to your activity with custom query params. <br/> <br/>
            /// No scopes required. <br/>
            /// ---------------------- <br/>
            /// ✅ | Web <br/>
            /// ✅ | iOS <br/>
            /// ✅ | Android <br/>
            /// ---------------------- <br/>
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            /// <exception cref="CommandException"></exception>
            public static async Task<ShareLinkData> ShareLink(string message, string? customId = null, string? linkId = null)
            {
                if (!_ready) throw new InvalidOperationException("Tried to use a command without being ready");

                if (_mock)
                {
                    var mockResponse = await MockSendCommand<ShareLinkResponse>();

                    return mockResponse.Data;
                }

                var response = await SendCommand<ShareLink, ShareLinkResponse>(new (message, customId, linkId));

                return response.Data;
            }

            /// <summary>
            /// Returns the current user's relationships. <br/> <br/>
            /// Scopes required: <c> relationships.read </c> <br/> <br/>
            /// <c> relationships.read </c> requires approval from Discord. <br/>
            /// ---------------------- <br/>
            /// ✅ | Web <br/>
            /// ✅ | iOS <br/>
            /// ✅ | Android <br/>
            /// ---------------------- <br/>
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            /// <exception cref="CommandException"></exception>
            public static async Task<Relationship[]> GetRelationships()
            {
                if (!_ready) throw new InvalidOperationException("Tried to use a command without being ready");

                if (_mock)
                {
                    //? Invalid scopes
                    if (!_configuration!.OauthScopes.Contains(OauthScope.RelationshipsRead))
                    {
                        if (!_configuration!.DisableDissonityInfoLogs) Utils.DissonityLogWarning("Cannot get relationships without the oauth scope 'relationships.read'");

                        throw new CommandException("Invalid oauth scopes inside mock", (int)RpcErrorCode.InvalidPermissions);
                    }

                    var mockResponse = await MockSendCommand<GetRelationshipsResponse>();

                    return mockResponse.Data.Relationships;
                }

                var response = await SendCommand<GetRelationships, GetRelationshipsResponse>(new());

                return response.Data.Relationships;
            }

            //todo Not documented but well-known functionality
            /// <summary>
            /// Returns a user. <br/> <br/>
            /// Available in the official SDK but not documented in https://discord.com/developers/docs/developer-tools/embedded-app-sdk <br/> <br/>
            /// Consider contributing.
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            /// <exception cref="CommandException"></exception>
            private static async Task<User?> GetUser(long userId)
            {
                if (!_ready) throw new InvalidOperationException("Tried to use a command without being ready");

                if (_mock)
                {
                    var mockResponse = await MockSendCommand<GetUserResponse>(userId);

                    return mockResponse.Data;
                }

                var response = await SendCommand<GetUser, GetUserResponse>(new (userId.ToString()));

                return response.Data;
            }
            
            //todo Not documented
            /// <summary>
            /// Invite a user. <br/> <br/>
            /// Available in the official SDK but not documented in https://discord.com/developers/docs/developer-tools/embedded-app-sdk <br/> <br/>
            /// Consider contributing.
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            /// <exception cref="CommandException"></exception>
            private static async Task InviteUserEmbedded(long userId, string? content = null)
            {
                if (!_ready) throw new InvalidOperationException("Tried to use a command without being ready");

                if (_mock)
                {
                    if (!_configuration!.DisableDissonityInfoLogs)
                    {
                        Utils.DissonityLog("Embedded invitation sent");
                    }

                    return;
                }

                await SendCommand<InviteUserEmbedded, NoResponse>(new(userId.ToString(), content));
            }
        }

        //# PROXY - - - - -
        /// <summary>
        /// Subclass that helps sending HTTPs requests through the Discord proxy.
        /// </summary>
        public static class Proxy
        {
            // POST - - - - -
            /// <summary>
            /// Sends an HTTPS post request with a JSON payload to the Discord proxy. <br/> <br/>
            /// You can pass a relative <c> /cat </c> or absolute <c> https://... </c> path. <br/> <br/>
            /// <c> ⚠️ </c> If you use an absolute path, you'll need to patch the url mappings so the request goes through the proxy.
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            /// <exception cref="WebException"></exception>
            /// <exception cref="JsonException"></exception>
            public static Task<TJsonResponse> HttpsPostRequest<TJsonRequest, TJsonResponse>(string path, TJsonRequest payload, Dictionary<string, string>? headers = null)
            {
                if (!_ready) throw new InvalidOperationException("Tried to make a proxy request without being ready");

#if UNITY_EDITOR
                string loweredPath = path.ToLower();
                if (isEditor && !loweredPath.StartsWith("http://") && !loweredPath.StartsWith("https://"))
                {
                    MockUseUrlMappings(ref path);
                }
#endif
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
            /// <exception cref="WebException"></exception>
            /// <exception cref="JsonException"></exception>
            public static Task<TJsonResponse> HttpsGetRequest<TJsonResponse>(string path, Dictionary<string, string>? headers = null)
            {
                if (!_ready) throw new InvalidOperationException("Tried to make a proxy request without being ready");

#if UNITY_EDITOR
                string loweredPath = path.ToLower();
                if (isEditor && !loweredPath.StartsWith("http://") && !loweredPath.StartsWith("https://"))
                {
                    MockUseUrlMappings(ref path);
                }
#endif
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
            /// <exception cref="WebException"></exception>
            /// <exception cref="JsonException"></exception>
            public static Task<TJsonResponse> HttpsPatchRequest<TJsonRequest, TJsonResponse>(string path, TJsonRequest payload, Dictionary<string, string>? headers = null)
            {
                if (!_ready) throw new InvalidOperationException("Tried to make a proxy request without being ready");

#if UNITY_EDITOR
                string loweredPath = path.ToLower();
                if (isEditor && !loweredPath.StartsWith("http://") && !loweredPath.StartsWith("https://"))
                {
                    MockUseUrlMappings(ref path);
                }
#endif
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
            /// <exception cref="WebException"></exception>
            /// <exception cref="JsonException"></exception>
            public static Task<TJsonResponse> HttpsPutRequest<TJsonRequest, TJsonResponse>(string path, TJsonRequest payload, Dictionary<string, string>? headers = null)
            {
                if (!_ready) throw new InvalidOperationException("Tried to make a proxy request without being ready");

#if UNITY_EDITOR
                string loweredPath = path.ToLower();
                if (isEditor && !loweredPath.StartsWith("http://") && !loweredPath.StartsWith("https://"))
                {
                    MockUseUrlMappings(ref path);
                }
#endif
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
            /// <exception cref="WebException"></exception>
            /// <exception cref="JsonException"></exception>
            public static Task<TJsonResponse> HttpsDeleteRequest<TJsonRequest, TJsonResponse>(string path, TJsonRequest payload, Dictionary<string, string>? headers = null)
            {
                if (!_ready) throw new InvalidOperationException("Tried to make a proxy request without being ready");

#if UNITY_EDITOR
                string loweredPath = path.ToLower();
                if (isEditor && !loweredPath.StartsWith("http://") && !loweredPath.StartsWith("https://"))
                {
                    MockUseUrlMappings(ref path);
                }
#endif
                string uri = GetFormattedUri(path);

                var tcs = new TaskCompletionSource<TJsonResponse>();
                
                bridge!.StartCoroutine( SendDeleteRequest(uri, payload, tcs, headers) );

                return tcs.Task;
            }

            private static string GetFormattedUri(string path)
            {
                string uri;

                string loweredPath = path.ToLower();
                if (!loweredPath.StartsWith("http://") && !loweredPath.StartsWith("https://"))
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
                //!! Unity 2021's UnityWebRequest.Post behaves differently than Unity 6's, so using a UnityWebRequest instance instead.
                UnityWebRequest request = new UnityWebRequest(uri, "POST");

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
        
            private static IEnumerator SendDeleteRequest<TJsonRequest, TJsonResponse>(string uri, TJsonRequest payload, TaskCompletionSource<TJsonResponse> tcs, Dictionary<string, string>? headers = null)
            {
                UnityWebRequest request = new UnityWebRequest(uri, "DELETE");
                
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
        
            private static void MockUseUrlMappings(ref string path)
            {
                string[] segments = path.Split("/", StringSplitOptions.RemoveEmptyEntries);

                if (segments.Length < 1)
                {
                    throw new InvalidOperationException("Cannot make a request to / in the editor");
                }

                string prefix = "/" + segments[0];
                MappingBuilder? mapping = _configuration!.Mappings.FirstOrDefault(m => m.Prefix == prefix);

                //? Mapping not found
                if (mapping == null)
                {
                    throw new InvalidOperationException("To mock relative requests while inside Unity, you must define the URL mappings in the configuration file");
                }

                segments[0] = mapping.Target;

                path = "https://" + string.Join("/", segments);
            }
        }

        //# SUBSCRIBE - - - -
        /// <summary>
        /// Subclass that allows listening to RPC events.
        /// </summary>
        public static class Subscribe
        {
            //* SubscribeCommandFactory handles mock mode

            /// <summary>
            /// Received when the number of instance participants changes. <br/> <br/>
            /// No scopes required.
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            /// <exception cref="CommandException"></exception>
            public static async Task<DiscordSubscription> ActivityInstanceParticipantsUpdate(Action<Participant[]> listener)
            {
                if (!_ready) throw new InvalidOperationException("Tried to subscribe without being ready");

                var reference = await SubscribeCommandFactory<ActivityInstanceParticipantsUpdate>(new MessageBusReaderIndefinite<DiscordEvent>(listener, (discordEvent) =>
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
            public static async Task<DiscordSubscription> ActivityLayoutModeUpdate(Action<LayoutModeType> listener)
            {
                if (!_ready) throw new InvalidOperationException("Tried to subscribe without being ready");

                var reference = await SubscribeCommandFactory<ActivityLayoutModeUpdate>(new MessageBusReaderIndefinite<DiscordEvent>(listener, (discordEvent) =>
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
            public static async Task<DiscordSubscription> CurrentUserUpdate(Action<User> listener)
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


            /// <summary>
            /// Received when the current guild member object changes. <br/> <br/>
            /// Scopes required: <c> identify </c>, <c> guilds.members.read </c> <br/>
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            /// <exception cref="CommandException"></exception>
            public static async Task<DiscordSubscription> CurrentGuildMemberUpdate(long guildId, Action<GuildMemberRpc> listener)
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
            /// Received when an entitlement is created for a SKU. <br/> <br/>
            /// No scopes required
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            /// <exception cref="CommandException"></exception>
            public static async Task<DiscordSubscription> EntitlementCreate(Action<Entitlement> listener)
            {
                if (!_ready) throw new InvalidOperationException("Tried to subscribe without being ready");

                var reference = await SubscribeCommandFactory<EntitlementCreate>(new MessageBusReaderIndefinite<DiscordEvent>(listener, (discordEvent) =>
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
            public static DiscordSubscription Error(Action<ErrorEventData> listener)
            {
                if (!_ready) throw new InvalidOperationException("Tried to subscribe without being ready");

                //\ Create reader
                var reader = new MessageBusReaderIndefinite<DiscordEvent>(listener, (discordEvent) =>
                {
                    if (discordEvent is not ErrorEvent _) return;

                    listener((ErrorEventData) discordEvent.Data!);
                });

                //\ Create reference
                var reference = new DiscordSubscription();
                reference.SaveSubscriptionData(reader, DiscordEventType.Error);

                //\ Save reader in message bus
                discordMessageBus.AddReader(DiscordEventType.Error, reader);

                return reference;
            }


            /// <summary>
            /// Received when screen orientation changes. <br/> <br/>
            /// No scopes required.
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            /// <exception cref="CommandException"></exception>
            public static async Task<DiscordSubscription> OrientationUpdate(Action<OrientationType> listener)
            {
                if (!_ready) throw new InvalidOperationException("Tried to subscribe without being ready");

                var reference = await SubscribeCommandFactory<OrientationUpdate>(new MessageBusReaderIndefinite<DiscordEvent>(listener, (discordEvent) =>
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
            public static async Task<DiscordSubscription> SpeakingStart(long channelId, Action<SpeakingData> listener)
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
            public static async Task<DiscordSubscription> SpeakingStop(long channelId, Action<SpeakingData> listener)
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
            public static async Task<DiscordSubscription> ThermalStateUpdate(Action<ThermalStateType> listener)
            {
                if (!_ready) throw new InvalidOperationException("Tried to subscribe without being ready");

                var reference = await SubscribeCommandFactory<ThermalStateUpdate>(new MessageBusReaderIndefinite<DiscordEvent>(listener, (discordEvent) =>
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
            public static async Task<DiscordSubscription> VoiceStateUpdate(long channelId, Action<UserVoiceState> listener)
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
            
            /// <summary>
            /// Received when a relationship of the current user is updated. <br/> <br/>
            /// Scopes required: <c> relationships.read </c> <br/> <br/>
            /// <c> relationships.read </c> requires approval from Discord. <br/>
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            /// <exception cref="CommandException"></exception>
            public static async Task<DiscordSubscription> RelationshipUpdate(Action<Relationship> listener)
            {
                if (!_ready) throw new InvalidOperationException("Tried to subscribe without being ready");

                if (_mock)
                {
                    //? Invalid scopes
                    if (!_configuration!.OauthScopes.Contains(OauthScope.RelationshipsRead))
                    {
                        if (!_configuration!.DisableDissonityInfoLogs) Utils.DissonityLogWarning("Cannot subscribe to Relationship Update without the oauth scope 'relationships.read'");
                    
                        throw new CommandException("Invalid oauth scopes inside mock", (int) RpcErrorCode.InvalidPermissions);
                    }
                }

                var reference = await SubscribeCommandFactory<RelationshipUpdate, Relationship>(listener);

                return reference;
            }

            // Method used to simplify the subscription methods
            internal static async Task<DiscordSubscription> SubscribeCommandFactory<TEvent, TEventData>(Action<TEventData> listener, object? args = null, bool isInternal = false, bool once = false) where TEvent : DiscordEvent
            {
                string eventString = EventUtility.GetStringFromType(typeof(TEvent));

                //\ Create reader
                MessageBusReader<DiscordEvent> reader;

                //? Once
                if (once)
                {
                    reader = new MessageBusReaderOnce<DiscordEvent>(listener, (discordEvent) =>
                    {
                        if (discordEvent is not TEvent _) return;

                        listener((TEventData) discordEvent.Data!);
                    });
                }

                //? Indefinite
                else
                {
                    reader = new MessageBusReaderIndefinite<DiscordEvent>(listener, (discordEvent) =>
                    {
                        if (discordEvent is not TEvent _) return;

                        listener((TEventData) discordEvent.Data!);
                    });
                }

                //\ Create reference
                var reference = new DiscordSubscription();
                reference.SaveSubscriptionData(reader, eventString);

                //\ Save reader in message bus
                discordMessageBus.AddReader(eventString, reader, isInternal);

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
        
            // SubcribeCommandFactory but with a manual reader. Used to return <Event>.Data.<UniqueProperty> in Discord subscriptions
            internal static async Task<DiscordSubscription> SubscribeCommandFactory<TEvent>(MessageBusReader<DiscordEvent> reader, object? args = null, bool isInternal = false) where TEvent : DiscordEvent
            {
                string eventString = EventUtility.GetStringFromType(typeof(TEvent));

                //\ Create reference
                var reference = new DiscordSubscription();
                reference.SaveSubscriptionData(reader, eventString);

                //\ Save reader in message bus
                discordMessageBus.AddReader(eventString, reader, isInternal);

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

        //# HIRPC - - - - -
        /// <summary>
        /// Subclass that enables C#/JS interoperation.
        /// </summary>
        public static class HiRpc
        {
            /// <summary>
            /// Send a message to JavaScript through this hiRPC channel. <br/> <br/>
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            public static void Send(string hiRpcChannel, object payload)
            {
                if (!_hiRpcReady) throw new InvalidOperationException("Tried to send a hiRPC message without being ready");
#if UNITY_EDITOR
                if (isEditor)
                {
                    JavascriptMock? jsMock = GameObject.FindAnyObjectByType<JavascriptMock>();

                    if (jsMock == null) throw new InvalidOperationException("Create a @JavascriptMock to use hiRPC while inside Unity");

                    if (!jsMock._hiRpclogJsMessages) return;

                    Utils.DissonityLog($"Message sent to JavaScript through '{hiRpcChannel}': {JsonConvert.SerializeObject(payload)}");

                    return;
                }
#endif
                BridgeMessage message = new()
                {
                    AppHash = _appHash!,
                    Data = payload,
                    Channel = hiRpcChannel
                };

                DsoSendToJs(JsonConvert.SerializeObject(message));
            }


            /// <summary>
            /// <c> ☄️ </c> <b> Initialization not required. </b> <br/> <br/>
            /// Receive messages sent through this hiRPC channel from JavaScript. <br/> <br/>
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            public static HiRpcSubscription Subscribe(string hiRpcChannel, Action<object> listener)
            {
#if UNITY_EDITOR
                if (isEditor)
                {
                    JavascriptMock? jsMock = GameObject.FindAnyObjectByType<JavascriptMock>();

                    if (jsMock == null) throw new InvalidOperationException("Create a @JavascriptMock to use hiRPC while inside Unity");
                }
#endif
                var reference = SubscribeCommandFactory(new MessageBusReaderIndefinite<HiRpcMessage>(listener, (hiRpcMessage) =>
                {
                    listener(hiRpcMessage.Data);
                }), hiRpcChannel);

                return reference;
            }


            //# UNSUBSCRIBE HIRPC - - - - -
            /// <summary>
            /// <c> ☄️ </c> <b> Initialization not required. </b> <br/> <br/>
            /// Remove a hiRPC subscription via a HiRpcSubscription instance (returned by HiRpc.Subscribe).
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            /// <exception cref="ArgumentException"></exception>
            public static void Unsubscribe(HiRpcSubscription reference)
            {
#if UNITY_EDITOR
                if (isEditor)
                {
                    JavascriptMock? jsMock = GameObject.FindAnyObjectByType<JavascriptMock>();

                    if (jsMock == null) throw new InvalidOperationException("Create a @JavascriptMock to use hiRPC while inside Unity");
                }
#endif
                if (!hiRpcMessageBus.ReaderSetDictionary.ContainsKey(reference.EventString)) throw new ArgumentException($"Tried to unsubscribe from a HiRpcSubscription that no longer exists ({reference.EventString})");

                var readerSet = hiRpcMessageBus.ReaderSetDictionary[reference.EventString];

                //? Reader set has reader
                if (!readerSet.Contains(reference.Reader)) throw new ArgumentException("Tried to unsubscribe from a HiRpcSubscription that no longer exists");
            
                hiRpcMessageBus.RemoveReader(reference.EventString, reference.Reader);
            }


            /// <summary>
            /// <c> ☄️ </c> <b> Initialization not required. </b> <br/> <br/>
            /// Remove a hiRPC subscription via a method and a channel.
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            /// <exception cref="ArgumentException"></exception>
            public static void Unsubscribe(string hiRpcChannel, Action<object> listener)
            {
#if UNITY_EDITOR
                if (isEditor)
                {
                    JavascriptMock? jsMock = GameObject.FindAnyObjectByType<JavascriptMock>();

                    if (jsMock == null) throw new InvalidOperationException("Create a @JavascriptMock to use hiRPC while inside Unity");
                }
#endif
                if (!hiRpcMessageBus.ReaderSetDictionary.ContainsKey(hiRpcChannel)) throw new ArgumentException($"Tried to unsubscribe from a hiRPC channel you're not subscribed to ({hiRpcChannel})");

                var readerSet = hiRpcMessageBus.ReaderSetDictionary[hiRpcChannel];

                //? Reader set has reader
                var reader = readerSet.FirstOrDefault(r => (Action<object>) r.UserListener == listener);
                if (reader == null) throw new ArgumentException($"Tried to unsubscribe from a listener that no longer exists");
            
                hiRpcMessageBus.RemoveReader(hiRpcChannel, reader);
            }


            /// <summary>
            /// <c> ☄️ </c> <b> Initialization not required. </b> <br/> <br/>
            /// Remove all subscriptions related to a single hiRPC channel.
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            public static void UnsubscribeFromChannel(string hiRpcChannel)
            {
#if UNITY_EDITOR
                if (isEditor)
                {
                    JavascriptMock? jsMock = GameObject.FindAnyObjectByType<JavascriptMock>();

                    if (jsMock == null) throw new InvalidOperationException("Create a @JavascriptMock to use hiRPC while inside Unity");
                }
#endif
                if (!hiRpcMessageBus.ReaderSetDictionary.ContainsKey(hiRpcChannel)) return;

                hiRpcMessageBus.ReaderSetDictionary.Remove(hiRpcChannel);
            }


            /// <summary>
            /// <c> ☄️ </c> <b> Initialization not required. </b> <br/> <br/>
            /// Remove all subscriptions from every hiRPC channel.
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            public static void ClearAllSubscriptions()
            {
#if UNITY_EDITOR
                if (isEditor)
                {
                    JavascriptMock? jsMock = GameObject.FindAnyObjectByType<JavascriptMock>();

                    if (jsMock == null) throw new InvalidOperationException("Create a @JavascriptMock to use hiRPC while inside Unity");
                }
#endif
                hiRpcMessageBus.ReaderSetDictionary.Clear();
            }


            // SubcribeCommandFactory but with a manual reader. It allows to use the hiRPC bus type
            internal static HiRpcSubscription SubscribeCommandFactory(MessageBusReader<HiRpcMessage> reader, string hiRpcChannel, bool isInternal = false)
            {
                //\ Create reference
                var reference = new HiRpcSubscription();
                reference.SaveSubscriptionData(reader, hiRpcChannel);

                //\ Save reader in message bus
                hiRpcMessageBus.AddReader(hiRpcChannel, reader, isInternal);

                return reference;
            }
        }

        //# LOCAL STORAGE - - - - -
        /// <summary>
        /// Subclass that allows storing key-value pairs locally.
        /// </summary>
        public static class LocalStorage
        {
            /// <summary>
            /// Add persistent data to the local storage.
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            public static void SetItem(string key, string value)
            {
                if (!_hiRpcReady) throw new InvalidOperationException("Tried to use local storage without being ready");

#if UNITY_EDITOR
                if (isEditor)
                {
                    JavascriptMock? jsMock = GameObject.FindAnyObjectByType<JavascriptMock>();

                    if (jsMock == null) throw new InvalidOperationException("Create a @JavascriptMock to use Local Storage while inside Unity");

                    jsMock._localStorage.Add(new (){
                        Key = key,
                        Value = value
                    });
                    
                    return;
                }
#endif
                BridgeMessage message = new()
                {
                    Data = new string[] { key, value }
                };

                DsoLocalStorageSetItem(JsonConvert.SerializeObject(message));
            }

            /// <summary>
            /// Get data from the local storage.
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            public async static Task<string?> GetItem(string key)
            {
                if (!_hiRpcReady) throw new InvalidOperationException("Tried to use local storage without being ready");

#if UNITY_EDITOR
                if (isEditor)
                {
                    JavascriptMock? jsMock = GameObject.FindAnyObjectByType<JavascriptMock>();

                    if (jsMock == null) throw new InvalidOperationException("Create a @JavascriptMock to use Local Storage while inside Unity");

                    MockStorageItem? item = jsMock._localStorage.Find(i => i.Key == key);

                    if (item == null) return null;
                    
                    return item.Value;
                }
#endif
                return await bridge!.ExeLocalStorageGetItem(key);
            }

            /// <summary>
            /// Clear local storage.
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            public static void Clear()
            {
                if (!_hiRpcReady) throw new InvalidOperationException("Tried to use local storage without being ready");

#if UNITY_EDITOR
                if (isEditor)
                {
                    JavascriptMock? jsMock = GameObject.FindAnyObjectByType<JavascriptMock>();

                    if (jsMock == null) throw new InvalidOperationException("Create a @JavascriptMock to use Local Storage while inside Unity");

                    jsMock._localStorage.Clear();

                    return;
                }
#endif
                DsoLocalStorageClear();
            }
        }

        //# METHODS - - - - -
        /// <summary>
        /// Initializes Dissonity. You must call and await this method once before doing anything else.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="OutsideDiscordException"></exception>
        /// <exception cref="ExternalException"></exception>
        /// <exception cref="JsonException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static Task<MultiEvent> Initialize()
        {
#if !UNITY_WEBGL && !UNITY_EDITOR
            throw new OutsideDiscordException("Not a WebGL build");
#endif
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

#if UNITY_EDITOR
            //? Not running inside Discord
            if (isEditor)
            {
                MockOn();

                //\ Find mock
                var mock = GameObject.FindAnyObjectByType<DiscordMock>();

                //? Create mock
                if (mock == null)
                {
                    var mockObject = new GameObject("@DiscordMock");
                    mockObject.AddComponent<DiscordMock>();

                    if (!_configuration!.DisableDissonityInfoLogs)
                    {
                        Utils.DissonityLog("Running inside the Unity editor, a Discord Mock has been generated. To manually create one: Right-click hierarchy > Dissonity > Discord Mock");
                    }
                }

                //\ Find JS Mock
                var jsMock = GameObject.FindAnyObjectByType<JavascriptMock>();

                //? Create JS Mock
                if (jsMock == null)
                {
                    var mockObject = new GameObject("@JavascriptMock");
                    mockObject.AddComponent<JavascriptMock>();
                }

                _ready = true;
                _hiRpcReady = true;

                readyTask.SetResult(true);

                return Task.FromResult(new MockMultiEvent().ToMultiEvent());
            }
#endif
            // Mock is invalid here
            _mock = false;
            disableMock = true;

            UnityEngine.Application.runInBackground = true;

            async void ListenToPayload()
            {
                // OverrideConsoleLogging is done in the hiRPC layer.
                // The RPC handshake is handled by the hiRPC even before the Unity build loads.
                MultiEvent? multiEvent = await bridge!.ExeMultiEvent();

                _hiRpcReady = true;

                //? Null Multi Event
                if (multiEvent == null)
                {
                    tcs.TrySetException(new OutsideDiscordException("The Multi Event is null - the environment is outside Discord"));
                    readyTask.SetResult(false);
                    return;
                }

                _userId = multiEvent.AuthenticateData.User.Id;
                _accessToken = multiEvent.AuthenticateData.AccessToken;

                string query = await bridge!.ExeQuery();

                InitializeQuery(tcs, multiEvent, query);
            }

            ListenToPayload();

            // After opening the downward flow, hiRPC will send the first payload (dissonity channel handshake) once ready.
            // From then, the JS and C# layer can interact.
            // This loads the hiRPC module if LAZY_HIRPC_LOAD is set to true.
            DsoOpenDownwardFlow();

            return tcs.Task;
        }


        /// <summary>
        /// Use this method to wait for initialization to complete. <br/> <br/>
        /// Returns true if the game is executed inside Discord.
        /// </summary>
        public static async Task<bool> OnReady()
        {
            if (_ready) return true;

            return await readyTask.Task;
        }


        /// <summary>
        /// Use this method to easily access external resources. <br/> <br/>
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
        /// Remove a subscription via a DiscordSubscription instance (returned by subscription methods).
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static void Unsubscribe(DiscordSubscription reference)
        {
            if (!_ready) throw new InvalidOperationException("Tried to unsubscribe without being ready");
            
            if (!discordMessageBus.ReaderSetDictionary.ContainsKey(reference.EventString)) throw new ArgumentException($"Tried to unsubscribe from a DiscordSubscription that no longer exists ({reference.EventString})");

            var readerSet = discordMessageBus.ReaderSetDictionary[reference.EventString];

            //? Reader set has reader
            if (!readerSet.Contains(reference.Reader)) throw new ArgumentException("Tried to unsubscribe from a DiscordSubscription that no longer exists");
        
            bool setIsGone = discordMessageBus.RemoveReader(reference.EventString, reference.Reader);

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
            if (!_ready) throw new InvalidOperationException("Tried to unsubscribe without being ready");

            string eventString = EventUtility.GetStringFromReturnDataType(typeof(T));
            
            if (!discordMessageBus.ReaderSetDictionary.ContainsKey(eventString)) throw new ArgumentException($"Tried to unsubscribe from an event you're not subscribed to ({eventString})");

            var readerSet = discordMessageBus.ReaderSetDictionary[eventString];

            //? Reader set has reader
            var reader = readerSet.FirstOrDefault(r => (Action<T>) r.UserListener == listener);
            if (reader == null) throw new ArgumentException($"Tried to unsubscribe from a listener that no longer exists");
        
            bool setIsGone = discordMessageBus.RemoveReader(eventString, reader);

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
        public static void UnsubscribeFromEvent(string eventString)
        {
            if (!_ready) throw new InvalidOperationException("Tried to unsubscribe without being ready");
            
            discordMessageBus.ReaderSetDictionary.Remove(eventString);

            //\ Unsubscribe from RPC
            if (_mock || discordMessageBus.ReaderSetExists(eventString)) return;

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

            discordMessageBus.ReaderSetDictionary.Clear();

            if (_mock) return;

            foreach (string key in subscribedRpcSet)
            {
                if (discordMessageBus.ReaderSetExists(key)) continue;

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

                BridgeMessage bridgeMessage = new()
                {
                    AppHash = _appHash!,
                };

                DsoCloseDownwardFlow(JsonConvert.SerializeObject(bridgeMessage));
            }

            discordMessageBus.ReaderSetDictionary.Clear();
            subscribedRpcSet.Clear();
            GameObject.Destroy(bridge);
            _ready = false;

            if (_mock && !_configuration!.DisableDissonityInfoLogs)
            {
                Utils.DissonityLog($"Embedded app closed with code {code} and message '{message}'");
            }
        }


        //# PRIVATE METHODS - - - - -
        private static void InitializeQuery(TaskCompletionSource<MultiEvent> tcs, MultiEvent multiEvent, string stringifiedQuery)
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

            // Custom id
            if (query.CustomId != null)
            {
                _customId = query.CustomId;
            }

            // Referrer id
            if (query.ReferrerId != null)
            {
                _referrerId = query.ReferrerId;
            }

            // Location id
            if (query.LocationId != null)
            {
                _locationId = query.LocationId;
            }

            CompleteInitialization(tcs, multiEvent);
        }

        private static async void CompleteInitialization(TaskCompletionSource<MultiEvent> tcs, MultiEvent multiEvent)
        {
            //# CHECK STATE - - - - -
            var code = await bridge!.ExeState();

            if (!_configuration!.DisableDissonityInfoLogs)
            {
                //? Problem
                if (code == StateCode.Errored || code == StateCode.Unfunctional)
                {
                    Utils.DissonityLogError($"hiRPC returned unexpected state code: {code}");
                }

                else Utils.DissonityLog($"hiRPC returned state code: {code}");
            }
            
            //? OutsideDiscord
            if (code == StateCode.OutsideDiscord)
            {
                tcs.TrySetException(new OutsideDiscordException("hiRPC returned OutsideDiscord state"));
            }

            //# HANDLE CONFIGURATION - - - - -
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
            if (_configuration!.SynchronizeGuildMemberRpc && _guildId != null)
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
        
            //? Desktop
            if (_platform == Models.Platform.Desktop)
            {
                //? Using max resolution
                if (_configuration.DesktopResolution == ScreenResolution.Max)
                {
                    SetupCanvasExpansion();
                }
            }

            //? Mobile
            else
            {
                //? Using max resolution
                if (_configuration.MobileResolution == ScreenResolution.Max)
                {
                    SetupCanvasExpansion();
                }
            }

            _ready = true;
            readyTask.SetResult(true);
            tcs.TrySetResult(multiEvent);
        }

        /// <summary>
        /// After calling this method, window.dso_expand_canvas will be called if it exists (when using max resolution).
        /// </summary>
        private static async void SetupCanvasExpansion()
        {
            // Indefinite orientation update
            await Subscribe.SubscribeCommandFactory<OrientationUpdate, OrientationUpdateData>(_ =>
            {
                DsoExpandCanvas();
            }, null, true);

            // Indefinite layout update
            await Subscribe.SubscribeCommandFactory<ActivityLayoutModeUpdate, ActivityLayoutModeUpdateData>(_ =>
            {
                DsoExpandCanvas();
            }, null, true);
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

            // Even for NoResponse commands. This allows CommandException(s) to be raised correctly.
            pendingCommands.Add(commandNonce, tcs);

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

#if !UNITY_EDITOR
                BridgeMessage message = new()
                {
                    Data = new object[2] { command.Opcode, payload }
                };

                //\ Send data to RPC
                DsoSendToRpc(JsonConvert.SerializeObject(message));
#endif
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

                // OpenExternalLinkResponse
                else if (typeof(TResponse) == typeof(OpenExternalLinkResponse))
                {
                    var response = new OpenExternalLinkResponse();

                    response.Data = new()
                    {
                        Opened = true
                    };

                    ((TaskCompletionSource<OpenExternalLinkResponse>) (object) tcs).TrySetResult(response);
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
                                _mock_name = $"{mockSku.Name} Entitlement"
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

                // ShareLinkResponse
                else if (typeof(TResponse) == typeof(ShareLinkResponse))
                {
                    var response = new ShareLinkResponse();

                    response.Data = new()
                    {
                        Success = true,
                        DidCopyLink = true,
                        DidSendMessage = true
                    };

                    ((TaskCompletionSource<ShareLinkResponse>) (object) tcs).TrySetResult(response);
                }

                // GetRelationshipsResponse
                else if (typeof(TResponse) == typeof(GetRelationshipsResponse))
                {
                    var response = new GetRelationshipsResponse();

                    response.Data = new()
                    {
                        Relationships = mock.GetRelationships()
                    };

                    ((TaskCompletionSource<GetRelationshipsResponse>) (object) tcs).TrySetResult(response);
                }

                // GetUserResponse
                else if (typeof(TResponse) == typeof(GetUserResponse))
                {
                    var response = new GetUserResponse();

                    response.Data = new();

                    //? User id is an actual mock user
                    MockPlayer? mockPlayer = mock._otherPlayers.Find(c => c.Participant.Id == (long) arg!);

                    if (mockPlayer != null)
                    {
                        response.Data = mockPlayer.Participant.ToUser();
                    }

                    else if (mock._currentPlayer.Participant.Id == (long) arg!)
                    {
                        response.Data = mock._currentPlayer.Participant.ToUser();
                    }

                    else
                    {
                        MockRelationship? mockRelationship = mock._relationships.Find(c => c.User.Id == (long)arg!);

                        if (mockRelationship != null)
                        {
                            response.Data = mockRelationship.User.ToUser();
                        }

                        else if (!_configuration!.DisableDissonityInfoLogs)
                        {
                            Utils.DissonityLogWarning("You can get mock user data by calling Api.Commands.GetUser with a mock user id");
                        }
                    }

                    ((TaskCompletionSource<GetUserResponse>) (object) tcs).TrySetResult(response);
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
    }

    /// @cond 
    public static class I_UnitApi
    {
        public static void RawOverrideConfiguration(I_UserData config)
        {
            DissonityConfigAttribute._rawOverrideConfiguration = config;
        }
    }
    /// @endcond
}
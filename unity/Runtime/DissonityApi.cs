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

//todo main tasks
// User instance authentication
// PriceUtils is not necessary yet
// PermissionUtils is not necessary yet
// Test patch url mappings
// Test mock
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
        internal static string? _clientId;
        internal static string? _instanceId;
        internal static string? _platform;
        internal static string? _guildId;
        internal static string? _channelId;
        internal static string? _userId = null;
        internal static string? _frameId;
        internal static string? _mobileAppVersion = null;
        internal static ISdkConfiguration? _configuration;
        internal static string handshakeStringId = "handshake";

        // Messages
        internal static Dictionary<string, object> pendingCommands = new(); //TaskCompletionSource<TResponse>
        internal static MessageBus messageBus = new();
        internal static GameObject? bridge = null;
        internal static HashSet<string> subscribedRpcSet = new();

        // Shortcut
        internal static bool isEditor = UnityEngine.Application.isEditor;

        // Initialization
        private static bool _initialized = false;
        private static bool _ready = false;
        private static bool _closedState = false; // True when the RpcBridge is in a closed state
        private static bool _mock = false;
        private static bool _disableMock = false;

        // Constants
        private const string ProxyDomain = "discordsays.com";

        // RpcVersion and RpcEncoding (handshake, overall) is handled in the RpcBridge


        //# PROPERTIES - - - - -
        //todo update current user and current member if the scopes are available???
        // HANDSHAKE_SDK_VERSION_MINIUM_MOBILE_VERSION is in the BridgeLib
        /// <summary>
        /// Embedded App SDK version that Dissonity is simulating.
        /// </summary>
        public const string SdkVersion = "1.5.0";
        public static string ClientId
        {
            get
            {
                if (!_ready) throw new InvalidOperationException("Tried to access a field without initialization");

                return _clientId!;
            }
        }
        public static string? InstanceId
        { 
            get
            {
                if (!_ready) throw new InvalidOperationException("Tried to access a field without initialization");

                if (_mock) return GameObject.FindObjectOfType<DiscordMock>().query.InstanceId;

                return _instanceId!;
            }
        }
        public static string? Platform
        {
           get
            {
                if (!_ready) throw new InvalidOperationException("Tried to access a field without initialization");

                if (_mock) return MockUtils.ToPlatformString(GameObject.FindObjectOfType<DiscordMock>().query.Platform);

                return _platform!;
            } 
        }  
        public static string? GuildId
        {
            get
            {
                if (!_ready) throw new InvalidOperationException("Tried to access a field without initialization");

                if (_mock) return GameObject.FindObjectOfType<DiscordMock>().query.GuildId;

                return _guildId!;
            }
        }
        public static string? ChannelId
        {
            get
            {
                if (!_ready) throw new InvalidOperationException("Tried to access a field without initialization");

                if (_mock) return GameObject.FindObjectOfType<DiscordMock>().query.ChannelId;

                return _channelId!;
            }
        }
        public static string? UserId
        {
            get
            {
                if (!_ready) throw new InvalidOperationException("Tried to access a field without initialization");

                if (_mock) return GameObject.FindObjectOfType<DiscordMock>().currentPlayer.Participant.Id;

                // Should never happen with the MultiEvent implementation, but I'll leave it just in case
                if (!_configuration!.DisableDissonityInfoLogs && _userId == null)
                {
                    Utils.DissonityLogWarning("Tried to access the current user id before authenticating. You will receive null");
                }

                return _userId!;
            }
        }
        public static string? FrameId
        {
            get
            {
                if (!_ready) throw new InvalidOperationException("Tried to access a field without initialization");

                if (_mock) return GameObject.FindObjectOfType<DiscordMock>().query.FrameId;

                return _frameId!;
            }
        }
        public static string? MobileAppVersion
        {
            get
            {
                if (!_ready) throw new InvalidOperationException("Tried to access a field without initialization");

                if (_mock) return GameObject.FindObjectOfType<DiscordMock>().query.MobileAppVersion;

                return _frameId!;
            }
        }
        public static bool Initialized
        {
            get
            {
                return _initialized;
            }
        }
        public static bool Ready
        {
            get
            {
                return _ready;
            }
        }
        public static ISdkConfiguration Configuration
        {
            get
            {
                if (!_ready) throw new InvalidOperationException("Tried to access a field without initialization");

                return _configuration!;
            }
        }
        public static bool IsMock => _mock;

        /// <summary>
        /// You can only access the Bridge field in mock mode.
        /// </summary>
        public static GameObject Bridge
        {
            get
            {
                if (!_mock || !isEditor) throw new InvalidOperationException("You can only access the Bridge field in mock mode");

                if (!_ready) throw new InvalidOperationException("Tried to access a field without initialization");

                return bridge!;
            }
        }


        //# JAVASCRIPT - - - - -
#if UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern void Listen();

        [DllImport("__Internal")]
        private static extern void StopListening();

        [DllImport("__Internal")]
        private static extern void Send(string stringifiedMessage);

        [DllImport("__Internal")]
        private static extern void RequestState();

        [DllImport("__Internal")]
        private static extern void RequestQuery();

        [DllImport("__Internal")]
        private static extern void RequestPatchUrlMappings(string stringifiedMessage);
#endif

#if !UNITY_WEBGL
        private static void Listen() {}
        private static void StopListening() {}
        private static void Send(string _) {}
        private static void RequestState() {}
        private static void RequestQuery() {}
        private static void RequestPatchUrlMappings(string _) {}
#endif

        //# COMMANDS - - - - -
        public static class Commands
        {
            /// <summary>
            /// ❗ <b>Authentication and authorization are handled in the RpcBridge. These methods will remain internal without mock implementation.</b> ❗ <br/> <br/>
            /// Authorize a new client with your app. <br/> <br/>
            /// ---------------------- <br/>
            /// ✅ | Web <br/>
            /// ✅ | iOS <br/>
            /// ✅ | Android <br/>
            /// ---------------------- <br/>
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            /// <exception cref="ArgumentException"></exception>
            internal static async Task<AuthorizeData> Authorize(params string[] scope)
            {
                if (!_ready) throw new InvalidOperationException("Tried to use a command without initialization");

                if (scope.Length == 0) throw new ArgumentException("You must specify valid scopes");

                var response = await SendCommand<Authorize, AuthorizeResponse>(new (_clientId!, scope) {
                    ResponseType = "code"
                });

                return response.Data;
            }


            /// <summary>
            /// ❗ <b>Authentication and authorization are handled in the RpcBridge. These methods will remain internal without mock implementation.</b> ❗ <br/> <br/>
            /// Authenticate an existing client with your app. <br/> <br/>
            /// No scopes required. <br/> <br/>
            /// ---------------------- <br/>
            /// ✅ | Web <br/>
            /// ✅ | iOS <br/>
            /// ✅ | Android <br/>
            /// ---------------------- <br/>
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            internal static async Task<AuthenticateData> Authenticate(string accessToken)
            {
                if (!_ready) throw new InvalidOperationException("Tried to use a command without initialization");

                var response = await SendCommand<Authenticate, AuthenticateResponse>(new (accessToken));

                if (_userId == null) _userId = response.Data.User.Id;
            
                return response.Data;
            }


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
                if (!_ready) throw new InvalidOperationException("Tried to use a command without initialization");

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
                if (!_ready) throw new InvalidOperationException("Tried to use a command without initialization");

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
            public static async Task<GetChannelData> GetChannel(string channelId)
            {
                if (!_ready) throw new InvalidOperationException("Tried to use a command without initialization");

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

                var response = await SendCommand<GetChannel, GetChannelResponse>(new (channelId));

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
                if (!_ready) throw new InvalidOperationException("Tried to use a command without initialization");

                if (_mock)
                {
                    //? Invalid scopes
                    if (!_configuration!.OauthScopes.Contains(OauthScope.GuildMembersRead))
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


            //todo Developer preview, not released yet
            /// <summary>
            /// Not released yet! :p
            /// </summary>
            internal static void GetEntitlements() {}

            /// <summary>
            /// Not released yet! :p
            /// </summary>
            internal static void GetSkus() {}

            /// <summary>
            /// Not released yet! :p
            /// </summary>
            internal static void StartPurchase() {}


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
                if (!_ready) throw new InvalidOperationException("Tried to use a command without initialization");

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
                if (!_ready) throw new InvalidOperationException("Tried to use a command without initialization");

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
                if (!_ready) throw new InvalidOperationException("Tried to use a command without initialization");

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
                if (!_ready) throw new InvalidOperationException("Tried to use a command without initialization");

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
                if (!_ready) throw new InvalidOperationException("Tried to use a command without initialization");
                
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
                if (!_ready) throw new InvalidOperationException("Tried to use a command without initialization");

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
                if (!_ready) throw new InvalidOperationException("Tried to use a command without initialization");

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
                if (!_ready) throw new InvalidOperationException("Tried to use a command without initialization");

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
                if (!_ready) throw new InvalidOperationException("Tried to use a command without initialization");

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
            public static async Task<GetInstanceConnectedParticipantsData> GetInstanceConnectedParticipants()
            {
                if (!_ready) throw new InvalidOperationException("Tried to use a command without initialization");

                if (_mock)
                {
                    var mockResponse = await MockSendCommand<GetInstanceConnectedParticipantsResponse>();

                    return mockResponse.Data;
                }

                var response = await SendCommand<GetInstanceConnectedParticipants, GetInstanceConnectedParticipantsResponse>(new ());

                return response.Data;
            }
        }

        //# PROXY - - - - -
        public static class Proxy
        {
            // POST - - - - -
            /// <summary>
            /// Sends an HTTPS post request with a JSON payload to the Discord proxy.
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            /// <exception cref="OutsideDiscordException"></exception>
            /// <exception cref="WebException"></exception>
            /// <exception cref="JsonException"></exception>
            public static Task<TJsonResponse> HttpsPostRequest<TJsonRequest, TJsonResponse>(string path, TJsonRequest payload)
            {
                if (!_ready) throw new InvalidOperationException("Tried to make a proxy request without initialization");

                if (isEditor) throw new OutsideDiscordException("You can't make requests to the Discord proxy while inside Unity");

                //? Already starts with .proxy
                if (path.StartsWith(".proxy/"))
                {
                    path = path.Replace(".proxy/", "");
                }

                path = path.StartsWith("/")
                    ? path
                    : $"/{path}";

                string uri = $"https://{_clientId}.{ProxyDomain}/.proxy{path}";

                var tcs = new TaskCompletionSource<TJsonResponse>();
                
                bridge!.GetComponent<DissonityBridge>().StartCoroutine( SendPostRequest(uri, payload, tcs) );

                return tcs.Task;
            }


            // GET - - - - -
            /// <summary>
            /// Sends an HTTPS get request to the Discord proxy.
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            /// <exception cref="OutsideDiscordException"></exception>
            /// <exception cref="WebException"></exception>
            /// <exception cref="JsonException"></exception>
            public static Task<TJsonResponse> HttpsGetRequest<TJsonResponse>(string path)
            {
                if (!_ready) throw new InvalidOperationException("Tried to make a proxy request without initialization");

                if (isEditor) throw new OutsideDiscordException("You can't make requests to the Discord proxy while inside Unity");

                //? Already starts with .proxy
                if (path.StartsWith(".proxy/"))
                {
                    path = path.Replace(".proxy/", "");
                }

                path = path.StartsWith("/")
                    ? path
                    : $"/{path}";

                string uri = $"https://{_clientId}.{ProxyDomain}/.proxy{path}";

                var tcs = new TaskCompletionSource<TJsonResponse>();
                
                bridge!.GetComponent<DissonityBridge>().StartCoroutine( SendGetRequest(uri, tcs) );

                return tcs.Task;
            }


            // PATCH - - - - -
            /// <summary>
            /// Sends an HTTPS patch request with a JSON payload to the Discord proxy.
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            /// <exception cref="OutsideDiscordException"></exception>
            /// <exception cref="WebException"></exception>
            /// <exception cref="JsonException"></exception>
            public static Task<TJsonResponse> HttpsPatchRequest<TJsonRequest, TJsonResponse>(string path, TJsonRequest payload)
            {
                if (!_ready) throw new InvalidOperationException("Tried to make a proxy request without initialization");

                if (isEditor) throw new OutsideDiscordException("You can't make requests to the Discord proxy while inside Unity");

                //? Already starts with .proxy
                if (path.StartsWith(".proxy/"))
                {
                    path = path.Replace(".proxy/", "");
                }

                path = path.StartsWith("/")
                    ? path
                    : $"/{path}";

                string uri = $"https://{_clientId}.{ProxyDomain}/.proxy{path}";

                var tcs = new TaskCompletionSource<TJsonResponse>();
                
                bridge!.GetComponent<DissonityBridge>().StartCoroutine( SendPatchRequest(uri, payload, tcs) );

                return tcs.Task;
            }


            // PUT - - - - -
            /// <summary>
            /// Sends an HTTPS put request with a JSON payload to the Discord proxy.
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            /// <exception cref="OutsideDiscordException"></exception>
            /// <exception cref="WebException"></exception>
            /// <exception cref="JsonException"></exception>
            public static Task<TJsonResponse> HttpsPutRequest<TJsonRequest, TJsonResponse>(string path, TJsonRequest payload)
            {
                if (!_ready) throw new InvalidOperationException("Tried to make a proxy request without initialization");

                if (isEditor) throw new OutsideDiscordException("You can't make requests to the Discord proxy while inside Unity");

                //? Already starts with .proxy
                if (path.StartsWith(".proxy/"))
                {
                    path = path.Replace(".proxy/", "");
                }

                path = path.StartsWith("/")
                    ? path
                    : $"/{path}";

                string uri = $"https://{_clientId}.{ProxyDomain}/.proxy{path}";

                var tcs = new TaskCompletionSource<TJsonResponse>();
                
                bridge!.GetComponent<DissonityBridge>().StartCoroutine( SendPutRequest(uri, payload, tcs) );

                return tcs.Task;
            }


            // DELETE - - - - -
            /// <summary>
            /// Sends an HTTPS delete request to the Discord proxy.
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            /// <exception cref="OutsideDiscordException"></exception>
            /// <exception cref="WebException"></exception>
            /// <exception cref="JsonException"></exception>
            public static Task<TJsonResponse> HttpsDeleteRequest<TJsonResponse>(string path)
            {
                if (!_ready) throw new InvalidOperationException("Tried to make a proxy request without initialization");

                if (isEditor) throw new OutsideDiscordException("You can't make requests to the Discord proxy while inside Unity");

                //? Already starts with .proxy
                if (path.StartsWith(".proxy/"))
                {
                    path = path.Replace(".proxy/", "");
                }

                path = path.StartsWith("/")
                    ? path
                    : $"/{path}";

                string uri = $"https://{_clientId}.{ProxyDomain}/.proxy{path}";

                var tcs = new TaskCompletionSource<TJsonResponse>();
                
                bridge!.GetComponent<DissonityBridge>().StartCoroutine( SendDeleteRequest(uri, tcs) );

                return tcs.Task;
            }

            private static IEnumerator SendPostRequest<TJsonRequest, TJsonResponse>(string uri, TJsonRequest payload, TaskCompletionSource<TJsonResponse> tcs)
            {
                UnityWebRequest request = UnityWebRequest.Post(uri, JsonConvert.SerializeObject(payload), "application/json");
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
        
            private static IEnumerator SendGetRequest<TJsonResponse>(string uri, TaskCompletionSource<TJsonResponse> tcs)
            {
                UnityWebRequest request = UnityWebRequest.Get(uri);
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
        
            private static IEnumerator SendPatchRequest<TJsonRequest, TJsonResponse>(string uri, TJsonRequest payload, TaskCompletionSource<TJsonResponse> tcs)
            {
                UnityWebRequest request = new UnityWebRequest(uri, "PATCH");

                request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload)));
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

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
        
            private static IEnumerator SendPutRequest<TJsonRequest, TJsonResponse>(string uri, TJsonRequest payload, TaskCompletionSource<TJsonResponse> tcs)
            {
                UnityWebRequest request = new UnityWebRequest(uri, "PUT");

                request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload)));
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

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
        
            private static IEnumerator SendDeleteRequest<TJsonResponse>(string uri, TaskCompletionSource<TJsonResponse> tcs)
            {
                UnityWebRequest request = new UnityWebRequest(uri, "DELETE");
                request.downloadHandler = new DownloadHandlerBuffer();

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
            /// <summary>
            /// Received when the number of instance participants changes. <br/> <br/>
            /// No scopes required.
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            public async static Task<SubscriptionReference> ActivityInstanceParticipantsUpdate(Action<ActivityInstanceParticipantsUpdateData> listener)
            {
                if (!_ready) throw new InvalidOperationException("Tried to subscribe without initialization");

                var reference = await SubscribeCommandFactory<ActivityInstanceParticipantsUpdate, ActivityInstanceParticipantsUpdateData>(listener);

                return reference;
            }


            /// <summary>
            /// Received when a user changes the layout mode in the Discord client. <br/> <br/>
            /// No scopes required.
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            public async static Task<SubscriptionReference> ActivityLayoutModeUpdate(Action<ActivityLayoutModeUpdateData> listener)
            {
                if (!_ready) throw new InvalidOperationException("Tried to subscribe without initialization");

                var reference = await SubscribeCommandFactory<ActivityLayoutModeUpdate, ActivityLayoutModeUpdateData>(listener);

                return reference;
            }


            /// <summary>
            /// Received when the current user object changes. <br/> <br/>
            /// Scopes required: <c> identify </c> <br/>
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            public async static Task<SubscriptionReference> CurrentUserUpdate(Action<User> listener)
            {
                if (!_ready) throw new InvalidOperationException("Tried to subscribe without initialization");

                var reference = await SubscribeCommandFactory<CurrentUserUpdate, User>(listener);

                return reference;
            }


            //todo Currently not documented so scopes might be wrong... That's gonna take a while to get merged https://github.com/discord/discord-api-docs/pull/7130
            /// <summary>
            /// Received when the current guild member object changes. <br/> <br/>
            /// Scopes required: <c> identify </c>, <c> guilds.members.read </c> <br/>
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            public async static Task<SubscriptionReference> CurrentGuildMemberUpdate(string guildId, Action<GuildMemberRpc> listener)
            {
                if (!_ready) throw new InvalidOperationException("Tried to subscribe without initialization");

                var reference = await SubscribeCommandFactory<CurrentGuildMemberUpdate, GuildMemberRpc>(listener, new EventArguments{ GuildId = guildId });

                return reference;
            }


            //todo Developer preview, not released yet
            /// <summary>
            /// Not released yet! :p
            /// </summary>
            internal static void EntitlementCreate() {}


            /// <summary>
            /// Non-subscription event sent when there is an error, including command responses. <br/> <br/>
            /// No scopes required.
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            public static SubscriptionReference Error(Action<ErrorEventData> listener)
            {
                if (!_ready) throw new InvalidOperationException("Tried to subscribe without initialization");

                //\ Create reader
                var reader = new MessageBusReaderIndefinite((discordEvent) =>
                {
                    var castedEvent = discordEvent as ErrorEvent;

                    if (castedEvent == null) return;

                    listener(castedEvent.Data!);
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
            public async static Task<SubscriptionReference> OrientationUpdate(Action<OrientationUpdateData> listener)
            {
                if (!_ready) throw new InvalidOperationException("Tried to subscribe without initialization");

                var reference = await SubscribeCommandFactory<OrientationUpdate, OrientationUpdateData>(listener);

                return reference;
            }


            /// <summary>
            /// Received when a user in a subscribed voice channel speaks. <br/> <br/>
            /// Scopes required: <c> rpc.voice.read </c> <br/>
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            public async static Task<SubscriptionReference> SpeakingStart(string channelId, Action<SpeakingStartData> listener)
            {
                if (!_ready) throw new InvalidOperationException("Tried to subscribe without initialization");

                var reference = await SubscribeCommandFactory<SpeakingStart, SpeakingStartData>(listener, new EventArguments{ ChannelId = channelId });

                return reference;
            }


            /// <summary>
            /// Received when a user in a subscribed voice channel stops speaking. <br/> <br/>
            /// Scopes required: <c> rpc.voice.read </c> <br/>
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            public async static Task<SubscriptionReference> SpeakingStop(string channelId, Action<SpeakingStopData> listener)
            {
                if (!_ready) throw new InvalidOperationException("Tried to subscribe without initialization");

                var reference = await SubscribeCommandFactory<SpeakingStop, SpeakingStopData>(listener, new EventArguments{ ChannelId = channelId });

                return reference;
            }


            /// <summary>
            /// Received when Android or iOS thermal states are surfaced to the Discord mobile app. <br/> <br/>
            /// No scopes required
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            public async static Task<SubscriptionReference> ThermalStateUpdate(Action<ThermalStateUpdateData> listener)
            {
                if (!_ready) throw new InvalidOperationException("Tried to subscribe without initialization");

                var reference = await SubscribeCommandFactory<ThermalStateUpdate, ThermalStateUpdateData>(listener);

                return reference;
            }


            /// <summary>
            /// Received when a user's voice state changes in a subscribed voice channel (mute, volume, etc). <br/> <br/>
            /// Scopes required: <c> rpc.voice.read </c> <br/>
            /// </summary>
            /// <exception cref="InvalidOperationException"></exception>
            public async static Task<SubscriptionReference> VoiceStateUpdate(string channelId, Action<UserVoiceState> listener)
            {
                if (!_ready) throw new InvalidOperationException("Tried to subscribe without initialization");

                var reference = await SubscribeCommandFactory<VoiceStateUpdate, UserVoiceState>(listener, new EventArguments{ ChannelId = channelId });

                return reference;
            }

            // Private method used to simplify the subscription methods
            private async static Task<SubscriptionReference> SubscribeCommandFactory<TEvent, TEventData>(Action<TEventData> listener, object? args = null) where TEvent : DiscordEvent
            {
                string eventString = EventUtility.GetStringFromType(typeof(TEvent));

                //\ Create reader
                var reader = new MessageBusReaderIndefinite((discordEvent) =>
                {
                    var castedEvent = discordEvent as TEvent;

                    if (castedEvent == null) return;

                    listener((TEventData) castedEvent.Data!);
                });

                //\ Create reference
                var reference = new SubscriptionReference();
                reference.SaveSubscriptionData(reader, eventString);

                //\ Save reader in message bus
                messageBus.AddReader(eventString, reader);

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
        /// <exception cref="AuthorizationException"></exception>
        /// <exception cref="JsonException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static Task<MultiEvent> Initialize()
        {
            if (_initialized) throw new InvalidOperationException("Already attempted to initialize");
            _initialized = true;

            _configuration = DissonityConfigAttribute.GetUserConfig();
            _clientId = _configuration.ClientId;

            //\ Create bridge instance
            bridge = new GameObject("_DissonityBridge");
            var bridgeComponent = bridge.AddComponent<DissonityBridge>();
            GameObject.DontDestroyOnLoad(bridge);

            //\ Prepare initialization task
            var tcs = new TaskCompletionSource<MultiEvent>();

            //? Not running inside Discord
            if (isEditor)
            {
                if (_mock)
                {
                    //\ Query data
                    var mock = GameObject.FindObjectOfType<DiscordMock>();

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
            _disableMock = true;

            Listen();

            //\ Request query
            bridgeComponent.queryAction += (query) =>
            {
                InitializeQuery(tcs, query);
            };

            RequestQuery();

            return tcs.Task;
        }

        /// <summary>
        /// Use this method to easily access external resources.
        /// If you need to use it before initialization, consider using <c> Mappings </c> and <c> PatchUrlMappingsConfig </c> from the <c> DissonityConfig </c> instead. <br/> <br/>
        /// https://discord.com/developers/docs/activities/development-guides#using-external-resources
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public static Task PatchUrlMappings(MappingBuilder[] mappings, PatchUrlMappingsConfigBuilder config)
        {
            if (_initialized) throw new InvalidOperationException("Tried to use a method without initialization");

            var tcs = new TaskCompletionSource<bool>();

            if (_mock)
            {
                if (!_configuration!.DisableDissonityInfoLogs) Utils.DissonityLog("Mock patch url mappings called via API method");

                tcs.TrySetResult(true);
                return tcs.Task;
            }

            bridge!.GetComponent<DissonityBridge>().patchUrlMappingsAction += () =>
            {
                tcs.TrySetResult(true);
            };

            var payload = new PatchUrlMappings()
            {
                Mappings = mappings,
                Config = config
            };

            RequestPatchUrlMappings(JsonConvert.SerializeObject(payload));

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
            if (!_ready) throw new InvalidOperationException("Tried to unsubscribe without initialization");
            
            if (!messageBus.ReaderSetDictionary.ContainsKey(reference.EventString)) throw new ArgumentException($"Tried to unsubscribe from a SubscriptionReference that no longer exists ({reference.EventString})");

            var readerSet = messageBus.ReaderSetDictionary[reference.EventString];

            //? Reader set has reader
            if (!readerSet.Contains(reference.Reader)) throw new ArgumentException("Tried to unsubscribe from a SubscriptionReference that no longer exists");
        
            bool removedSet = messageBus.RemoveReader(reference.EventString, reference.Reader);

            //? Should unsubscribe from RPC
            if (removedSet && !_mock)
            {
                subscribedRpcSet.Remove(reference.EventString);
                SendCommand<Unsubscribe, SubscribeResponse>(new (reference.EventString));
            }
        }

        /// <summary>
        /// Remove all subscriptions related to a single event.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static void UnsubscribeFromEvent(string eventString)
        {
            if (!_ready) throw new InvalidOperationException("Tried to unsubscribe without initialization");
            
            if (!messageBus.ReaderSetDictionary.ContainsKey(eventString)) throw new ArgumentException($"Tried to unsubscribe from an event that had no subscriptions ({eventString})");

            messageBus.ReaderSetDictionary.Remove(eventString);

            //\ Unsubscribe from RPC
            if (_mock) return;

            subscribedRpcSet.Remove(eventString);
            SendCommand<Unsubscribe, SubscribeResponse>(new (eventString));
        }

        /// <summary>
        /// Remove all subscriptions from every event.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public static void ClearAllSubscriptions()
        {
            if (!_ready) throw new InvalidOperationException("Tried to unsubscribe without initialization");

            messageBus.ReaderSetDictionary.Clear();

            if (_mock) return;

            foreach (string key in subscribedRpcSet)
            {
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
            if (!_ready && !_closedState) throw new InvalidOperationException("Tried to close the app without initialization");

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
                tcs.TrySetException(new JsonException("Something went wrong attempting to read the query."));
                return;
            }

            //# QUERY PARAMS - - - - -
            // Frame id
            if (query.FrameId == null)
            {
                // RpcBridge state should only be OutsideDiscord here

                if (!_configuration!.DisableDissonityInfoLogs)
                {
                    Utils.DissonityLogWarning("'frame_id' query param is not defined - Running outside of Discord or too nested to access query. Initialization is canceled.");
                }

                tcs.TrySetException(new OutsideDiscordException());
                return;
            }
            _frameId = query.FrameId;

            // Instance id
            if (query.InstanceId == null)
            {
                tcs.TrySetException(new ArgumentException("instance_id query param is not defined"));
                return;
            }
            _instanceId = query.InstanceId;

            // Platform
            if (query.Platform == null)
            {
                tcs.TrySetException(new ArgumentException("platform query param is not defined"));
                return;
            }
            else if (query.Platform != Models.Platform.Desktop && query.Platform != Models.Platform.Mobile)
            {
                tcs.TrySetException(new ArgumentException($"Invalid query param 'platform' of '${query.Platform}'. Valid values are '${Models.Platform.Desktop}' or '${Models.Platform.Mobile}'"));
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

            //\ Request state
            bridge!.GetComponent<DissonityBridge>().stateAction += (code) =>
            {
                if (!_configuration!.DisableDissonityInfoLogs)
                {
                    //? Problem
                    if (code == BridgeStateCode.Errored || code == BridgeStateCode.OutsideDiscord)
                    {
                        Utils.DissonityLogError($"Bridge returned unexpected state code: {code}");
                    }

                    else Utils.DissonityLog($"Bridge returned state code: {code}");
                }

                //? If the RpcBridge is closed, user denied authorization
                if (code == BridgeStateCode.Closed)
                {
                    _closedState = true;
                    tcs.TrySetException(new AuthorizationException());
                }
            };

            //\ Listen for multi event
            bridge.GetComponent<DissonityBridge>().multiEventAction += (multiEvent) =>
            {
                // OverrideConsoleLogging is done in the BridgeLib
                _ready = true;
                _userId = multiEvent.AuthenticateData.User.Id;
                tcs.TrySetResult(multiEvent);
            };

            // After requesting the state, the RpcBridge will send the multi event (once ready) through <DissonityBridge>.ReceiveMultiEvent
            RequestState();

            // The handshake is handled by the RpcBridge even before the Unity build loads
        }

        private static Task<TResponse> SendCommand<TCommand, TResponse>(TCommand command) where TCommand : DiscordCommand<TResponse> where TResponse : DiscordEvent
        {
            if (!_initialized)
            {
                throw new InvalidOperationException("Tried to send a command without initialization");
            }

            var tcs = new TaskCompletionSource<TResponse>();

            // Only the handshake command lacks a nonce
            string commandNonce = (command.Guid == null)
                ? handshakeStringId
                : command.Guid.ToString();


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

        private static void SendToBridge<TCommand, TResponse>(TCommand command) where TCommand : DiscordCommand<TResponse> where TResponse : DiscordEvent
        {
            if (!_initialized)
            {
                throw new InvalidOperationException("Tried to send to bridge without initialization");
            }

            object payload;

            if (command is FrameCommand<TResponse> frameCommand)
            {
                payload = new SerializableFrameCommand<TResponse>(frameCommand);
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
        
        private static Task<TResponse> MockSendCommand<TResponse>(object? arg = null) where TResponse : DiscordEvent
        {
            if (!_mock) throw new InvalidOperationException("This method can only be called in mock mode");
            if (!_initialized)
            {
                throw new InvalidOperationException("Tried to send a command without initialization");
            }

            var tcs = new TaskCompletionSource<TResponse>();
            DiscordMock mock = GameObject.FindObjectOfType<DiscordMock>();


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
                    string id = ChannelId!;
                    MockChannel? mockChannel = mock.channels.Find(c => c.Id == id)?.Value;

                    if (mockChannel != null)
                    {
                        response.Data.Permissions = mockChannel.ChannelPermissions.Permissions;
                    }

                    ((TaskCompletionSource<GetChannelPermissionsResponse>) (object) tcs).TrySetResult(response);
                }

                // GetChannelResponse
                else if (typeof(TResponse) == typeof(GetChannelResponse))
                {
                    var response = new GetChannelResponse();

                    response.Data = new();

                    //? Channel id is an actual mock channel
                    MockChannel? mockChannel = mock.channels.Find(c => c.Id == (string) arg!)?.Value;

                    if (mockChannel != null)
                    {
                        response.Data = mockChannel.ChannelData.ToChannelData();
                    }

                    else if (!_configuration!.DisableDissonityInfoLogs) Utils.DissonityLogWarning("You can get mock channel data by calling Api.Commands.GetChannel with a mock channel id");

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

                // UserSettingsGetLocaleResponse
                else if (typeof(TResponse) == typeof(UserSettingsGetLocaleResponse))
                {
                    var response = new UserSettingsGetLocaleResponse();

                    response.Data = new()
                    {
                        Locale = MockUtils.ToLocaleString(mock.locale)
                    };

                    ((TaskCompletionSource<UserSettingsGetLocaleResponse>) (object) tcs).TrySetResult(response);
                }

                else
                    ((TaskCompletionSource<NoResponse>) (object) tcs).TrySetResult(new NoResponse());
            }

            return tcs.Task;
        }

        internal static void EnableMock()
        {
            if (_disableMock) return;
            _mock = true;
        }
    }
}
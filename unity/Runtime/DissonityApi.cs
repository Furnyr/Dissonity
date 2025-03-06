/*
    Class used by the users to interact with Dissonity.
    If you have any problem open an issue at https://github.com/Furnyr/Dissonity
*/

using System;
using System.Threading.Tasks;
using UnityEngine;

#if UNITY_WEBGL && !UNITY_EDITOR
    using System.Runtime.InteropServices;
#endif

namespace Dissonity
{
    //* Not yet released features are not implemented in the API
    public static class Api {

        //# FIELDS - - - - -
        public static bool bridgeInitialized = false;
        public static bool npmLoaded = false;
        #nullable enable
            public static string? cachedApplicationId = null;
            public static string? cachedInstanceId = null;
            public static string? cachedChannelId = null;
            public static string? cachedGuildId = null;
            public static string? cachedUserId = null;
        #nullable disable


        //# DEBUG - - - - -
        #nullable enable
            public static string? OverrideApplicationId = null;
            public static string? OverrideInstanceId = null;
            public static string? OverrideUserId = null;
            public static string? OverrideUserGlobalName = null;
            public static string? OverrideUsername = null;
            public static string? OverrideUserAvatar = null;
            public static bool? OverrideHardwareAcceleration = null;
            public static string? OverrideUserLocale = null;
        #nullable disable

        // If this method is called inside the editor, it logs as normal,
        // if it's called outside the editor, the log is prefixed with
        // [Dissonity]: to make it easy to filter in a browser dev console.
        public static void DissonityLog (object message) {

            #if UNITY_WEBGL && !UNITY_EDITOR
                UnityEngine.Debug.Log($"[Dissonity]: {message}");
            #else
                UnityEngine.Debug.Log(message);
            #endif
        }

        public static void DissonityWarn (object message) {

            #if UNITY_WEBGL && !UNITY_EDITOR
                UnityEngine.Debug.LogWarning($"[Dissonity]: {message}");
            #else
                UnityEngine.Debug.LogWarning(message);
            #endif
        }
        

        //# DELEGATES - - - - -
        // Subscriptions
        public delegate void VoiceStateUpdateDelegate (VoiceStateUpdateData data);
        public delegate void SpeakingDelegate (SpeakingData data);
        public delegate void ActivityLayoutModeUpdateDelegate (ActivityLayoutModeUpdateData data);
        public delegate void OrientationUpdateDelegate (OrientationUpdateData data);
        public delegate void CurrentUserUpdateDelegate (CurrentUserUpdateData data);
        public delegate void EntitlementCreateDelegate (EntitlementCreateData data);
        public delegate void ThermalStateUpdateDelegate (ThermalStateUpdateData data);
        public delegate void InstanceParticipantsDelegate (InstanceParticipantsData data);

        // Non-subscriptions
        public delegate void GetStringDelegate (string str);
        public delegate void GetUserDelegate (User user);
        public delegate void HardwareAccelerationDelegate (HardwareAccelerationData data);
        public delegate void GetChannelDelegate (Channel channel);
        public delegate void GetChannelPermissionsDelegate (ChannelPermissionsData data);
        public delegate void GetPlatformBehaviorsDelegate (PlatformBehaviorsData data);
        public delegate void SetActivityDelegate (Activity activity);
        public delegate void GetLocaleDelegate (LocaleData data);
        public delegate void SetConfigDelegate (ConfigData data);
        public delegate void ImageUploadDelegate (ImageUploadData data);


        //# EVENTS - - - - -
        // Subscriptions
        internal static VoiceStateUpdateDelegate _VoiceStateUpdateEvent;
        internal static SpeakingDelegate _SpeakingStartEvent;
        internal static SpeakingDelegate _SpeakingStopEvent;
        internal static ActivityLayoutModeUpdateDelegate _ActivityLayoutModeUpdateEvent;
        internal static OrientationUpdateDelegate _OrientationUpdateEvent;
        internal static CurrentUserUpdateDelegate _CurrentUserUpdateEvent;
        internal static EntitlementCreateDelegate _EntitlementCreateEvent;
        internal static ThermalStateUpdateDelegate _ThermalStateUpdateEvent;
        internal static InstanceParticipantsDelegate _ActivityInstanceParticipantsUpdateEvent;

        // Non-subscriptions
        internal static Action _LoadEvent;
        internal static GetStringDelegate _GetApplicationIdEvent;
        internal static GetStringDelegate _GetInstanceIdEvent;
        internal static GetStringDelegate _GetChannelIdEvent;
        internal static GetStringDelegate _GetGuildIdEvent;
        internal static GetStringDelegate _GetUserIdEvent;
        internal static GetUserDelegate _GetUserEvent;
        internal static InstanceParticipantsDelegate _GetInstanceParticipantsEvent;
        internal static HardwareAccelerationDelegate _HardwareAccelerationEvent;
        internal static GetChannelDelegate _GetChannelEvent;
        internal static GetChannelPermissionsDelegate _GetChannelPermissionsEvent;
        internal static GetPlatformBehaviorsDelegate _GetPlatformBehaviorsEvent;
        internal static SetActivityDelegate _SetActivityEvent;
        internal static GetLocaleDelegate _GetLocaleEvent;
        internal static SetConfigDelegate _SetConfigEvent;
        internal static ImageUploadDelegate _ImageUploadEvent;

        //# JAVASCRIPT PLUGIN - - - - 
        #if UNITY_WEBGL && !UNITY_EDITOR
            [DllImport("__Internal")]
            private static extern void InitializeIFrameBridge();

            [DllImport("__Internal")]
            private static extern void IFrameBridge();   // MUST be imported, it's called internally

            [DllImport("__Internal")]
            private static extern string Subscribe(string ev);

            [DllImport("__Internal")]
            private static extern string Unsubscribe(string ev);

            [DllImport("__Internal")]
            private static extern string RequestSetActivity(string stringifiedActivity);

            [DllImport("__Internal")]
            private static extern string RequestApplicationId();

            [DllImport("__Internal")]
            private static extern string RequestInstanceId();

            [DllImport("__Internal")]
            private static extern string RequestChannelId();

            [DllImport("__Internal")]
            private static extern string RequestGuildId();

            [DllImport("__Internal")]
            private static extern string RequestUserId();

            [DllImport("__Internal")]
            private static extern string RequestUser();

            [DllImport("__Internal")]
            private static extern string RequestInstanceParticipants();

            [DllImport("__Internal")]
            private static extern string RequestHardwareAcceleration();

            [DllImport("__Internal")]
            private static extern string RequestChannel(string channelId);

            [DllImport("__Internal")]
            private static extern string RequestChannelPermissions(string channelId);

            [DllImport("__Internal")]
            private static extern string RequestPlatformBehaviors();

            [DllImport("__Internal")]
            private static extern string RequestImageUpload();

            [DllImport("__Internal")]
            private static extern string RequestOpenExternalLink(string url);

            [DllImport("__Internal")]
            private static extern string RequestInviteDialog();

            [DllImport("__Internal")]
            private static extern string RequestShareMomentDialog(string mediaUrl);

            [DllImport("__Internal")]
            private static extern string RequestSetOrientationLockState(string lockState, string pictureInPictureLockState, string gridLockState);
        
            [DllImport("__Internal")]
            private static extern string RequestLocale();

            [DllImport("__Internal")]
            private static extern string RequestSetConfig(string useInteractivePip); // string representation of bool

            [DllImport("__Internal")]
            private static extern void PingLoad();
        #endif
    

        //\ Initialize bridge
        //! You don't need to call this, it's automatically called by the DiscordBridge.
        public static void InitializeSDKBridge () {

            if (bridgeInitialized) return;

            #if UNITY_WEBGL && !UNITY_EDITOR
                InitializeIFrameBridge();
            #endif
        }

        //\ Await until the npm package is loaded
        public static Task WaitForLoad () {

            var tcs = new TaskCompletionSource<bool>();

            //? Already loaded
            if (npmLoaded) {

                tcs.TrySetResult(true);
                return tcs.Task;
            }

            //\ Add listener
            _LoadEvent += () => {

                npmLoaded = true;
                tcs.TrySetResult(true);
            };

            #if UNITY_WEBGL && !UNITY_EDITOR
                PingLoad();
            #endif

            return tcs.Task;
        }


        //# SUBSCRIBE METHODS - - - - -
        public static void SubVoiceStateUpdate (VoiceStateUpdateDelegate del, bool soft = false) {

            _VoiceStateUpdateEvent += del;

            if (soft) return;

            #if UNITY_WEBGL && !UNITY_EDITOR
                Subscribe("VOICE_STATE_UPDATE");
            #endif
        }

        public static void SubSpeakingStart (SpeakingDelegate del, bool soft = false) {

            _SpeakingStartEvent += del;

            if (soft) return;

            #if UNITY_WEBGL && !UNITY_EDITOR
                Subscribe("SPEAKING_START");
            #endif
        }

        public static void SubSpeakingStop (SpeakingDelegate del, bool soft = false) {

            _SpeakingStopEvent += del;

            if (soft) return;

            #if UNITY_WEBGL && !UNITY_EDITOR
                Subscribe("SPEAKING_STOP");
            #endif
        }

        public static void SubActivityLayoutModeUpdate (ActivityLayoutModeUpdateDelegate del, bool soft = false) {

            _ActivityLayoutModeUpdateEvent += del;

            if (soft) return;

            #if UNITY_WEBGL && !UNITY_EDITOR
                Subscribe("ACTIVITY_LAYOUT_MODE_UPDATE");
            #endif
        }

        public static void SubOrientationUpdate (OrientationUpdateDelegate del, bool soft = false) {

            _OrientationUpdateEvent += del;

            if (soft) return;

            #if UNITY_WEBGL && !UNITY_EDITOR
                Subscribe("ORIENTATION_UPDATE");
            #endif
        }

        public static void SubCurrentUserUpdate (CurrentUserUpdateDelegate del, bool soft = false) {

            _CurrentUserUpdateEvent += del;

            if (soft) return;

            #if UNITY_WEBGL && !UNITY_EDITOR
                Subscribe("CURRENT_USER_UPDATE");
            #endif
        }

        public static void SubThermalStateUpdate (ThermalStateUpdateDelegate del, bool soft = false) {

            _ThermalStateUpdateEvent += del;

            if (soft) return;

            #if UNITY_WEBGL && !UNITY_EDITOR
                Subscribe("THERMAL_STATE_UPDATE");
            #endif
        }

        public static void SubActivityInstanceParticipantsUpdate (InstanceParticipantsDelegate del, bool soft = false) {

            _ActivityInstanceParticipantsUpdateEvent += del;

            if (soft) return;

            #if UNITY_WEBGL && !UNITY_EDITOR
                Subscribe("ACTIVITY_INSTANCE_PARTICIPANTS_UPDATE");
            #endif
        }


        //# UNSUBSCRIBE METHODS - - - - -
        public static void UnsubVoiceStateUpdate (VoiceStateUpdateDelegate del, bool soft = false) {

            _VoiceStateUpdateEvent -= del;

            if (soft) return;

            #if UNITY_WEBGL && !UNITY_EDITOR
                Unsubscribe("VOICE_STATE_UPDATE");
            #endif
        }

        public static void UnsubSpeakingStart (SpeakingDelegate del, bool soft = false) {

            _SpeakingStartEvent -= del;

            if (soft) return;

            #if UNITY_WEBGL && !UNITY_EDITOR
                Unsubscribe("SPEAKING_START");
            #endif
        }

        public static void UnsubSpeakingStop (SpeakingDelegate del, bool soft = false) {

            _SpeakingStopEvent -= del;

            if (soft) return;

            #if UNITY_WEBGL && !UNITY_EDITOR
                Unsubscribe("SPEAKING_STOP");
            #endif
        }

        public static void UnsubActivityLayoutModeUpdate (ActivityLayoutModeUpdateDelegate del, bool soft = false) {

            _ActivityLayoutModeUpdateEvent -= del;

            if (soft) return;

            #if UNITY_WEBGL && !UNITY_EDITOR
                Unsubscribe("ACTIVITY_LAYOUT_MODE_UPDATE");
            #endif
        }

        public static void UnsubOrientationUpdate (OrientationUpdateDelegate del, bool soft = false) {

            _OrientationUpdateEvent -= del;

            if (soft) return;

            #if UNITY_WEBGL && !UNITY_EDITOR
                Unsubscribe("ORIENTATION_UPDATE");
            #endif
        }

        public static void UnsubCurrentUserUpdate (CurrentUserUpdateDelegate del, bool soft = false) {

            _CurrentUserUpdateEvent -= del;

            if (soft) return;

            #if UNITY_WEBGL && !UNITY_EDITOR
                Unsubscribe("CURRENT_USER_UPDATE");
            #endif
        }

        public static void UnsubThermalStateUpdate (ThermalStateUpdateDelegate del, bool soft = false) {

            _ThermalStateUpdateEvent -= del;

            if (soft) return;

            #if UNITY_WEBGL && !UNITY_EDITOR
                Unsubscribe("THERMAL_STATE_UPDATE");
            #endif
        }

        public static void UnsubActivityInstanceParticipantsUpdate (InstanceParticipantsDelegate del, bool soft = false) {

            _ActivityInstanceParticipantsUpdateEvent -= del;

            if (soft) return;

            #if UNITY_WEBGL && !UNITY_EDITOR
                Unsubscribe("ACTIVITY_INSTANCE_PARTICIPANTS_UPDATE");
            #endif
        }


        //# NON-SUBSCRIBE METHODS - - - - -
        public static Task<string> GetApplicationId () {

            var tcs = new TaskCompletionSource<string>();

            //? Cached
            if (cachedApplicationId != null) {

                tcs.TrySetResult(cachedApplicationId);
                return tcs.Task;
            }

            _GetApplicationId((id) => {

                tcs.TrySetResult(id);
            });

            #if UNITY_EDITOR

                tcs.TrySetResult(OverrideApplicationId ?? "ph_application_id");
            #endif

            return tcs.Task;
        }
        
        public static Task<string> GetSDKInstanceId () {

            var tcs = new TaskCompletionSource<string>();

            //? Cached
            if (cachedInstanceId != null) {

                tcs.TrySetResult(cachedInstanceId);
                return tcs.Task;
            }

            _GetSDKInstanceId((id) => {

                tcs.TrySetResult(id);
            });

            #if UNITY_EDITOR

                tcs.TrySetResult(OverrideInstanceId ?? "ph_instance_id");
            #endif

            return tcs.Task;
        }

        public static Task<string> GetChannelId () {

            var tcs = new TaskCompletionSource<string>();

            _GetChannelId((id) => {
                tcs.TrySetResult(id);
            });

            #if UNITY_EDITOR
                tcs.TrySetResult("ph_channel_id");
            #endif

            return tcs.Task;
        }

        public static Task<string> GetGuildId () {

            var tcs = new TaskCompletionSource<string>();

            _GetGuildId((id) => {
                tcs.TrySetResult(id);
            });

            #if UNITY_EDITOR
                tcs.TrySetResult("ph_guild_id");
            #endif

            return tcs.Task;
        }

        public static Task<string> GetUserId () {

            var tcs = new TaskCompletionSource<string>();

            _GetUserId((id) => {
                tcs.TrySetResult(id);
            });

            #if UNITY_EDITOR

                tcs.TrySetResult(OverrideUserId ?? "ph_user_id");
            #endif

            return tcs.Task;
        }

        public static Task<User> GetUser () {

            var tcs = new TaskCompletionSource<User>();

            _GetUser((user) => {
                tcs.TrySetResult(user);
            });

            #if UNITY_EDITOR

                // Editor placeholder data
                tcs.TrySetResult(new User {
                    id =  OverrideUserId ?? "ph_user_id",
                    global_name = OverrideUserGlobalName ?? "ph_user_global_name",
                    username = OverrideUsername ?? "ph_user_name",
                    avatar = OverrideUserAvatar ?? "ph_user_avatar"
                });
            #endif

            return tcs.Task;
        }

        public static Task<InstanceParticipantsData> GetInstanceParticipants () {

            var tcs = new TaskCompletionSource<InstanceParticipantsData>();

            _GetInstanceParticipants((data) => {
                tcs.TrySetResult(data);
            });

            #if UNITY_EDITOR

                // If this is run inside the editor, there's no way to test it
                UnityEngine.Debug.LogWarning("Called GetInstanceParticipants inside editor, unexpected behaviour will occur");
            #endif

            return tcs.Task;
        }

        public static Task EncourageHardwareAcceleration () {

            var tcs = new TaskCompletionSource<object>();

            _EncourageHardwareAcceleration((data) => {
                tcs.TrySetResult(data);
            });

            #if UNITY_EDITOR

                if (OverrideHardwareAcceleration == null) {
                    UnityEngine.Debug.LogWarning("Called EncourageHardwareAcceleration inside editor, use overrides to test it inside Unity");
                }

                // Editor placeholder data
                tcs.TrySetResult(new HardwareAccelerationData{
                    enabled = OverrideHardwareAcceleration ?? false
                });
            #endif

            return tcs.Task;
        }

        public static Task<Channel> GetChannel (string channelId) {

            var tcs = new TaskCompletionSource<Channel>();

            _GetChannel(channelId, (channel) => {
                tcs.TrySetResult(channel);
            });

            #if UNITY_EDITOR

                // Editor placeholder data
                tcs.TrySetResult(new Channel{
                    id =  "ph_channel_id",
                    guild_id =  "ph_guild_name",
                    name =  "ph_channel_name",
                    topic =  "ph_channel_topic",
                });
            #endif

            return tcs.Task;
        }

        public static Task<ChannelPermissionsData> GetChannelPermissions (string channelId) {

            var tcs = new TaskCompletionSource<ChannelPermissionsData>();

            _GetChannelPermissions(channelId, (data) => {
                tcs.TrySetResult(data);
            });

            #if UNITY_EDITOR

                UnityEngine.Debug.LogWarning("Called GetChannelPermissions inside editor, unexpected behaviour will occur");

                // Editor placeholder data
                tcs.TrySetResult(new ChannelPermissionsData{
                    permissions = "ph_channel_permissions"
                });
            #endif

            return tcs.Task;
        }

        public static Task<PlatformBehaviorsData> GetPlatformBehaviors () {

            var tcs = new TaskCompletionSource<PlatformBehaviorsData>();

            _GetPlatformBehaviors((data) => {
                tcs.TrySetResult(data);
            });

            #if UNITY_EDITOR

                // Editor placeholder data
                tcs.TrySetResult(new PlatformBehaviorsData{
                    iosKeyboardResizesView = false
                });
            #endif

            return tcs.Task;
        }

        public static Task<ImageUploadData> InitiateImageUpload () {

            var tcs = new TaskCompletionSource<ImageUploadData>();

            _InitiateImageUpload((data) => {
                tcs.TrySetResult(data);
            });

            #if UNITY_EDITOR

                // If this is run inside the editor, there's no way to test it
                UnityEngine.Debug.LogWarning("Called InitiateImageUpload inside editor, unexpected behaviour will occur");
            #endif

            return tcs.Task;
        }

        public static void OpenExternalLink (string url) {

            #if UNITY_WEBGL && !UNITY_EDITOR
                RequestOpenExternalLink(url);
            #else

                // If this is run inside the editor, there's no way to test it
                UnityEngine.Debug.LogWarning("Called OpenExternalLink inside editor, unexpected behaviour will occur");
            #endif
        }

        public static void OpenInviteDialog () {

            #if UNITY_WEBGL && !UNITY_EDITOR
                RequestInviteDialog();
            #else

                // If this is run inside the editor, there's no way to test it
                UnityEngine.Debug.LogWarning("Called OpenInviteDialog inside editor, unexpected behaviour will occur");
            #endif
        }

        public static void OpenShareMomentDialog (string mediaUrl) {

            #if UNITY_WEBGL && !UNITY_EDITOR
                RequestShareMomentDialog(mediaUrl);
            #else

                // If this is run inside the editor, there's no way to test it
                UnityEngine.Debug.LogWarning("Called OpenShareMomentDialog inside editor, unexpected behaviour will occur");
            #endif
        }

        public static Task<Activity> SetActivity (ActivityBuilder activity) {

            var tcs = new TaskCompletionSource<Activity>();

            string jsonString = JsonUtility.ToJson(activity);

            _SetActivity(jsonString, (data) => {
                tcs.TrySetResult(data);
            });

            #if UNITY_EDITOR

                // If this is run inside the editor, there's no way to test it
                UnityEngine.Debug.LogWarning("Called SetActivity inside editor");
            #endif

            return tcs.Task;
        }

        public static void SetOrientationLockState (string lockState, string pictureInPictureLockState = "", string gridLockState = "") {

            #if UNITY_WEBGL && !UNITY_EDITOR
                RequestSetOrientationLockState(lockState, pictureInPictureLockState, gridLockState);
            #else

                // If this is run inside the editor, there's no way to test it
                UnityEngine.Debug.LogWarning("Called SetOrientationLockState inside editor, unexpected behaviour will occur");
            #endif
        }

        public static Task<LocaleData> GetUserLocale () {

            var tcs = new TaskCompletionSource<LocaleData>();

            _GetUserLocale((data) => {
                tcs.TrySetResult(data);
            });

            #if UNITY_EDITOR

                if (OverrideUserLocale == null) {
                    UnityEngine.Debug.LogWarning("Called GetUserLocale inside editor, use overrides to test it inside Unity");
                }

                // Editor placeholder data
                tcs.TrySetResult(new LocaleData{
                    locale = OverrideUserLocale ?? "en-US"
                });
            #endif

            return tcs.Task;
        }

        public static Task<ConfigData> SetConfig (bool useInteractivePip) {

            var tcs = new TaskCompletionSource<ConfigData>();

            _SetConfig(useInteractivePip, (data) => {
                tcs.TrySetResult(data);
            });

            #if UNITY_EDITOR

                // If this is run inside the editor, there's no way to test it
                UnityEngine.Debug.LogWarning("Called SetConfig inside editor, unexpected behaviour will occur");
            #endif

            return tcs.Task;
        }


        // Private wrap methods
        private static void _GetApplicationId (GetStringDelegate del) {

            //? Not yet cached
            if (cachedApplicationId == null) {

                _GetApplicationIdEvent += del;

                #if UNITY_WEBGL && !UNITY_EDITOR
                    RequestApplicationId();
                #endif
            }

            //? Cached
            else {
                del(cachedApplicationId);
            }
        }

        private static void _GetSDKInstanceId (GetStringDelegate del) {

            //? Not yet cached
            if (cachedInstanceId == null) {

                _GetInstanceIdEvent += del;

                #if UNITY_WEBGL && !UNITY_EDITOR
                    RequestInstanceId();
                #endif
            }

            //? Cached
            else {
                del(cachedInstanceId);
            }
        }

        private static void _GetChannelId (GetStringDelegate del) {

            //? Not yet cached
            if (cachedChannelId == null) {

                _GetChannelIdEvent += del;

                #if UNITY_WEBGL && !UNITY_EDITOR
                    RequestChannelId();
                #endif
            }

            //? Cached
            else {
                del(cachedChannelId);
            }
        }

        private static void _GetGuildId (GetStringDelegate del) {

            //? Not yet cached
            if (cachedGuildId == null) {

                _GetGuildIdEvent += del;

                #if UNITY_WEBGL && !UNITY_EDITOR
                    RequestGuildId();
                #endif
            }

            //? Cached
            else {
                del(cachedGuildId);
            }
        }

        private static void _GetUserId (GetStringDelegate del) {

            //? Not yet cached
            if (cachedUserId == null) {

                _GetUserIdEvent += del;

                #if UNITY_WEBGL && !UNITY_EDITOR
                    RequestUserId();
                #endif
            }

            //? Cached
            else {
                del(cachedUserId);
            }
        }

        private static void _GetUser (GetUserDelegate del) {

            _GetUserEvent += del;

            #if UNITY_WEBGL && !UNITY_EDITOR
                RequestUser();
            #endif
        }

        private static void _GetInstanceParticipants (InstanceParticipantsDelegate del) {

            _GetInstanceParticipantsEvent += del;
            
            #if UNITY_WEBGL && !UNITY_EDITOR
                RequestInstanceParticipants();
            #endif
        }
    
        private static void _GetChannel (string channelId, GetChannelDelegate del) {

            _GetChannelEvent += del;

            #if UNITY_WEBGL && !UNITY_EDITOR
                RequestChannel(channelId);
            #endif
        }

        private static void _GetChannelPermissions (string channelId, GetChannelPermissionsDelegate del) {

            _GetChannelPermissionsEvent += del;

            #if UNITY_WEBGL && !UNITY_EDITOR
                RequestChannelPermissions(channelId);
            #endif
        }

        private static void _EncourageHardwareAcceleration (HardwareAccelerationDelegate del) {

            _HardwareAccelerationEvent += del;

            #if UNITY_WEBGL && !UNITY_EDITOR
                RequestHardwareAcceleration();
            #endif
        }

        private static void _GetPlatformBehaviors (GetPlatformBehaviorsDelegate del) {

            _GetPlatformBehaviorsEvent += del;

            #if UNITY_WEBGL && !UNITY_EDITOR
                RequestPlatformBehaviors();
            #endif
        }

        private static void _InitiateImageUpload (ImageUploadDelegate del) {

            _ImageUploadEvent += del;

            #if UNITY_WEBGL && !UNITY_EDITOR
                RequestImageUpload();
            #endif
        }

        private static void _SetActivity (string stringifiedActivity, SetActivityDelegate del) {

            _SetActivityEvent += del;

            #if UNITY_WEBGL && !UNITY_EDITOR
                RequestSetActivity(stringifiedActivity);
            #endif
        }
    
        private static void _GetUserLocale (GetLocaleDelegate del) {

            _GetLocaleEvent += del;

            #if UNITY_WEBGL && !UNITY_EDITOR
                RequestLocale();
            #endif
        }

        private static void _SetConfig (bool useInteractivePip, SetConfigDelegate del) {

            _SetConfigEvent += del;

            #if UNITY_WEBGL && !UNITY_EDITOR
                RequestSetConfig(useInteractivePip.ToString());
            #endif
        }
    }
}

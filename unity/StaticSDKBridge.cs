using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;

#if UNITY_WEBGL && !UNITY_EDITOR
    using System.Runtime.InteropServices;
#endif

// Delegates
public delegate void VoiceStateUpdateDelegate (VoiceStateUpdateData data);
public delegate void SpeakingDelegate (SpeakingData data);
public delegate void ActivityLayoutModeUpdateDelegate (ActivityLayoutModeUpdateData data);
public delegate void OrientationUpdateDelegate (OrientationUpdateData data);
public delegate void CurrentUserUpdateDelegate (CurrentUserUpdateData data);
public delegate void EntitlementCreateDelegate (EntitlementCreateData data);
public delegate void ThermalStateUpdateDelegate (ThermalStateUpdateData data);
public delegate void InstanceParticipantsDelegate (InstanceParticipantsData data);

public delegate void GetIdDelegate (string id);
public delegate void GetUserDelegate (User user);

// Will send data to the IFrameBridge and be read/written by the DynamicSDKBridge
public static class StaticSDKBridge
{
    //# DEBUG - - - - -
    // You can update these values to facilitate the development,
    // although not every SDK feature can be tested via overrides.
    #nullable enable
        private static string? OverrideInstanceId = null;
        private static string? OverrideUserId = null;
        private static string? OverrideUserGlobalName = null;
        private static string? OverrideUsername = null;
        private static string? OverrideUserAvatar = null;
    #nullable disable

    // When you call this method inside an activity,
    // the log will contain [UnityDebugLog] before to
    // make it easy to filter application logs,
    public static void DiscordDebugLog (object message) {

        #if UNITY_WEBGL && !UNITY_EDITOR
            UnityEngine.Debug.Log($"[UnityDebugLog]: {message}");
        #else
            UnityEngine.Debug.Log(message);
        #endif
    }


    //# JSLIB - - - - -
    #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void InitializeIFrameBridge();

        [DllImport("__Internal")]
        private static extern void IFrameBrige();   // MUST be imported, it's called internally

        [DllImport("__Internal")]
        private static extern string Subscribe(string ev);

        [DllImport("__Internal")]
        private static extern string Unsubscribe(string ev);

        [DllImport("__Internal")]
        private static extern string RequestInstanceId();

        [DllImport("__Internal")]
        private static extern string RequestChannelId();

        [DllImport("__Internal")]
        private static extern string RequestGuildId();

        [DllImport("__Internal")]
        private static extern string RequestUser();

        [DllImport("__Internal")]
        private static extern string RequestInstanceParticipants();
    #endif

    // Cache
    public static bool initialized = false;
    #nullable enable
        public static string? instanceId = null;
        public static string? channelId = null;
        public static string? guildId = null;
        public static User? user = null;
    #nullable disable


    //# HANDLERS - - - - -
    public static VoiceStateUpdateDelegate VoiceStateUpdateHandler;
    public static SpeakingDelegate SpeakingStartHandler;
    public static SpeakingDelegate SpeakingStopHandler;
    public static ActivityLayoutModeUpdateDelegate ActivityLayoutModeUpdateHandler;
    public static OrientationUpdateDelegate OrientationUpdateHandler;
    public static CurrentUserUpdateDelegate CurrentUserUpdateHandler;
    public static EntitlementCreateDelegate EntitlementCreateHandler;
    public static ThermalStateUpdateDelegate ThermalStateUpdateHandler;
    public static InstanceParticipantsDelegate ActivityInstanceParticipantsUpdateHandler;

    public static GetIdDelegate GetInstanceIdHandler;
    public static GetIdDelegate GetChannelIdHandler;
    public static GetIdDelegate GetGuildIdHandler;
    public static GetUserDelegate GetUserHandler;
    public static InstanceParticipantsDelegate GetInstanceParticipantsHandler;


    // Initialize bridge
    //! You don't need to call this, it's automatically called by DynamicSDKBridge
    public static void InitializeSDKBridge () {
        
        #if UNITY_WEBGL && !UNITY_EDITOR
            InitializeIFrameBridge();
        #endif
    }

    //# SUBSCRIBE METHODS - - - - -
    public static void SubVoiceStateUpdate (VoiceStateUpdateDelegate del, bool soft = false) {

        VoiceStateUpdateHandler += del;

        if (soft) return;

        #if UNITY_WEBGL && !UNITY_EDITOR
            Subscribe("VOICE_STATE_UPDATE");
        #endif
    }

    public static void SubSpeakingStart (SpeakingDelegate del, bool soft = false) {

        SpeakingStartHandler += del;

        if (soft) return;

        #if UNITY_WEBGL && !UNITY_EDITOR
            Subscribe("SPEAKING_START");
        #endif
    }

    public static void SubSpeakingStop (SpeakingDelegate del, bool soft = false) {

        SpeakingStopHandler += del;

        if (soft) return;

        #if UNITY_WEBGL && !UNITY_EDITOR
            Subscribe("SPEAKING_STOP");
        #endif
    }

    public static void SubActivityLayoutModeUpdate (ActivityLayoutModeUpdateDelegate del, bool soft = false) {

        ActivityLayoutModeUpdateHandler += del;

        if (soft) return;

        #if UNITY_WEBGL && !UNITY_EDITOR
            Subscribe("ACTIYIVY_LAYOUT_MODE_UPDATE");
        #endif
    }

    public static void SubOrientationUpdate (OrientationUpdateDelegate del, bool soft = false) {

        OrientationUpdateHandler += del;

        if (soft) return;

        #if UNITY_WEBGL && !UNITY_EDITOR
            Subscribe("ORIENTATION_UPDATE");
        #endif
    }

    public static void SubCurrentUserUpdate (CurrentUserUpdateDelegate del, bool soft = false) {

        CurrentUserUpdateHandler += del;

        if (soft) return;

        #if UNITY_WEBGL && !UNITY_EDITOR
            Subscribe("CURRENT_USER_UPDATE");
        #endif
    }

    public static void SubEntitlementCreate (EntitlementCreateDelegate del, bool soft = false) {

        EntitlementCreateHandler += del;

        if (soft) return;

        #if UNITY_WEBGL && !UNITY_EDITOR
            Subscribe("ENTITLEMENT_CREATE");
        #endif
    }

    public static void SubThermalStateUpdate (ThermalStateUpdateDelegate del, bool soft = false) {

        ThermalStateUpdateHandler += del;

        if (soft) return;

        #if UNITY_WEBGL && !UNITY_EDITOR
            Subscribe("THERMAL_STATE_UPDATE");
        #endif
    }

    public static void SubActivityInstanceParticipantsUpdate (InstanceParticipantsDelegate del, bool soft = false) {

        ActivityInstanceParticipantsUpdateHandler += del;

        if (soft) return;

        #if UNITY_WEBGL && !UNITY_EDITOR
            Subscribe("ACTIVITY_INSTANCE_PARTICIPANTS_UPDATE");
        #endif
    }


    //# UNSUBSCRIBE METHODS - - - - -
    public static void UnsubVoiceStateUpdate (VoiceStateUpdateDelegate del, bool soft = false) {

        VoiceStateUpdateHandler -= del;

        if (soft) return;

        #if UNITY_WEBGL && !UNITY_EDITOR
            Unsubscribe("VOICE_STATE_UPDATE");
        #endif
    }

    public static void UnsubSpeakingStart (SpeakingDelegate del, bool soft = false) {

        SpeakingStartHandler -= del;

        if (soft) return;

        #if UNITY_WEBGL && !UNITY_EDITOR
            Unsubscribe("SPEAKING_START");
        #endif
    }

    public static void UnsubSpeakingStop (SpeakingDelegate del, bool soft = false) {

        SpeakingStopHandler -= del;

        if (soft) return;

        #if UNITY_WEBGL && !UNITY_EDITOR
            Unsubscribe("SPEAKING_STOP");
        #endif
    }

    public static void UnsubActivityLayoutModeUpdate (ActivityLayoutModeUpdateDelegate del, bool soft = false) {

        ActivityLayoutModeUpdateHandler -= del;

        if (soft) return;

        #if UNITY_WEBGL && !UNITY_EDITOR
            Unsubscribe("ACTIYIVY_LAYOUT_MODE_UPDATE");
        #endif
    }

    public static void UnsubOrientationUpdate (OrientationUpdateDelegate del, bool soft = false) {

        OrientationUpdateHandler -= del;

        if (soft) return;

        #if UNITY_WEBGL && !UNITY_EDITOR
            Unsubscribe("ORIENTATION_UPDATE");
        #endif
    }

    public static void UnsubCurrentUserUpdate (CurrentUserUpdateDelegate del, bool soft = false) {

        CurrentUserUpdateHandler -= del;

        if (soft) return;

        #if UNITY_WEBGL && !UNITY_EDITOR
            Unsubscribe("CURRENT_USER_UPDATE");
        #endif
    }

    public static void UnsubEntitlementCreate (EntitlementCreateDelegate del, bool soft = false) {

        EntitlementCreateHandler -= del;

        if (soft) return;

        #if UNITY_WEBGL && !UNITY_EDITOR
            Unsubscribe("ENTITLEMENT_CREATE");
        #endif
    }

    public static void UnsubThermalStateUpdate (ThermalStateUpdateDelegate del, bool soft = false) {

        ThermalStateUpdateHandler -= del;

        if (soft) return;

        #if UNITY_WEBGL && !UNITY_EDITOR
            Unsubscribe("THERMAL_STATE_UPDATE");
        #endif
    }

    public static void UnsubActivityInstanceParticipantsUpdate (InstanceParticipantsDelegate del, bool soft = false) {

        ActivityInstanceParticipantsUpdateHandler -= del;

        if (soft) return;

        #if UNITY_WEBGL && !UNITY_EDITOR
            Unsubscribe("ACTIVITY_INSTANCE_PARTICIPANTS_UPDATE");
        #endif
    }


    //# "SUBSCRIBE" GET METHODS - - - - -
    public static Task<string> GetSDKInstanceId () {

        var tcs = new TaskCompletionSource<string>();

        _GetSDKInstanceId((id) => {

            tcs.TrySetResult(id);
        });

        #if UNITY_EDITOR

            if (OverrideInstanceId != null) tcs.TrySetResult(OverrideInstanceId);
            else tcs.TrySetResult("mockup_instance_id");
        #endif

        return tcs.Task;
    }

    public static Task<string> GetChannelId () {

        var tcs = new TaskCompletionSource<string>();

        _GetChannelId((id) => {
            tcs.TrySetResult(id);
        });

        #if UNITY_EDITOR
            tcs.TrySetResult("mockup_channel_id");
        #endif

        return tcs.Task;
    }

    public static Task<string> GetGuildId () {

        var tcs = new TaskCompletionSource<string>();

        _GetGuildId((id) => {
            tcs.TrySetResult(id);
        });

        #if UNITY_EDITOR
            tcs.TrySetResult("mockup_guild_id");
        #endif

        return tcs.Task;
    }

    public static Task<User> GetUser () {

        var tcs = new TaskCompletionSource<User>();

        _GetUser((user) => {
            tcs.TrySetResult(user);
        });

        #if UNITY_EDITOR

            // If this is run inside the editor, just use my data
            // for testing purposes
            tcs.TrySetResult(new User{
                id =  OverrideUserId ?? "mockup_user_id",
                global_name = OverrideUserGlobalName ?? "mockup_user_global_name",
                username = OverrideUsername ?? "mockup_user_name",
                avatar = OverrideUserAvatar ?? "mockup_user_avatar"
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

    private static void _GetSDKInstanceId (GetIdDelegate del) {

        //? Not yet cached
        if (instanceId == null) {

            GetInstanceIdHandler += del;

            #if UNITY_WEBGL && !UNITY_EDITOR
                RequestInstanceId();
            #endif
        }

        //? Cached
        else {
            del(instanceId);
        }
    }

    private static void _GetChannelId (GetIdDelegate del) {

        //? Not yet cached
        if (channelId == null) {

            GetChannelIdHandler += del;

            #if UNITY_WEBGL && !UNITY_EDITOR
                RequestChannelId();
            #endif
        }

        //? Cached
        else {
            del(channelId);
        }
    }

    private static void _GetGuildId (GetIdDelegate del) {

        //? Not yet cached
        if (guildId == null) {

            GetGuildIdHandler += del;

            #if UNITY_WEBGL && !UNITY_EDITOR
                RequestGuildId();
            #endif
        }

        //? Cached
        else {
            del(guildId);
        }
    }

    private static void _GetUser (GetUserDelegate del) {

        //? Not yet cached
        if (user == null) {

            GetUserHandler += del;

            #if UNITY_WEBGL && !UNITY_EDITOR
                RequestUser();
            #endif
        }

        //? Cached
        else {
            del(user);
        }
    }

    private static void _GetInstanceParticipants (InstanceParticipantsDelegate del) {

        GetInstanceParticipantsHandler += del;
        
        #if UNITY_WEBGL && !UNITY_EDITOR
            RequestInstanceParticipants();
        #endif
    }
}
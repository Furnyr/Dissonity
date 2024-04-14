/*
    Classes (only) used to parse data received as string.
    If you have any problem open an issue at https://github.com/Furnyr/Dissonity
*/

using System;

namespace Dissonity
{
    [Serializable]
    public class VoiceStateUpdateData
    {
        public bool mute;
        public string nick;
        public float number;
        public User user;
        public VoiceState voice_state;
        public float volume;
    }

    [Serializable]
    public class SpeakingData
    {
        #nullable enable
            public string? user_id;
            public string? channel_id;
            public string? lobby_id;
        #nullable disable
    }

    [Serializable]
    public class ActivityLayoutModeUpdateData
    {
        public int layout_mode; // 0 | 1 | -1
    }

    [Serializable]
    public class OrientationUpdateData
    {
        public string orientation; // "landscape" | "portrait"
        public int screen_orientation; // 0 | 1 | -1
    }

    [Serializable]
    public class CurrentUserUpdateData : User {}

    [Serializable]
    public class EntitlementCreateData
    {
        public Entitlement entitlement;
    }

    [Serializable]
    public class ThermalStateUpdateData
    {
        public int thermal_state;
    }

    [Serializable]
    public class InstanceParticipantsData
    {
        public Participant[] participants;
    }

    [Serializable]
    public class HardwareAccelerationData
    {
        public bool enabled;
    }

    [Serializable]
    public class ChannelPermissionsData
    {
        public string permissions;
    }

    //! Entitlements are not released yet for the developer preview.
    [Serializable]
    public class EntitlementsData
    {
        public Entitlement[] entitlements;
    }

    [Serializable]
    public class PlatformBehaviorsData
    {
        #nullable enable
            public bool? iosKeyboardResizesView;
        #nullable disable
    }

    //! Skus are not released yet for the developer preview.
    [Serializable]
    public class SkusData
    {
        public Sku[] skus;
    }

    [Serializable]
    public class LocaleData
    {
        public string locale;
    }

    [Serializable]
    public class ConfigData
    {
        public bool use_interactive_pip;
    }

    // This payload is not exactly like the one returned by the SDK,
    // but the canceled field is very useful to handle this sequence.
    [Serializable]
    public class ImageUploadData
    {
        public string image_url;
        public bool canceled;
    }
}

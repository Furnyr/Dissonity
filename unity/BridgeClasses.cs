
using System;

// Classes used to parse data received as string.
// I haven't tested all of them, so if you have any problem
// open an issue at https://github.com/Furnyr/Unity-Embedded-App-SDK

// You could also add classes here to implement more SDK features

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
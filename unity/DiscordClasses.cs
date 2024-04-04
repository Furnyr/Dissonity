using System;

[Serializable]
public class User {
    public string username = "mockup_username";
    public string id = "mockup_id";
    public bool bot = false;
    #nullable enable
        public AvatarDecoration? avatar_decoration_data = null;
        public string? global_name = null;
        public string? avatar = null;
        public int? flags = null;
        public int? premium_type = null;
    #nullable disable

    // You may use this property to easily get the name displayed for a user.
    // Notice this won't include guild nicknames
    public string display_name
    {
        get {
            if (global_name != null) {
                return global_name;
            }

            return username;
        }
    }
}

// For some reason, the SDK adds a nickname field in the ACTIVITY_INSTANCE_PARTICIPANTS_UPDATE data.
// Otherwise, these can be treated as users.
[Serializable]
public class Participant : User {
    #nullable enable
        public string? nickname = null;
    #nullable disable
}

[Serializable]
public class AvatarDecoration {
    public string asset = "mockup_asset";
    #nullable enable
        public string? sku_id = null;
    #nullable disable
}

[Serializable]
public class VoiceState {
    public bool deaf = false;
    public bool self_mute = false;
    public bool self_deaf = false;
    public bool suppress = false;
}

//! Entitlements are not release for the public preview of the Embedded App SDK.
[Serializable]
public class Entitlement {
    public string application_id = "mockup_application_id";
    public int gift_code_flags = 0;
    public string id = "mockup_id";
    public string sku_id = "mockup_sku_id";
    public int type = 0;
    public string user_id = "mockup_user_id";
    #nullable enable
        public string[]? branches = null;
        public bool? consumed = null;
        public bool? deleted = null;
        public string? ends_at = null;
        public string? gift_code_batch_id = null;
        public string? gifter_user_id = null;
        public string? parent_id = null;
        public string? starts_at = null;
    #nullable disable
}
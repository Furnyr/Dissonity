/*
    Classes used to represent Discord structures.
    If you have any problem open an issue at https://github.com/Furnyr/Dissonity
*/

using System;

namespace Dissonity
{
    [Serializable]
    public class User {
        public string username;
        public string id;
        public bool bot;
        #nullable enable
            public AvatarDecoration? avatar_decoration_data;
            public string? global_name;
            public string? avatar;
            public int? flags;
            public int? premium_type;
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

    [Serializable]
    public class Emoji {
        public string id;
        #nullable enable
            public string? name;
            public string[]? roles;
            public User? user;
            public bool? require_colons;
            public bool? managed;
            public bool? animated;
            public bool? available;
        #nullable disable
    }

    [Serializable]
    public class Reaction {
        public Emoji emoji;
        public int count;
        public bool me;
    }

    public class Member {
        public User user;
        public string[] roles;
        public string joined_at;
        public bool deaf;
        public bool mute;
        #nullable enable
            public string? nick;
        #nullable disable
    }

    // For some reason, the SDK adds a nickname field in the ACTIVITY_INSTANCE_PARTICIPANTS_UPDATE data.
    // Otherwise, these can be treated as users.
    [Serializable]
    public class Participant : User {
        #nullable enable
            public string? nickname;
        #nullable disable
    }

    [Serializable]
    public class AvatarDecoration {
        public string asset;
        #nullable enable
            public string? sku_id;
        #nullable disable
    }

    [Serializable]
    public class VoiceState {
        public bool deaf;
        public bool self_mute;
        public bool self_deaf;
        public bool suppress;
        #nullable enable
            public bool mute;   // Inconsistencies...
        #nullable disable
    }

    //! Entitlements are not released yet for the developer preview.
    [Serializable]
    public class Entitlement {
        public string application_id;
        public int gift_code_flags;
        public string id;
        public string sku_id;
        public int type;
        public string user_id;
        #nullable enable
            public string[]? branches;
            public bool? consumed;
            public bool? deleted;
            public string? ends_at;
            public string? gift_code_batch_id;
            public string? gifter_user_id;
            public string? parent_id;
            public string? starts_at;
        #nullable disable
    }

    //! Skus are not released yet for the developer preview.
    [Serializable]
    public class SkuPrize {
        public float amount;
        public string currency;
    }

    [Serializable]
    public class Sku {
        public int type;
        public string id;
        public string name;
        public int flags;
        public string application_id;
        public SkuPrize prize;
        #nullable enable
            public string? release_date;
        #nullable disable
    }

    [Serializable]
    public class MentionChannel {
        public int type;
        public string id;
        public string name;
        public string guild_id;
    }

    [Serializable]
    public class Attachment {
        public string id;
        public string url;
        public int size;
        public string filename;
        public string proxy_url;
        #nullable enable
            public int? height;
            public int? width;
        #nullable disable
    }

    [Serializable]
    public class EmbedFooter {
        public string text;
        #nullable enable
            public string? icon_url;
            public string? proxy_icon_url;
        #nullable disable
    }

    [Serializable]
    public class EmbedImage {
        #nullable enable
            public string? url;
            public string? proxy_url;
            public int? height;
            public int? width;
        #nullable disable
    }

    [Serializable]
    public class EmbedVideo {
        #nullable enable
            public string? url;
            public int? height;
            public int? width;
        #nullable disable
    }

    [Serializable]
    public class EmbedProvider {
        #nullable enable
            public string? name;
            public string? url;
        #nullable disable
    }

    [Serializable]
    public class EmbedAuthor {
        #nullable enable
            public string? name;
            public string? url;
            public string? icon_url;
            public string? proxy_icon_url;
        #nullable disable
    }

    [Serializable]
    public class EmbedFields {
        public string value;
        public string name;
        public bool inline;
    }

    [Serializable]
    public class Embed {
        #nullable enable
            public string? title;
            public string? type;
            public string? description;
            public string? url;
            public string? timestamp;
            public int? color;
            public EmbedFooter? footer;
            public EmbedImage? image;
            public EmbedImage? thumbnail;
            public EmbedVideo? video;
            public EmbedProvider? provider;
            public EmbedAuthor? author;
            public EmbedFields[]? fields;
        #nullable disable
    }

    [Serializable]
    public class Timestamp {
        #nullable enable
            public int? start;
            public int? end;
        #nullable disable
    }

    [Serializable]
    public class ActivityParty {
        #nullable enable
        public string? id;
        public int[]? size;
        #nullable disable
    }

    [Serializable]
    public class ActivityAssets {
        #nullable enable
            public string? large_image;
            public string? large_text;
            public string? small_image;
            public string? small_text;
        #nullable disable
    }

    [Serializable]
    public class ActivitySecrets {
        #nullable enable
            public string? join;
            public string? match;
        #nullable disable
    }

    [Serializable]
    public class ActivityBuilder {
        public int type;
        #nullable enable
            public Timestamp? timestamps;
            public string? details;
            public string? state;
            public ActivityParty? party;
            public ActivityAssets? assets;
            public ActivitySecrets? secrets;
            public bool? instance;
        #nullable disable
    }

    [Serializable]
    public class Activity : ActivityBuilder {
        public string name;
        #nullable enable
            public int? created_at;
            public string? application_id;
            public Emoji? emoji;
            public int? flags;
        #nullable disable
    }

    [Serializable]
    public class MessageActivity {
        public int type;
        #nullable enable
            public string? party_id;
        #nullable disable
    }

    [Serializable]
    public class Application {
        public string id;
        public string description;
        public string name;
        #nullable enable
            public string? cover_image;
            public string? icon;
        #nullable disable
    }

    [Serializable]
    public class MessageReference {
        #nullable enable
            public string? message_id;
            public string? channel_id;
            public string? guild_id;
        #nullable disable
    }

    [Serializable]
    public class Message  {
        public int type;
        public string id;
        public string timestamp;
        public string channel_id;
        public string content;
        public bool tts;
        public bool mention_everyone;
        public User[] mentions;
        public string[] mention_roles;
        public MentionChannel[] mention_channels;
        public Attachment[] attachments;
        public Embed[] embeds;
        public bool pinned;
        
        #nullable enable
            public string? guild_id;
            public User? user;
            public Member? member;
            public string? edited_timestamp;
            public Reaction[]? reaction;
            public string? nonce;
            public string? webhook_id;
            public MessageActivity? activity;
            public Application? application;
            public MessageReference? message_reference;
            public int? flags;
            // stickers and referenced_message are unknown types
        #nullable disable
    }

    // Another example of weirdly named SDK fields
    // (voice_states) in <sdkInstance>.commands.getChannel is, actually, not a VoiceState array
    [Serializable]
    public class UserVoiceState {
        public User user;
        public string nick;
        public bool mute;
        public VoiceState voice_state;
        public float volume;
    }

    [Serializable]
    public class Channel {
        public int type;
        public string id;
        public UserVoiceState[] voice_state;
        public Message[] messages;
        #nullable enable
            public string? guild_id;
            public string? name;
            public string? topic;
            public int? bitrate;
            public int? user_limit;
            public int? position;
        #nullable disable
    }
}

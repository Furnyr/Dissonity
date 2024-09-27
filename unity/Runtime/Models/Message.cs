using System;
using Newtonsoft.Json;

namespace Dissonity.Models
{
    [Serializable]
    public class Message
    {
        #nullable enable annotations

        [JsonProperty("id")]
        public string Id { get; set; }
        
        [JsonProperty("channel_id")]
        public string ChannelId { get; set; }

        [JsonProperty("guild_id")]
        public string? GuildId { get; set; }

        [JsonProperty("author")]
        public User? Author { get; set; }

        [JsonProperty("member")]
        public GuildMember? Member { get; set; }
        
        [JsonProperty("content")]
        public string Content { get; set; }
        
        [JsonProperty("timestamp")]
        public string Timestamp { get; set; }
        
        [JsonProperty("edited_timestamp")]
        public string? EditedTimestamp { get; set; }

        [JsonProperty("tts")]
        public bool TextToSpeech { get; set; }
        
        [JsonProperty("mention_everyone")]
        public bool MentionEveryone { get; set; }

        [JsonProperty("mentions", NullValueHandling = NullValueHandling.Ignore)]
        public User[] Mentions { get; set; }

        [JsonProperty("mention_roles", NullValueHandling = NullValueHandling.Ignore)]
        public string[] MentionRoles { get; set; }

        [JsonProperty("mention_channels", NullValueHandling = NullValueHandling.Ignore)]
        public ChannelMention[] MentionChannels { get; set; }
        
        [JsonProperty("attachments", NullValueHandling = NullValueHandling.Ignore)]
        public Attachment[] Attachments { get; set; }

        [JsonProperty("embeds", NullValueHandling = NullValueHandling.Ignore)]
        public Embed[] Embeds { get; set; }
        
        [JsonProperty("reactions", NullValueHandling = NullValueHandling.Ignore)]
        public Reaction[] Reactions { get; set; }

        [JsonProperty("nonce")]
        public string Nonce { get; set; }
        
        [JsonProperty("pinned")]
        public bool Pinned { get; set; }

        [JsonProperty("webhook_id")]
        public string? WebhookId { get; set; }
        
        [JsonProperty("type")]
        public int Type { get; set; }

        [JsonProperty("activity")]
        public MessageActivity? Activity { get; set; }

        [JsonProperty("application")]
        public MessageApplication? Application { get; set; }

        [JsonProperty("message_reference")]
        public MessageReference? MessageReference { get; set; }
        
        [JsonProperty("flags")]
        public int? Flags { get; set; }

        [JsonProperty("stickers", NullValueHandling = NullValueHandling.Ignore)]
        public object[] Stickers { get; set; }

        [JsonProperty("referenced_message")]
        public object? ReferencedMessage { get; set; }
    }
}
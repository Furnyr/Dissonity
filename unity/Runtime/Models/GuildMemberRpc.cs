using System;
using Dissonity.Events;
using Newtonsoft.Json;

namespace Dissonity.Models
{
    [Serializable]
    public class GuildMemberRpc
    {
        #nullable enable annotations

        [JsonProperty("user_id")]
        public long UserId { get; set; }

        [JsonProperty("guild_id")]
        public long GuildId { get; set; }

        [JsonProperty("avatar")]
        public string? Avatar { get; set; }

        [JsonProperty("avatar_decoration_data")]
        public AvatarDecoration? AvatarDecoration { get; set; }

        [JsonProperty("color_string")]
        public string? ColorString { get; set; }
        
        [JsonProperty("nick")]
        public string? Nickname { get; set; }

        // These are sent by the RPC but I don't think they are stable.

        // [JsonProperty("banner")]
        // public string? Banner { get; set; }

        // [JsonProperty("bio")]
        // public string? Bio { get; set; }

        // [JsonProperty("pronouns")]
        // public string? Pronouns { get; set; }
    }
}
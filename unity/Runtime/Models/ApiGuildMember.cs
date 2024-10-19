using System;
using Newtonsoft.Json;

namespace Dissonity.Models
{
    [Serializable]
    public class ApiGuildMember
    {
        #nullable enable annotations

        [JsonProperty("user")]
        public User? User { get; set; }

        [JsonProperty("avatar")]
        public string? Avatar { get; set; }

        [JsonProperty("nick")]
        public string? Nickname { get; set; }

        [JsonProperty("roles")]
        public long[] Roles { get; set; } = new long[0];

        [JsonProperty("joined_at")]
        public string JoinedAt { get; set; }

        [JsonProperty("premium_since")]
        public string? PremiumSince { get; set; }

        [JsonProperty("deaf")]
        public bool Deaf { get; set; }

        [JsonProperty("mute")]
        public bool Mute { get; set; }

        [JsonProperty("flags")]
        public long Flags { get; set; }

        [JsonProperty("pending")]
        public bool? Pending { get; set; }

        [JsonProperty("permissions")]
        public long? Permissions { get; set; }

        [JsonProperty("communication_disabled_until")]
        public string? CommunicationDisabledUntil { get; set; }

        [JsonProperty("avatar_decoration_data")]
        public AvatarDecoration? AvatarDecoration { get; set; }

        [JsonIgnore]
        public string DisplayName
        {
            get
            {
                if (Nickname != null) return Nickname;

                if (User.GlobalName != null) return User.GlobalName;

                return User.Username;
            }
        }
    }
}
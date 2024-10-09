using System;
using Newtonsoft.Json;

namespace Dissonity.Models
{
    [Serializable]
    public class GuildMember
    {
        #nullable enable annotations

        [JsonProperty("user")]
        public User User { get; set; }

        [JsonProperty("roles")]
        public string[] Roles { get; set; } = new string[0];

        [JsonProperty("joined_at")]
        public string JoinedAt { get; set; }

        [JsonProperty("deaf")]
        public bool Deaf { get; set; }

        [JsonProperty("mute")]
        public bool Mute { get; set; }

        [JsonProperty("nick")]
        public string? Nickname { get; set; }

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
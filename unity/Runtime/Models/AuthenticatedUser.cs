using System;
using Newtonsoft.Json;

namespace Dissonity.Models
{
    // Yet-another structure that the official SDK calls "user".
    [Serializable]
    public class AuthenticatedUser : BaseUser
    {
        #nullable enable annotations

        [JsonProperty("global_name")]
        public string? GlobalName { get; set; }

        [JsonProperty("accent_color")]
        public int? AccentColor { get; set; }

        [JsonProperty("avatar_decoration_data")]
        public AvatarDecoration? AvatarDecoration { get; set; }

        [JsonProperty("public_flags")]
        public long PublicFlags { get; set; }

        [JsonIgnore]
        public string DisplayName
        {
            get
            {
                if (GlobalName != null) return GlobalName;

                return Username;
            }
        }

        // [JsonProperty("clan")]
        // public string? Clan { get; set; }
    }
}
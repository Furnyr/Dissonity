using System;
using Dissonity.Events;
using Newtonsoft.Json;

namespace Dissonity.Models
{
    [Serializable]
    public class User : BaseUser
    {
        #nullable enable annotations

        [JsonProperty("global_name")]
        public string? GlobalName { get; set; }

        [JsonProperty("accent_color")]
        public int AccentColor { get; set; }

        [JsonProperty("avatar_decoration_data")]
        public AvatarDecoration? AvatarDecoration { get; set; }

        [JsonProperty("bot")]
        public bool Bot { get; set; }

        [JsonProperty("flags")]
        public long? Flags { get; set; }

        [JsonProperty("premium_type")]
        public PremiumType? PremiumType { get; set; }

        [JsonIgnore]
        public string DisplayName
        {
            get
            {
                if (GlobalName != null) return GlobalName;

                return Username;
            }
        }
    }
}
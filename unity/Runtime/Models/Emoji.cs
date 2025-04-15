using System;
using Newtonsoft.Json;

namespace Dissonity.Models
{
    [Serializable]
    public class Emoji
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("roles")]
        public long[] Roles { get; set; } = new long[0];
        
        [JsonProperty("user")]
        public User User { get; set; }
        
        [JsonProperty("require_colons")]
        public bool? RequireColons { get; set; }
        
        [JsonProperty("managed")]
        public bool? Managed { get; set; }

        [JsonProperty("animated")]
        public bool? Animated { get; set; }

        [JsonProperty("available")]
        public bool? Available { get; set; }
    }
}
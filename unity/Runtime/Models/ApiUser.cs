using System;
using Newtonsoft.Json;

namespace Dissonity.Models
{
    [Serializable]
    public class ApiUser : User
    {
        #nullable enable annotations

        [JsonProperty("system")]
        public bool? System { get; set; }

        [JsonProperty("mfa_enabled")]
        public bool? MfaEnabled { get; set; }

        [JsonProperty("banner")]
        public string? Banner { get; set; }

        [JsonProperty("locale")]
        public bool? Locale { get; set; }

        [JsonProperty("public_flags")]
        public long? PublicFlags { get; set; }
    }
}
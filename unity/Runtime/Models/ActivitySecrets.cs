using System;
using Newtonsoft.Json;

namespace Dissonity.Models
{
    [Serializable]
    public class ActivitySecrets
    {
        #nullable enable annotations

        [JsonProperty("join")]
        public string? Join { get; set; }

        [JsonProperty("match")]
        public string? Match { get; set; }
    }
}
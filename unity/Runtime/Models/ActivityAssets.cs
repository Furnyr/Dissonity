using System;
using Newtonsoft.Json;

namespace Dissonity.Models
{
    [Serializable]
    public class ActivityAssets
    {
        #nullable enable annotations

        [JsonProperty("large_image")]
        public string? LargeImage { get; set; }

        [JsonProperty("large_text")]
        public string? LargeText { get; set; }

        [JsonProperty("large_url")]
        public string? LargeUrl { get; set; }
        
        [JsonProperty("small_image")]
        public string? SmallImage { get; set; }

        [JsonProperty("small_text")]
        public string? SmallText { get; set; }

        [JsonProperty("small_url")]
        public string? SmallUrl { get; set; }
    }
}
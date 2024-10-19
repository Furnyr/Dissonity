using System;
using Newtonsoft.Json;

namespace Dissonity.Models
{
    [Serializable]
    public class Sku
    {
        #nullable enable annotations

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public SkuType Type { get; set; }

        [JsonProperty("prize")]
        public SkuPrice Price { get; set; }

        [JsonProperty("application_id")]
        public long ApplicationId { get; set; }

        [JsonProperty("flags")]
        public long Flags { get; set; }

        [JsonProperty("release_date")]
        public string? ReleaseDate { get; set; }
    }
}
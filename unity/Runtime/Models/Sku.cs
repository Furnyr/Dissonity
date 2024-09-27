using System;
using Newtonsoft.Json;

namespace Dissonity.Models
{
    //todo Not released yet. Some models related to this functionality may not be implemented.
    [Serializable]
    internal class Sku
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public SkuType Type { get; set; }

        [JsonProperty("prize")]
        public SkuPrize Prize { get; set; }

        [JsonProperty("application_id")]
        public string ApplicationId { get; set; }

        [JsonProperty("flags")]
        public int Flags { get; set; }

        [JsonProperty("release_date")]
        public string ReleaseDate { get; set; }
    }
}
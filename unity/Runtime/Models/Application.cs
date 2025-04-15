using System;
using Newtonsoft.Json;

namespace Dissonity.Models
{
    [Serializable]
    public class Application
    {
        #nullable enable annotations

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("description")]
        public string Description { get; set; }
        
        [JsonProperty("icon")]
        public string? Icon { get; set; }

        [JsonProperty("cover_image")]
        public string? CoverImage { get; set; }
    }
}
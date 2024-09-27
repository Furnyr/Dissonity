using System;
using Newtonsoft.Json;

namespace Dissonity.Models
{
    [Serializable]
    public class MessageApplication
    {
        #nullable enable annotations

        [JsonProperty("id")]
        public string Id { get; set; }
        
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("cover_image")]
        public string? CoverImage { get; set; }

        [JsonProperty("icon")]
        public string? Icon { get; set; }
    }
}
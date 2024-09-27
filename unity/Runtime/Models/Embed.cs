using System;
using Newtonsoft.Json;

namespace Dissonity.Models
{
    [Serializable]
    public class Embed
    {
        #nullable enable annotations

        [JsonProperty("title")]
        public string? Title { get; set; }

        [JsonProperty("type")]
        public string? Type { get; set; }

        [JsonProperty("description")]
        public string? Description { get; set; }
        
        [JsonProperty("url")]
        public string? Url { get; set; }

        [JsonProperty("timestamp")]
        public string? Timestamp { get; set; }

        [JsonProperty("color")]
        public int? Color { get; set; }

        [JsonProperty("footer")]
        public EmbedFooter? Footer { get; set; }

        [JsonProperty("image")]
        public Image? Image { get; set; }

        [JsonProperty("thumbnail")]
        public Image? Thumbnail { get; set; }
        
        [JsonProperty("video")]
        public Video? Video { get; set; }
        
        [JsonProperty("provider")]
        public EmbedProvider? Provider { get; set; }
        
        [JsonProperty("author")]
        public EmbedAuthor? Author { get; set; }
        
        [JsonProperty("fields", NullValueHandling = NullValueHandling.Ignore)]
        public EmbedField[] Field { get; set; }
    }
}
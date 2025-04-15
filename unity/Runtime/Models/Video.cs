using System;
using Newtonsoft.Json;

namespace Dissonity.Models
{
    [Serializable]
    public class Video
    {
        [JsonProperty("url")]
        public string Url { get; set; }
        
        [JsonProperty("height")]
        public int? Height { get; set; }

        [JsonProperty("width")]
        public int? Width { get; set; }
    }
}
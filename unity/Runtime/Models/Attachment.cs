using System;
using Newtonsoft.Json;

namespace Dissonity.Models
{
    [Serializable]
    public class Attachment
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("size")]
        public int Size { get; set; }

        [JsonProperty("filename")]
        public string Filename { get; set; }

        [JsonProperty("proxy_url")]
        public string ProxyUrl { get; set; }
        
        [JsonProperty("height")]
        public int? Height { get; set; }
        
        [JsonProperty("width")]
        public int? Width { get; set; }
    }
}
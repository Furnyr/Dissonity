using System;
using Newtonsoft.Json;

namespace Dissonity.Models
{
    [Serializable]
    public class EmbedProvider
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }
}
using System;
using Newtonsoft.Json;

namespace Dissonity.Models
{
    [Serializable]
    public class Reaction
    {
        [JsonProperty("emoji")]
        public Emoji Emoji { get; set; }

        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("me")]
        public bool Me { get; set; }
    }
}
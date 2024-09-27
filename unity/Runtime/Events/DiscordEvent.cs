using System;
using Newtonsoft.Json;

namespace Dissonity.Events
{
    [Serializable]
    internal class DiscordEvent
    {
        #nullable enable annotations

        [JsonProperty("evt")]
        public string? Event { get; set; }
        
        [JsonProperty("nonce")]
        public string? Nonce { get; set; }

        [JsonProperty("data")]
        public object Data { get; set; }
        
        [JsonProperty("cmd")]
        public string Command { get; set; }
    }
}
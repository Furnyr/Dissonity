using System;
using Dissonity.Models;
using Newtonsoft.Json;

namespace Dissonity.Events
{
    [Serializable]
    internal class ReadyEvent : DiscordEvent
    {
        [JsonProperty("data")]
        new public ReadyEventData Data { get; set; }
    }

    [Serializable]
    public class ReadyEventData
    {
        #nullable enable annotations

        [JsonProperty("v")]
        public int Version { get; set; }
        
        [JsonProperty("config")]
        public ReadyConfig Config { get; set; }
        
        // This property seems to always be null in a normal RPC connection
        [JsonProperty("user")]
        public BaseUser? User { get; set; }
    }
}
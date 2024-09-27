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
        
        //todo when is this data sent? is is always null?
        [JsonProperty("user")]
        public BaseUser? User { get; set; }
    }
}
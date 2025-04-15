using System;
using Dissonity.Models.Interop;
using Newtonsoft.Json;

namespace Dissonity.Events
{
    [Serializable]
    internal class ErrorEvent : DiscordEvent
    {
        [JsonProperty("data")]
        new public ErrorEventData Data { get; set; }
    }

    [Serializable]
    public class ErrorEventData
    {
        [JsonProperty("code")]
        public int Code { get; set; }
        
        [JsonProperty("message")] 
        public string Message { get; set; }
    }
}
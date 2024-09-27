using System;
using Dissonity.Events;
using Newtonsoft.Json;

namespace Dissonity.Commands.Responses
{
    internal class EncourageHardwareAccelerationResponse : DiscordEvent
    {
        [JsonProperty("data")]
        new public EncourageHardwareAccelerationData Data { get; set; }
    }

    [Serializable]
    public class EncourageHardwareAccelerationData
    {
        [JsonProperty("enabled")]
        public bool Enabled { get; set; }
    }
}
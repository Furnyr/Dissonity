using System;
using Newtonsoft.Json;
using Dissonity.Models;

namespace Dissonity.Events
{
    [Serializable]
    internal class SpeakingStart : DiscordEvent
    {
        [JsonProperty("data")]
        new public SpeakingData Data { get; set; }
    }
}
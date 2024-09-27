using System;
using Dissonity.Models;
using Newtonsoft.Json;

namespace Dissonity.Events
{
    [Serializable]
    internal class VoiceStateUpdate : DiscordEvent
    {
        [JsonProperty("data")]
        new public UserVoiceState Data { get; set; }
    }
}
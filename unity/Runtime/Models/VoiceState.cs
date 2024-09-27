using System;
using Newtonsoft.Json;

namespace Dissonity.Models
{
    [Serializable]
    public class VoiceState
    {
        [JsonProperty("mute")]
        public bool Mute { get; set; }

        [JsonProperty("deaf")]
        public bool Deaf { get; set; }

        [JsonProperty("self_mute")]
        public bool SelfMute { get; set; }

        [JsonProperty("self_deaf")]
        public bool SelfDeaf { get; set; }

        [JsonProperty("suppress")]
        public bool Suppress { get; set; }
    }
}
using System;
using Newtonsoft.Json;

namespace Dissonity.Models
{
    [Serializable]
    public class UserVoiceState
    {
        [JsonProperty("mute")]
        public bool Mute { get; set; }

        [JsonProperty("nick")]
        public string Nickname { get; set; }
        
        [JsonProperty("user")]
        public User User { get; set; }

        [JsonProperty("voice_state")]
        public VoiceState VoiceState { get; set; }
        
        [JsonProperty("volume")]
        public float Volume { get; set; }
    }
}

using System;
using Newtonsoft.Json;

namespace Dissonity.Models
{
    [Serializable]
    public class ChannelRpc
    {
        #nullable enable annotations

        [JsonProperty("id")]
        public long Id { get; set; }
        
        [JsonProperty("type")]
        public ChannelType Type { get; set; }

        [JsonProperty("guild_id")]
        public long? GuildId { get; set; }
        
        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("topic")]
        public string? Topic { get; set; }
        
        [JsonProperty("bitrate")]
        public double? Bitrate { get; set; }

        [JsonProperty("user_limit")]
        public int? UserLimit { get; set; }
        
        [JsonProperty("position")]
        public int? Position { get; set; }
        
        [JsonProperty("voice_states")]
        public UserVoiceState[] VoiceStates { get; set; } = new UserVoiceState[0];
        
        [JsonProperty("messages")]
        public Message[] Messages { get; set; } = new Message[0];
    }
}
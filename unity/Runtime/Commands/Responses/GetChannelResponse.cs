using System;
using Dissonity.Events;
using Dissonity.Models;
using Newtonsoft.Json;

namespace Dissonity.Commands.Responses
{
    [Serializable]
    internal class GetChannelResponse : DiscordEvent
    {
        [JsonProperty("data")]
        new public GetChannelData Data { get; set; }
    }
    
    [Serializable]
    public class GetChannelData
    {
        #nullable enable annotations

        [JsonProperty("id")]
        public string Id { get; set; }
        
        [JsonProperty("type")]
        public ChannelType Type { get; set; }

        [JsonProperty("guild_id")]
        public string? GuildId { get; set; }
        
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
        public UserVoiceState[] VoiceStates { get; set; }
        
        [JsonProperty("messages")]
        public Message[] Messages { get; set; }
    }
}
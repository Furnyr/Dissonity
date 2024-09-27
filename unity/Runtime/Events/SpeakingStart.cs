using System;
using Newtonsoft.Json;

namespace Dissonity.Events
{
    [Serializable]
    internal class SpeakingStart : DiscordEvent
    {
        [JsonProperty("data")]
        new public SpeakingStartData Data { get; set; }
    }

    [Serializable]
    public class SpeakingStartData
    {
        #nullable enable annotations

        [JsonProperty("lobby_id")]
        public string? LobbyId { get; set; }
        
        [JsonProperty("channel_id")]
        public string? ChannelId { get; set; }
        
        [JsonProperty("user_id")]
        public string UserId { get; set; }
    }
}
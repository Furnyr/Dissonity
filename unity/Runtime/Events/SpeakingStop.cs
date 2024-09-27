using System;
using Newtonsoft.Json;

namespace Dissonity.Events
{
    [Serializable]
    internal class SpeakingStop : DiscordEvent
    {
        [JsonProperty("data")]
        new public SpeakingStopData Data { get; set; }
    }

    [Serializable]
    public class SpeakingStopData
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
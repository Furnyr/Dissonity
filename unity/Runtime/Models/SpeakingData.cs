using System;
using Newtonsoft.Json;

namespace Dissonity.Models
{
    [Serializable]
    public class SpeakingData
    {
        #nullable enable annotations

        [JsonProperty("lobby_id")]
        public long? LobbyId { get; set; }
        
        [JsonProperty("channel_id")]
        public long? ChannelId { get; set; }
        
        [JsonProperty("user_id")]
        public long UserId { get; set; }
    }
}
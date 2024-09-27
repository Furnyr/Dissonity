
using System;
using Newtonsoft.Json;

namespace Dissonity.Models.Interop
{
    [Serializable]
    public class QueryData
    {
        #nullable enable annotations

        [JsonProperty("instance_id")]
        public string? InstanceId { get; set; }

        [JsonProperty("channel_id")]
        public string? ChannelId { get; set; }

        [JsonProperty("guild_id")]
        public string? GuildId { get; set; }

        [JsonProperty("frame_id")]
        public string? FrameId { get; set; }

        [JsonProperty("platform")]
        public string? Platform { get; set; }
    }
}
using System;
using Newtonsoft.Json;

namespace Dissonity.Models
{
    [Serializable]
    public class MessageReference
    {
        #nullable enable annotations

        [JsonProperty("message_id")]
        public string MessageId { get; set; }

        [JsonProperty("channel_id")]
        public string ChannelId { get; set; }

        [JsonProperty("guild_id")]
        public string GuildId { get; set; }
    }
}
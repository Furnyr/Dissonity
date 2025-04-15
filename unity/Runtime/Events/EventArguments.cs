using System;
using Newtonsoft.Json;

namespace Dissonity.Events
{
    [Serializable]
    internal class EventArguments
    {
        #nullable enable annotations

        [JsonProperty("channel_id", NullValueHandling = NullValueHandling.Ignore)]
        public string? ChannelId { get; set; }

        [JsonProperty("guild_id", NullValueHandling = NullValueHandling.Ignore)]
        public string? GuildId { get; set; }
    }
}
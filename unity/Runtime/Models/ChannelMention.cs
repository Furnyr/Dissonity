using System;
using Newtonsoft.Json;

namespace Dissonity.Models
{
    [Serializable]
    public class ChannelMention
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("guild_id")]
        public long GuildId { get; set; }
        
        [JsonProperty("type")]
        public ChannelType Type { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
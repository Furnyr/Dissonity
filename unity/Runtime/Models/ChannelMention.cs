using System;
using Newtonsoft.Json;

namespace Dissonity.Models
{
    [Serializable]
    public class ChannelMention
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("guild_id")]
        public string GuildId { get; set; }
        
        [JsonProperty("type")]
        public int Type { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
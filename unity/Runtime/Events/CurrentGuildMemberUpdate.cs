using System;
using Dissonity.Models;
using Newtonsoft.Json;

namespace Dissonity.Events
{
    [Serializable]
    internal class CurrentGuildMemberUpdate : DiscordEvent
    {
        [JsonProperty("data")]
        new public GuildMemberRpc Data { get; set; }
    }
}
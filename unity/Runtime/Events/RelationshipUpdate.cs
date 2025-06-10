using System;
using Dissonity.Models;
using Newtonsoft.Json;

namespace Dissonity.Events
{
    [Serializable]
    internal class RelationshipUpdate : DiscordEvent
    {
        [JsonProperty("data")]
        new public Relationship Data { get; set; }
    }
}
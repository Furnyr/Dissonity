using System;
using Newtonsoft.Json;

namespace Dissonity.Models
{
    [Serializable]
    internal class Relationship
    {
        #nullable enable annotations

        [JsonProperty("type")]
        public RelationshipType Type { get; set; }

        // Accent color may always be null here.
        [JsonProperty("user")]
        public User User { get; set; }

        [JsonProperty("presence")]
        public Presence? Presence { get; set; }
    }
}
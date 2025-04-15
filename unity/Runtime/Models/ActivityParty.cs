using System;
using Newtonsoft.Json;

namespace Dissonity.Models
{
    [Serializable]
    public class ActivityParty
    {
        #nullable enable annotations

        [JsonProperty("id")]
        public string? Id { get; set; }
        
        [JsonProperty("size", NullValueHandling = NullValueHandling.Ignore)]
        public int[] Size { get; set; } = new int[0];

        // In <Relationship>.presence.activity.party, there is { privacy: z.number().optional() },
        // but from the lack of information around it, I'm choosing not to include it.
    }
}
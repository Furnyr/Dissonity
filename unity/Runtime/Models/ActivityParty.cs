using System;
using Newtonsoft.Json;

namespace Dissonity.Models
{
    [Serializable]
    public class ActivityParty
    {
        #nullable enable annotations

        [JsonProperty("id")]
        public long? Id { get; set; }
        
        [JsonProperty("size", NullValueHandling = NullValueHandling.Ignore)]
        public int[] Size { get; set; } = new int[0];
    }
}
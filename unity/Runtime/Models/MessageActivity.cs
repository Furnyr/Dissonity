using System;
using Newtonsoft.Json;

namespace Dissonity.Models
{
    [Serializable]
    public class MessageActivity
    {
        #nullable enable annotations

        [JsonProperty("type")]
        public int Type { get; set; }
        
        [JsonProperty("party_id")]
        public string? PartyId { get; set; }
    }
}
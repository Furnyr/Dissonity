using System;
using Dissonity.Events;
using Dissonity.Models;
using Newtonsoft.Json;

namespace Dissonity.Commands.Responses
{
    [Serializable]
    internal class GetRelationshipsResponse : DiscordEvent
    {
        [JsonProperty("data")]
        new public GetRelationshipsData Data { get; set; }
    }
    
    [Serializable]
    internal class GetRelationshipsData
    {
        [JsonProperty("relationships")]
        public Relationship[] Relationships { get; set; } = new Relationship[0];
    }
}
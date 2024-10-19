using System;
using Dissonity.Events;
using Dissonity.Models;
using Newtonsoft.Json;

namespace Dissonity.Commands.Responses
{
    [Serializable]
    internal class GetEntitlementsResponse : DiscordEvent
    {
        [JsonProperty("data")]
        new public GetEntitlementsData Data { get; set; }
    }
     
    [Serializable]
    internal class GetEntitlementsData
    {
        [JsonProperty("entitlements")]
        public Entitlement[] Entitlements { get; set; } = new Entitlement[0];
    }
}
using System;
using Dissonity.Models;
using Newtonsoft.Json;

namespace Dissonity.Events
{
    [Serializable]
    internal class EntitlementCreate : DiscordEvent
    {
        [JsonProperty("data")]
        new public EntitlementCreateData Data { get; set; }
    }

    [Serializable]
    public class EntitlementCreateData
    {
        [JsonProperty("entitlement")]
        public Entitlement Entitlement { get; set; }
    }
}
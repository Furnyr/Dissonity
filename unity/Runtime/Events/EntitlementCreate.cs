using System;
using Dissonity.Models;
using Newtonsoft.Json;

namespace Dissonity.Events
{
    //todo Not released yet. Some models related to this functionality may not be implemented.
    /// <summary>
    /// Not released yet
    /// </summary>
    [Serializable]
    internal class EntitlementCreate : DiscordEvent
    {
        [JsonProperty("data")]
        new public EntitlementCreateData Data { get; set; }
    }

    [Serializable]
    internal class EntitlementCreateData
    {
        [JsonProperty("entitlement")]
        public Entitlement Entitlement { get; set; }
    }
}
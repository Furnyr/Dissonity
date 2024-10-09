using System;
using Dissonity.Events;
using Dissonity.Models;
using Newtonsoft.Json;

namespace Dissonity.Commands.Responses
{
    [Serializable]
    internal class StartPurchaseResponse : DiscordEvent
    {
        #nullable enable annotations

        // Discord docs indicate this can be null, but Dissonity always uses empty arrays
        [JsonProperty("data")]
        new public Entitlement[]? Data { get; set; } = new Entitlement[0];
    }
}
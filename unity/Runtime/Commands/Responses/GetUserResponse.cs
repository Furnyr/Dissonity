using System;
using Dissonity.Events;
using Dissonity.Models;
using Newtonsoft.Json;

namespace Dissonity.Commands.Responses
{
    [Serializable]
    internal class GetUserResponse : DiscordEvent
    {
        #nullable enable annotations

        // Accent color may always be null here.
        [JsonProperty("data")]
        new public User? Data { get; set; }
    }
}
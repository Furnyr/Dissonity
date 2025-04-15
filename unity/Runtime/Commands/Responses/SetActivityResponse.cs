using System;
using Dissonity.Events;
using Dissonity.Models;
using Newtonsoft.Json;

namespace Dissonity.Commands.Responses
{
    [Serializable]
    internal class SetActivityResponse : DiscordEvent
    {
        [JsonProperty("data")]
        new public Activity Data { get; set; }
    }
}
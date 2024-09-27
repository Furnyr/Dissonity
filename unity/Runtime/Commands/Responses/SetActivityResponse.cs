using Dissonity.Events;
using Dissonity.Models;
using Newtonsoft.Json;

namespace Dissonity.Commands.Responses
{
    internal class SetActivityResponse : DiscordEvent
    {
        [JsonProperty("data")]
        new public Activity Data { get; set; }
    }
}
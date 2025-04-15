using System;
using Dissonity.Models;
using Newtonsoft.Json;

namespace Dissonity.Events
{
    [Serializable]
    internal class CurrentUserUpdate : DiscordEvent
    {
        [JsonProperty("data")]
        new public User Data { get; set; }
    }
}
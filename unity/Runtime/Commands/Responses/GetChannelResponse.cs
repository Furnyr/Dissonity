using System;
using Dissonity.Events;
using Dissonity.Models;
using Newtonsoft.Json;

namespace Dissonity.Commands.Responses
{
    [Serializable]
    internal class GetChannelResponse : DiscordEvent
    {
        #nullable enable annotations

        [JsonProperty("data")]
        new public ChannelRpc? Data { get; set; }
    }
}
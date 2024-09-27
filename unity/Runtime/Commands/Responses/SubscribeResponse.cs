using System;
using Dissonity.Events;
using Newtonsoft.Json;

namespace Dissonity.Commands.Responses
{
    [Serializable]
    internal class SubscribeResponse : DiscordEvent
    {
        [JsonProperty("data")]
        new public SubscribeData Data { get; set; }
    }

    [Serializable]
    public class SubscribeData
    {
        [JsonProperty("evt")]
        public string Event { get; set; }
    }
}
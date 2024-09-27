using System;
using Dissonity.Events;
using Newtonsoft.Json;

namespace Dissonity.Commands.Responses
{
    internal class AuthorizeResponse : DiscordEvent
    {
        [JsonProperty("data")]
        new public AuthorizeData Data { get; set; }
    }

    [Serializable]
    public class AuthorizeData
    {
        [JsonProperty("code")]
        public string Code { get; set; }
    }
}
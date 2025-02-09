using System;
using Dissonity.Events;
using Newtonsoft.Json;

namespace Dissonity.Commands.Responses
{
    [Serializable]
    internal class ShareLinkResponse : DiscordEvent
    {
        [JsonProperty("data")]
        new public ShareLinkData Data { get; set; }
    }
    
    [Serializable]
    public class ShareLinkData
    {
        [JsonProperty("success")]
        public bool Success { get; set; }
    }
}
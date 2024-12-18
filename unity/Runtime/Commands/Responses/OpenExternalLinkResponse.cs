using System;
using Dissonity.Events;
using Newtonsoft.Json;

namespace Dissonity.Commands.Responses
{
    [Serializable]
    internal class OpenExternalLinkResponse : DiscordEvent
    {
        [JsonProperty("data")]
        new public OpenExternalLinkData Data { get; set; }
    }
    
    [Serializable]
    public class OpenExternalLinkData
    {
        /// <summary>
        /// <c>Opened</c> is null on Discord clients before December 2024 that do not report the result of the open link action.
        /// </summary>
        [JsonProperty("opened")]
        public bool? Opened { get; set; }
    }
}
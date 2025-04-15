using System;
using Dissonity.Events;
using Newtonsoft.Json;

namespace Dissonity.Commands.Responses
{
    [Serializable]
    internal class GetPlatformBehaviorsResponse : DiscordEvent
    {
        [JsonProperty("data")]
        new public GetPlatformBehaviorsData Data { get; set; }
    }
    
    [Serializable]
    public class GetPlatformBehaviorsData
    {
        #nullable enable annotations

        [JsonProperty("iosKeyboardResizesView")]
        public bool? IosKeyboardResizesView { get; set; }
    }
}
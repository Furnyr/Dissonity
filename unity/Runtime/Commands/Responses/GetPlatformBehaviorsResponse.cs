using Dissonity.Events;
using Newtonsoft.Json;

namespace Dissonity.Commands.Responses
{
    internal class GetPlatformBehaviorsResponse : DiscordEvent
    {
        [JsonProperty("data")]
        new public GetPlatformBehaviorsData Data { get; set; }
    }
    
    public class GetPlatformBehaviorsData
    {
        #nullable enable annotations

        [JsonProperty("iosKeyboardResizesView")]
        public bool? IosKeyboardResizesView { get; set; }
    }
}
using Dissonity.Events;
using Newtonsoft.Json;

namespace Dissonity.Commands.Responses
{
    internal class SetConfigResponse : DiscordEvent
    {
        [JsonProperty("data")]
        new public SetConfigData Data { get; set; }
    }
    
    public class SetConfigData
    {
        [JsonProperty("use_interactive_pip")]
        public bool UseInteractivePip { get; set; }
    }
}
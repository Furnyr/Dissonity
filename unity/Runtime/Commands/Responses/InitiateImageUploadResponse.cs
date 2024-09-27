using Dissonity.Events;
using Newtonsoft.Json;

namespace Dissonity.Commands.Responses
{
    internal class InitiateImageUploadResponse : DiscordEvent
    {
        [JsonProperty("data")]
        new public InitiateImageUploadData Data { get; set; }
    }
    
    public class InitiateImageUploadData
    {
        [JsonProperty("image_url")]
        public string ImageUrl { get; set; }
    }
}
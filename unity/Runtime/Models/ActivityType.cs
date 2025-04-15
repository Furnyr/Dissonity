using Dissonity.Json;
using Newtonsoft.Json;

namespace Dissonity.Models
{
    [JsonConverter(typeof(SafeEnumConverter))]
    public enum ActivityType
    {
        Unknown = -1,
        Playing = 0,
        Streaming = 1,
        Listening = 2,
        Watching = 3,
        Custom = 4,
        Competing = 5
    }
}
using Dissonity.Json;
using Newtonsoft.Json;

namespace Dissonity.Models
{
    [JsonConverter(typeof(SafeEnumConverter))]
    public enum MessageActivityType
    {
        Unknown = -1,
        Join = 1,
        Spectate = 2,
        Listen = 3,
        JoinRequest = 5,
    }
}
using Dissonity.Json;
using Newtonsoft.Json;

namespace Dissonity.Models
{
    [JsonConverter(typeof(SafeEnumConverter))]
    public enum OrientationType
    {
        Unknown = -1,
        Portrait = 0,
        Landscape = 1
    }
}
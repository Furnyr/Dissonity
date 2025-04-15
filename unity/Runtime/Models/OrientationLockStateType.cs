using Dissonity.Json;
using Newtonsoft.Json;

namespace Dissonity.Models
{
    [JsonConverter(typeof(SafeEnumConverter))]
    public enum OrientationLockStateType
    {
        Unknown = -1,
        Unlocked = 1,
        Portrait = 2,
        Landscape = 3
    }
}
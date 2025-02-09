using Dissonity.Json;
using Newtonsoft.Json;

namespace Dissonity.Models
{
    [JsonConverter(typeof(SafeEnumConverter))]
    public enum ThermalStateType
    {
        Unknown = -1,
        Nominal = 0,
        Fair = 1,
        Serious = 2,
        Critical = 3
    }
}
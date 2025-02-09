using Dissonity.Json;
using Newtonsoft.Json;

namespace Dissonity.Models
{
    [JsonConverter(typeof(SafeEnumConverter))]
    public enum SkuType
    {
        Unknown = -1,
        Application = 1,
        Dlc = 2,
        Consumable = 3,
        Bundle = 4,
        Subscription = 5
    }
}
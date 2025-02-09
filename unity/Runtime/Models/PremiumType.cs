using Dissonity.Json;
using Newtonsoft.Json;

namespace Dissonity.Models
{
    [JsonConverter(typeof(SafeEnumConverter))]
    public enum PremiumType
    {
        Unknown = -1,
        None = 0,
        NitroClassic = 1,
        Nitro = 2,
        NitroBasic = 3
    }
}
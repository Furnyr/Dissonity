using Dissonity.Json;
using Newtonsoft.Json;

namespace Dissonity.Models
{
    [JsonConverter(typeof(SafeEnumConverter))]
    public enum LayoutModeType
    {
        Unknown = -1,
        Focused = 0,
        Pip = 1,
        Grid = 2
    }
}